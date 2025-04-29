using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;

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
                    .Where(o => o.Status == "В процессе")
                    .Select(o => new
                    {
                        OrderId = o.OrderID,
                        ClientName = o.Customers.FullName,
                        CarMake = o.Vehicles.Make,
                        CarModel = o.Vehicles.Model,
                        Description = o.Problem
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
            // Настройка области графика
            chartOrders.ChartAreas[0].AxisX.Title = "Клиенты";
            chartOrders.ChartAreas[0].AxisY.Title = "Количество заказов";
            chartOrders.ChartAreas[0].AxisX.Interval = 1;
            chartOrders.Series[0].Name = "Заказы в процессе";
        }

        private void UpdateChart()
        {
            if (OrdersDataGrid.ItemsSource == null) return;

            // Очистка предыдущих данных
            chartOrders.Series.Clear();

            var customers = Entities.GetContext().Customers.ToList();
            var vehicles = Entities.GetContext().Vehicles.ToList();
            int total = Entities.GetContext().Orders.Count(o => o.Status == "В процессе");

            // Добавление данных в график
            Series series = new Series();
            series.Points.Add(total);
            series.Points[0].AxisLabel = "В процессе";
            series.IsValueShownAsLabel = true;

            chartOrders.Series.Add(series);

            // Установка типа графика
            string chartType = (cbChartType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Column";
            chartOrders.Series[0].ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), chartType);
        }

        private void UpdateChart_Click(object sender, RoutedEventArgs e)
        {
            UpdateChart();
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = true;
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = workbook.Worksheets[1];

                // Заголовок отчета
                worksheet.Cells[1, 1] = "Отчет по заказам в процессе";
                Excel.Range headerRange = worksheet.Range["A1", $"E1"];
                headerRange.Merge();
                headerRange.Font.Bold = true;
                headerRange.Font.Size = 14;
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Заголовки таблицы
                string[] headers = { "№ Заказа", "Клиент", "Марка", "Модель", "Описание" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[3, i + 1] = headers[i];
                    worksheet.Cells[3, i + 1].Font.Bold = true;
                    worksheet.Cells[3, i + 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }

                // Данные
                int row = 4;
                foreach (var item in OrdersDataGrid.Items)
                {
                    dynamic order = item;
                    worksheet.Cells[row, 1] = order.OrderId;
                    worksheet.Cells[row, 2] = order.ClientName;
                    worksheet.Cells[row, 3] = order.CarMake;
                    worksheet.Cells[row, 4] = order.CarModel;
                    worksheet.Cells[row, 5] = order.Description;

                    // Форматирование границ
                    Excel.Range dataRange = worksheet.Range[$"A{row}", $"E{row}"];
                    dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    row++;
                }

                // Автоподбор ширины столбцов
                worksheet.Columns["A:E"].AutoFit();

                // Добавление сводной информации
                worksheet.Cells[row + 2, 1] = $"Всего заказов в процессе: {OrdersDataGrid.Items.Count}";
                worksheet.Cells[row + 2, 1].Font.Bold = true;

                // Добавление диаграммы
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(100, 300, 400, 250);
                Excel.Chart chart = chartObject.Chart;

                // Создаем данные для диаграммы (по клиентам)
                var chartData = new Dictionary<string, int>();
                foreach (var item in OrdersDataGrid.Items)
                {
                    dynamic order = item;
                    string client = order.ClientName;
                    if (chartData.ContainsKey(client))
                        chartData[client]++;
                    else
                        chartData[client] = 1;
                }

                // Добавляем данные на лист
                int chartRow = row + 4;
                worksheet.Cells[chartRow, 1] = "Клиент";
                worksheet.Cells[chartRow, 2] = "Количество заказов";
                worksheet.Cells[chartRow, 1].Font.Bold = true;
                worksheet.Cells[chartRow, 2].Font.Bold = true;

                foreach (var item in chartData)
                {
                    chartRow++;
                    worksheet.Cells[chartRow, 1] = item.Key;
                    worksheet.Cells[chartRow, 2] = item.Value;
                }

                // Настройка диаграммы
                Excel.Range chartRange = worksheet.Range[$"A{row + 4}", $"B{chartRow}"];
                chart.SetSourceData(chartRange);
                chart.ChartType = Excel.XlChartType.xlColumnClustered;
                chart.HasTitle = true;
                chart.ChartTitle.Text = "Распределение заказов по клиентам";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wordApp = new Word.Application();
                wordApp.Visible = true;
                Word.Document document = wordApp.Documents.Add();

                // Заголовок документа
                Word.Paragraph title = document.Paragraphs.Add();
                title.Range.Text = "Отчет по заказам в процессе";
                title.Range.Font.Bold = 1;
                title.Range.Font.Size = 16;
                title.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.InsertParagraphAfter();

                // Сводная информация
                Word.Paragraph summary = document.Paragraphs.Add();
                summary.Range.Text = $"Всего заказов в процессе: {OrdersDataGrid.Items.Count}";
                summary.Range.Font.Bold = 1;
                summary.Format.SpaceAfter = 12;
                summary.Range.InsertParagraphAfter();

                // Таблица с данными
                Word.Table table = document.Tables.Add(
                    summary.Range,
                    OrdersDataGrid.Items.Count + 1,
                    5);  // 5 столбцов: №, Клиент, Марка, Модель, Описание

                // Заголовки таблицы
                table.Cell(1, 1).Range.Text = "№ Заказа";
                table.Cell(1, 2).Range.Text = "Клиент";
                table.Cell(1, 3).Range.Text = "Марка";
                table.Cell(1, 4).Range.Text = "Модель";
                table.Cell(1, 5).Range.Text = "Описание";

                // Форматирование заголовков
                for (int i = 1; i <= 5; i++)
                {
                    table.Cell(1, i).Range.Font.Bold = 1;
                    table.Cell(1, i).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }

                // Заполнение данными
                int row = 2;
                foreach (var item in OrdersDataGrid.Items)
                {
                    dynamic order = item;
                    table.Cell(row, 1).Range.Text = order.OrderId;
                    table.Cell(row, 2).Range.Text = order.ClientName;
                    table.Cell(row, 3).Range.Text = order.CarMake;
                    table.Cell(row, 4).Range.Text = order.CarModel;
                    table.Cell(row, 5).Range.Text = order.Description;
                    row++;
                }

                // Стиль таблицы
                table.Borders.Enable = 1;
                table.Rows[1].Range.Font.Bold = 1;
                table.Rows[1].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray15;

                // Добавление диаграммы
                Word.Paragraph chartTitle = document.Paragraphs.Add();
                chartTitle.Range.Text = "Распределение заказов по клиентам";
                chartTitle.Range.Font.Bold = 1;
                chartTitle.Range.Font.Size = 14;
                chartTitle.Format.SpaceAfter = 12;
                chartTitle.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                chartTitle.Range.InsertParagraphAfter();

                // Вставка изображения графика
                string tempImagePath = System.IO.Path.GetTempFileName() + ".png";
                chartOrders.SaveImage(tempImagePath, ChartImageFormat.Png);
                document.InlineShapes.AddPicture(tempImagePath);
                System.IO.File.Delete(tempImagePath);

                // Автоподбор ширины таблицы
                table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}