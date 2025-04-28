using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

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
                        Description = o.Problem,
                        TotalPrice = o.Payments.FirstOrDefault().Amount
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
            chartOrders.ChartAreas[0].AxisY.Title = "Сумма (₽)";
            chartOrders.ChartAreas[0].AxisX.Interval = 1;
            chartOrders.ChartAreas[0].AxisY.Interval = 5000;
        }

        private void UpdateChart()
        {
            if (OrdersDataGrid.ItemsSource == null) return;

            // Очистка предыдущих данных
            chartOrders.Series[0].Points.Clear();

            // Получение данных для графика
            var chartData = OrdersDataGrid.ItemsSource.Cast<dynamic>()
                .Select(x => new { Client = x.ClientName, Amount = (double)x.TotalPrice })
                .ToList();

            // Добавление данных в график
            foreach (var item in chartData)
            {
                chartOrders.Series[0].Points.AddXY(item.Client, item.Amount);
            }

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

                // Заголовки
                for (int i = 0; i < OrdersDataGrid.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = OrdersDataGrid.Columns[i].Header;
                }

                // Данные
                for (int i = 0; i < OrdersDataGrid.Items.Count; i++)
                {
                    dynamic item = OrdersDataGrid.Items[i];
                    worksheet.Cells[i + 2, 1] = item.OrderId;
                    worksheet.Cells[i + 2, 2] = item.ClientName;
                    worksheet.Cells[i + 2, 3] = item.CarMake;
                    worksheet.Cells[i + 2, 4] = item.CarModel;
                    worksheet.Cells[i + 2, 5] = item.Description;
                    worksheet.Cells[i + 2, 6] = item.TotalPrice;
                }

                // Добавление графика
                Excel.ChartObjects chartObjects = worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(100, 100, 500, 300);
                Excel.Chart chart = chartObject.Chart;

                Excel.Range range = worksheet.Range["A1", $"F{OrdersDataGrid.Items.Count + 1}"];
                chart.SetSourceData(range);
                chart.ChartType = Excel.XlChartType.xlColumnClustered;

                // Автоподбор ширины
                worksheet.Columns.AutoFit();
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

                // Заголовок
                Word.Paragraph titlePara = document.Paragraphs.Add();
                titlePara.Range.Text = "Отчет по заказам";
                titlePara.Range.Font.Bold = 1;
                titlePara.Range.Font.Size = 16;
                titlePara.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titlePara.Range.InsertParagraphAfter();

                // Таблица с данными
                Word.Table table = document.Tables.Add(
                    titlePara.Range,
                    OrdersDataGrid.Items.Count + 1,
                    OrdersDataGrid.Columns.Count);
                table.Borders.Enable = 1;

                // Заголовки таблицы
                for (int i = 0; i < OrdersDataGrid.Columns.Count; i++)
                {
                    table.Cell(1, i + 1).Range.Text = OrdersDataGrid.Columns[i].Header.ToString();
                    table.Cell(1, i + 1).Range.Font.Bold = 1;
                }

                // Данные таблицы
                for (int i = 0; i < OrdersDataGrid.Items.Count; i++)
                {
                    dynamic item = OrdersDataGrid.Items[i];
                    table.Cell(i + 2, 1).Range.Text = item.OrderId.ToString();
                    table.Cell(i + 2, 2).Range.Text = item.ClientName;
                    table.Cell(i + 2, 3).Range.Text = item.CarMake;
                    table.Cell(i + 2, 4).Range.Text = item.CarModel;
                    table.Cell(i + 2, 5).Range.Text = item.Description;
                    table.Cell(i + 2, 6).Range.Text = item.TotalPrice.ToString("N0") + " ₽";
                }

                // Вставка изображения графика
                string tempImagePath = System.IO.Path.GetTempFileName() + ".png";
                chartOrders.SaveImage(tempImagePath, ChartImageFormat.Png);
                document.InlineShapes.AddPicture(tempImagePath);
                System.IO.File.Delete(tempImagePath);
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