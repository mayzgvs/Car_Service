using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Drawing;
using Color = System.Drawing.Color;

namespace Car_Service.Pages
{
    public partial class Otchet2 : Page
    {
        private Entities _context = new Entities();

        public Otchet2()
        {
            InitializeComponent();
            LoadData();
            InitializeChart();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка транспортных средств с информацией о владельцах и заказах
                var vehicles = _context.Vehicles
                    .Include("Customers")
                    .Include("Orders")
                    .Select(v => new
                    {
                        v.VehicleID,
                        v.Make,
                        v.Model,
                        v.VIN,
                        v.Customers,
                        v.Orders
                    })
                    .ToList();

                VehiclesDataGrid.ItemsSource = vehicles;
                UpdateChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                chartArea.AxisX.Title = "Марки автомобилей";
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
            
            chart.Series.Clear();

            // Получение данных по маркам автомобилей
            var vehicleStats = _context.Vehicles
                .GroupBy(v => v.Make)
                .Select(g => new
                {
                    Make = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Тип графика
            string chartType = (cbChartType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Column";

            // Создание серии
            Series series = new Series("Автомобили");
            series.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), chartType);
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0}";
            series.LabelForeColor = Color.White;

            // Добавление данных
            foreach (var stat in vehicleStats)
            {
                var point = series.Points.AddXY(stat.Make, stat.Count);
            }

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
            var vehicles = _context.Vehicles
                .Include("Customers")
                .Include("Orders")
                .ToList();

            var application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Worksheets[1];

            // Заголовок отчета
            worksheet.Cells[1, 1] = $"Отчет по транспортным средствам на {DateTime.Now.ToString("dd-MM-yyyy")}";
            Excel.Range headerRange = worksheet.Range["A1:F1"];
            headerRange.Merge();
            headerRange.Font.Bold = true;
            headerRange.Font.Size = 14;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // Статистика по маркам
            var topMakes = vehicles
                .GroupBy(v => v.Make)
                .Select(g => new { Make = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            worksheet.Cells[3, 1] = "Топ-5 марок автомобилей:";
            for (int i = 0; i < topMakes.Count; i++)
            {
                worksheet.Cells[4 + i, 1] = $"{topMakes[i].Make} - {topMakes[i].Count} авто";
            }

            // Заголовки таблицы
            worksheet.Cells[10, 1] = "Марка";
            worksheet.Cells[10, 2] = "Модель";
            worksheet.Cells[10, 3] = "VIN";
            worksheet.Cells[10, 4] = "Владелец";
            worksheet.Cells[10, 5] = "Кол-во заказов";

            Excel.Range tableHeader = worksheet.Range["A10:F10"];
            tableHeader.Font.Bold = true;
            tableHeader.Interior.Color = Excel.XlRgbColor.rgbLightGray;

            // Заполнение данными
            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicle = vehicles[i];
                worksheet.Cells[i + 11, 1] = vehicle.Make;
                worksheet.Cells[i + 11, 2] = vehicle.Model;
                worksheet.Cells[i + 11, 3] = vehicle.VIN;
                worksheet.Cells[i + 11, 4] = vehicle.Customers?.FullName ?? "Не указан";
                worksheet.Cells[i + 11, 5] = vehicle.Orders.Count;
            }

            // Форматирование таблицы
            Excel.Range dataRange = worksheet.Range[$"A10:F{10 + vehicles.Count}"];
            dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
            dataRange.Columns.AutoFit();

            // Сохранение файла
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"VehicleReport_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx";
            string filePath = System.IO.Path.Combine(desktop, fileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                fileName = $"VehicleReport_{DateTime.Now.ToString("dd-MM-yyyy")}_{counter}.xlsx";
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
            var vehicles = _context.Vehicles
                .Include("Customers")
                .Include("Orders")
                .ToList();

            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            // Заголовок отчета
            Word.Paragraph titleParagraph = document.Paragraphs.Add();
            Word.Range titleRange = titleParagraph.Range;
            titleRange.Text = $"Отчет по транспортным средствам на {DateTime.Now.ToString("dd-MM-yyyy")}";
            titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            titleRange.Font.Name = "Times New Roman";
            titleRange.Font.Size = 16;
            titleRange.Font.Bold = 1;
            titleRange.InsertParagraphAfter();

            // Статистика по маркам
            var topMakes = vehicles
                .GroupBy(v => v.Make)
                .Select(g => new { Make = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            Word.Paragraph statsParagraph = document.Paragraphs.Add();
            Word.Range statsRange = statsParagraph.Range;
            statsRange.Text = "Топ-5 марок автомобилей:\n" + 
                string.Join("\n", topMakes.Select(m => $"{m.Make} - {m.Count} авто"));
            statsRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            statsRange.Font.Name = "Times New Roman";
            statsRange.Font.Size = 14;
            statsRange.Font.Bold = 1;
            statsRange.InsertParagraphAfter();

            // Таблица с транспортными средствами
            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table vehiclesTable = document.Tables.Add(tableRange, vehicles.Count + 1, 6);

            vehiclesTable.Borders.InsideLineStyle = vehiclesTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            vehiclesTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            // Заголовки таблицы
            vehiclesTable.Cell(1, 1).Range.Text = "Марка";
            vehiclesTable.Cell(1, 2).Range.Text = "Модель";
            vehiclesTable.Cell(1, 3).Range.Text = "VIN";
            vehiclesTable.Cell(1, 4).Range.Text = "Владелец";
            vehiclesTable.Cell(1, 5).Range.Text = "Кол-во заказов";

            // Форматирование заголовков
            Word.Range headerRange = vehiclesTable.Rows[1].Range;
            headerRange.Font.Bold = 1;
            headerRange.Font.Name = "Times New Roman";
            headerRange.Font.Size = 14;
            headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            headerRange.Shading.BackgroundPatternColor = (Word.WdColor)(0xEDEDED);

            // Заполнение таблицы данными
            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicle = vehicles[i];
                vehiclesTable.Cell(i + 2, 1).Range.Text = vehicle.Make;
                vehiclesTable.Cell(i + 2, 2).Range.Text = vehicle.Model;
                vehiclesTable.Cell(i + 2, 3).Range.Text = vehicle.VIN;
                vehiclesTable.Cell(i + 2, 4).Range.Text = vehicle.Customers?.FullName ?? "Не указан";
                vehiclesTable.Cell(i + 2, 5).Range.Text = vehicle.Orders.Count.ToString();

                // Форматирование строк
                for (int j = 1; j <= 6; j++)
                {
                    vehiclesTable.Cell(i + 2, j).Range.Font.Name = "Times New Roman";
                    vehiclesTable.Cell(i + 2, j).Range.Font.Size = 12;
                }
            }

            // Автоподбор ширины столбцов
            vehiclesTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);

            // Сохранение документа
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = System.IO.Path.Combine(desktop, $"VehicleReport_{DateTime.Now.ToString("dd-MM-yyyy")}.docx");

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = System.IO.Path.Combine(desktop, $"VehicleReport_{DateTime.Now.ToString("dd-MM-yyyy")}_{counter}.docx");
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