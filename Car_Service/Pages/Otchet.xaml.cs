using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Drawing;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;
using Color = System.Drawing.Color;

namespace Car_Service.Pages
{
    public partial class Otchet : Page
    {
        private Entities _context = new Entities();

        public Otchet()
        {
            InitializeComponent();
            LoadData();
            InitializeChart();

            OrdersDataGrid.ItemsSource = Entities.GetContext().Orders.ToList();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка заказов
                var completedOrders = _context.Orders
                    //.Where(o => o.Status == "В процессе")
                    .Select(o => new
                    {
                        OrderId = o.OrderID,
                        ClientName = o.Customers.FullName,
                        CarMake = o.Vehicles.Make,
                        CarModel = o.Vehicles.Model,
                        Description = o.Problem,
                        Status = o.Status
                    })
                    .ToList();

                OrdersDataGrid.ItemsSource = completedOrders;
                UpdateChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void InitializeChart()
        {
            if (wfhChart.Child is Chart chart)
            {
                chart.Series.Clear();
                chart.ChartAreas.Clear();
                chart.Legends.Clear();

                // Настройка области графика
                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Статусы заказов";
                chartArea.AxisY.Title = "Количество";
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.BackColor = Color.White;

                chart.ChartAreas.Add(chartArea);

                // Настройка легенды
                Legend legend = new Legend();
                legend.BackColor = Color.Transparent;
                chart.Legends.Add(legend);
            }
        }

        private void UpdateChart()
        {
            if (!(wfhChart.Child is Chart chart)) return;
            if (OrdersDataGrid.ItemsSource == null) return;

            chart.Series.Clear();

            // Получение данных
            var ordersInProcess = _context.Orders.Count(o => o.Status == "В процессе");
            var ordersCompleted = _context.Orders.Count(o => o.Status == "Выполнен");

            // Тип графика
            string chartType = (cbChartType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Column";

            // Создание серии
            Series series = new Series("Заказы");
            series.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), chartType);
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0}";
            series.LabelForeColor = Color.White;

            // Добавление данных
            series.Points.AddXY("В процессе", ordersInProcess);
            series.Points.AddXY("Выполненные", ordersCompleted);

            // Настройка цветов
            series.Points[0].Color = Color.FromArgb(65, 140, 240);
            series.Points[1].Color = Color.FromArgb(70, 200, 120);

            if (chartType == "Pie" || chartType == "Doughnut")
            {
                series["PieLabelStyle"] = "Outside";
                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
            }

            chart.Series.Add(series);
        }


        private void UpdateChart_Click(object sender, RoutedEventArgs e)
        {
            UpdateChart();
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var orders = Entities.GetContext().Orders
                .Where(o => o.Status == "В процессе" || o.Status == "Выполнен")
                .OrderBy(x => x.Status)
                .ThenBy(x => x.OrderID)
                .ToList();

            var customers = Entities.GetContext().Customers.ToList();
            var vehicles = Entities.GetContext().Vehicles.ToList();

            var application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Worksheets[1];

            // Заголовок отчета
            worksheet.Cells[1, 1] = $"Отчет по заказам на {DateTime.Now.ToString("dd-MM-yyyy")}";
            Excel.Range headerRange = worksheet.Range["A1:E1"];
            headerRange.Merge();
            headerRange.Font.Bold = true;
            headerRange.Font.Size = 14;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // Статистика по статусам
            var inProcessCount = orders.Count(o => o.Status == "В процессе");
            var completedCount = orders.Count(o => o.Status == "Выполнен");

            worksheet.Cells[2, 1] = $"Заказов в процессе: {inProcessCount}";
            worksheet.Cells[3, 1] = $"Выполненных заказов: {completedCount}";
            worksheet.Range["A2:A3"].Font.Bold = true;

            // Заголовки таблицы
            worksheet.Cells[5, 1] = "Статус";
            worksheet.Cells[5, 2] = "№ Заказа";
            worksheet.Cells[5, 3] = "Клиент";
            worksheet.Cells[5, 4] = "Автомобиль";
            worksheet.Cells[5, 5] = "Описание проблемы";

            Excel.Range tableHeader = worksheet.Range["A5:E5"];
            tableHeader.Font.Bold = true;
            tableHeader.Interior.Color = Excel.XlRgbColor.rgbLightGray;

            // Заполнение данными
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                var customer = customers.FirstOrDefault(c => c.CustomerID == order.CustomerID);
                var vehicle = vehicles.FirstOrDefault(v => v.VehicleID == order.VehicleID);

                worksheet.Cells[i + 6, 1] = order.Status;
                worksheet.Cells[i + 6, 2] = order.OrderID;
                worksheet.Cells[i + 6, 3] = customer?.FullName ?? "Не указан";
                worksheet.Cells[i + 6, 4] = $"{vehicle?.Make ?? "Не указана"} {vehicle?.Model ?? ""}";
                worksheet.Cells[i + 6, 5] = order.Problem ?? "Не указана";

                // Подсветка строк по статусу
                var rowRange = worksheet.Range[$"A{i + 6}:E{i + 6}"];
                if (order.Status == "В процессе")
                    rowRange.Interior.Color = Excel.XlRgbColor.rgbLightYellow;
                else
                    rowRange.Interior.Color = Excel.XlRgbColor.rgbLightGreen;
            }

            // Форматирование таблицы
            Excel.Range dataRange = worksheet.Range[$"A5:E{5 + orders.Count}"];
            dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
            dataRange.Columns.AutoFit();

            // Сохранение файла
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"OrdersReport_All_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx";
            string filePath = System.IO.Path.Combine(desktop, fileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                fileName = $"OrdersReport_All_{DateTime.Now.ToString("dd-MM-yyyy")}_{counter}.xlsx";
                filePath = System.IO.Path.Combine(desktop, fileName);
                counter++;
            }

            workbook.SaveAs(filePath);
            workbook.Close();
            application.Quit();

            MessageBox.Show($"Отчет успешно сохранен:\n{filePath}", "Экспорт завершен",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var orders = Entities.GetContext().Orders
                .Where(o => o.Status == "В процессе" || o.Status == "Выполнен")
                .OrderBy(x => x.Status)
                .ThenBy(x => x.OrderID)
                .ToList();

            var customers = Entities.GetContext().Customers.ToList();
            var vehicles = Entities.GetContext().Vehicles.ToList();

            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            // Заголовок отчета
            Word.Paragraph titleParagraph = document.Paragraphs.Add();
            Word.Range titleRange = titleParagraph.Range;
            titleRange.Text = $"Отчет по заказам на {DateTime.Now.ToString("dd-MM-yyyy")}";
            titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            titleRange.Font.Name = "Times New Roman";
            titleRange.Font.Size = 16;
            titleRange.Font.Bold = 1;
            titleRange.InsertParagraphAfter();

            // Статистика
            var inProcessCount = orders.Count(o => o.Status == "В процессе");
            var completedCount = orders.Count(o => o.Status == "Выполнен");

            Word.Paragraph statsParagraph = document.Paragraphs.Add();
            Word.Range statsRange = statsParagraph.Range;
            statsRange.Text = $"Заказов в процессе: {inProcessCount}\nВыполненных заказов: {completedCount}";
            statsRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            statsRange.Font.Name = "Times New Roman";
            statsRange.Font.Size = 14;
            statsRange.Font.Bold = 1;
            statsRange.InsertParagraphAfter();

            // Таблица с заказами
            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table ordersTable = document.Tables.Add(tableRange, orders.Count + 1, 5);

            ordersTable.Borders.InsideLineStyle = ordersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            ordersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            // Заголовки таблицы
            ordersTable.Cell(1, 1).Range.Text = "Статус";
            ordersTable.Cell(1, 2).Range.Text = "№ Заказа";
            ordersTable.Cell(1, 3).Range.Text = "Клиент";
            ordersTable.Cell(1, 4).Range.Text = "Автомобиль";
            ordersTable.Cell(1, 5).Range.Text = "Описание проблемы";

            // Форматирование заголовков - мягкий серый
            Word.Range headerRange = ordersTable.Rows[1].Range;
            headerRange.Font.Bold = 1;
            headerRange.Font.Name = "Times New Roman";
            headerRange.Font.Size = 14;
            headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            headerRange.Shading.BackgroundPatternColor = (Word.WdColor)(0xEDEDED); // Очень светлый серый

            // Заполнение таблицы данными
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                var customer = customers.FirstOrDefault(c => c.CustomerID == order.CustomerID);
                var vehicle = vehicles.FirstOrDefault(v => v.VehicleID == order.VehicleID);

                ordersTable.Cell(i + 2, 1).Range.Text = order.Status;
                ordersTable.Cell(i + 2, 2).Range.Text = order.OrderID.ToString();
                ordersTable.Cell(i + 2, 3).Range.Text = customer?.FullName ?? "Не указан";
                ordersTable.Cell(i + 2, 4).Range.Text = $"{vehicle?.Make ?? "Не указана"} {vehicle?.Model ?? ""}";
                ordersTable.Cell(i + 2, 5).Range.Text = order.Problem ?? "Не указана";

                // Мягкая подсветка строк по статусу
                if (order.Status == "В процессе")
                    ordersTable.Rows[i + 2].Shading.BackgroundPatternColor = (Word.WdColor)(0xE6F3FF); // Очень светлый голубой
                else
                    ordersTable.Rows[i + 2].Shading.BackgroundPatternColor = (Word.WdColor)(0xE8F5E9); // Очень светлый зеленый

                // Форматирование строк
                for (int j = 1; j <= 5; j++)
                {
                    ordersTable.Cell(i + 2, j).Range.Font.Name = "Times New Roman";
                    ordersTable.Cell(i + 2, j).Range.Font.Size = 12;
                }
            }

            // Автоподбор ширины столбцов
            ordersTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);

            // Добавляем пробелы после таблицы для лучшего вида
            Word.Paragraph afterTableParagraph = document.Paragraphs.Add();
            afterTableParagraph.Range.InsertParagraphAfter();
            afterTableParagraph.Range.InsertParagraphAfter();

            // Сохранение документа
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = System.IO.Path.Combine(desktop, $"OrdersReport_All_{DateTime.Now.ToString("dd-MM-yyyy")}.docx");

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = System.IO.Path.Combine(desktop, $"OrdersReport_All_{DateTime.Now.ToString("dd-MM-yyyy")}_{counter}.docx");
                counter++;
            }

            document.SaveAs2(filePath);
            document.Close();
            application.Quit();

            MessageBox.Show($"Отчет сохранен в: {filePath}", "Экспорт завершен",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}