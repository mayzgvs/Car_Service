using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Car_Service.Pages;

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private bool _isSearchMode = false;
        private DataGrid _resultsGrid;
        private Button _cancelSearchButton;

        public MainPage()
        {
            InitializeComponent();

            SearchTextBox.GotFocus += SearchTextBox_GotFocus;
            SearchTextBox.LostFocus += SearchTextBox_LostFocus;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            PageCategory.SelectionChanged += PageFilter_SelectionChanged;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_isSearchMode)
            {
                EnterSearchMode();
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Теперь выход из режима поиска только по кнопке отмены
        }

        private void EnterSearchMode()
        {
            _isSearchMode = true;

            // Скрываем все элементы управления, кроме панели поиска
            ButtonOtchet.Visibility = Visibility.Collapsed;
            ButtonOtchetVIN.Visibility = Visibility.Collapsed;

            ButtonCustomers.Visibility = Visibility.Collapsed;
            ButtonCars.Visibility = Visibility.Collapsed;
            ButtonEmployees.Visibility = Visibility.Collapsed;
            ButtonInventory.Visibility = Visibility.Collapsed;
            ButtonParts.Visibility = Visibility.Collapsed;

            ButtonService.Visibility = Visibility.Collapsed;
            ButtonOrders.Visibility = Visibility.Collapsed;
            ButtonWork.Visibility = Visibility.Collapsed;
            ButtonPay.Visibility = Visibility.Collapsed;
            ButtonFeedback.Visibility = Visibility.Collapsed;

            // Создаем кнопку отмены поиска
            _cancelSearchButton = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5),
                Style = (Style)FindResource("ActiveButtonStyle")
            };
            _cancelSearchButton.Click += CancelSearchButton_Click;

            // Добавляем кнопку отмены в панель поиска
            var searchPanel = (StackPanel)((Border)MainGrid.Children[0]).Child;
            searchPanel.Children.Add(_cancelSearchButton);

            // Создаем и добавляем DataGrid для отображения результатов
            _resultsGrid = new DataGrid
            {
                AutoGenerateColumns = true,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsReadOnly = true,
                SelectionMode = DataGridSelectionMode.Single,
                Style = (Style)FindResource("DataGridStyle")
            };

            Grid.SetRow(_resultsGrid, 2);
            Grid.SetColumnSpan(_resultsGrid, 2);

            MainGrid.Children.Add(_resultsGrid);

            // Фокусируемся на поле поиска
            SearchTextBox.Focus();
        }

        private void ExitSearchMode()
        {
            _isSearchMode = false;

            // Удаляем кнопку отмены
            if (_cancelSearchButton != null)
            {
                var searchPanel = (StackPanel)((Border)MainGrid.Children[0]).Child;
                searchPanel.Children.Remove(_cancelSearchButton);
                _cancelSearchButton.Click -= CancelSearchButton_Click;
                _cancelSearchButton = null;
            }

            // Восстанавливаем видимость всех элементов
            ButtonOtchet.Visibility = Visibility.Visible;
            ButtonOtchetVIN.Visibility = Visibility.Visible;

            ButtonCustomers.Visibility = Visibility.Visible;
            ButtonCars.Visibility = Visibility.Visible;
            ButtonEmployees.Visibility = Visibility.Visible;
            ButtonInventory.Visibility = Visibility.Visible;
            ButtonParts.Visibility = Visibility.Visible;

            ButtonService.Visibility = Visibility.Visible;
            ButtonOrders.Visibility = Visibility.Visible;
            ButtonWork.Visibility = Visibility.Visible;
            ButtonPay.Visibility = Visibility.Visible;
            ButtonFeedback.Visibility = Visibility.Visible;

            // Удаляем DataGrid с результатами
            if (_resultsGrid != null && MainGrid.Children.Contains(_resultsGrid))
            {
                MainGrid.Children.Remove(_resultsGrid);
                _resultsGrid = null;
            }

            // Очищаем поле поиска
            SearchTextBox.Text = string.Empty;
        }

        private void CancelSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ExitSearchMode();
        }

        private void PerformSearch()
        {
            if (!_isSearchMode || _resultsGrid == null) return;

            string searchText = SearchTextBox.Text.Trim().ToLower();
            var selectedPage = (PageCategory.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(searchText))
            {
                _resultsGrid.ItemsSource = null;
                return;
            }

            try
            {
                var results = GetSearchResults(selectedPage, searchText);
                _resultsGrid.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<dynamic> GetSearchResults(string pageType, string searchText)
        {
            if (string.IsNullOrEmpty(pageType) || string.IsNullOrEmpty(searchText))
                return new List<dynamic>();

            switch (pageType)
            {
                case "Клиенты":
                    return Entities.GetContext().Customers
                        .Where(c => c.FullName.ToLower().Contains(searchText) ||
                                   c.Phone.Contains(searchText))
                        .Select(c => new { c.FullName, c.Phone })
                        .ToList<dynamic>();

                case "Машины":
                    return Entities.GetContext().Vehicles
                        .Where(v => v.Make.ToLower().Contains(searchText) ||
                                   v.Model.ToLower().Contains(searchText) ||
                                   v.VIN.Contains(searchText))
                        .Select(v => new { v.Make, v.Model, v.VIN, Owner = v.Customers.FullName })
                        .ToList<dynamic>();

                case "Сотрудники":
                    return Entities.GetContext().Employees
                        .Where(e => e.FullName.ToLower().Contains(searchText) ||
                                   e.Position.ToLower().Contains(searchText))
                        .Select(e => new { e.FullName, e.Position })
                        .ToList<dynamic>();

                case "Склады":
                    return Entities.GetContext().Inventory
                        .Where(i => i.InventoryName.ToLower().Contains(searchText) ||
                                   i.Adress.ToLower().Contains(searchText))
                        .Select(i => new { i.InventoryName, i.Adress })
                        .ToList<dynamic>();

                case "Запчасти":
                    return Entities.GetContext().Parts
                        .Where(p => p.PartName.ToLower().Contains(searchText))
                        .Select(p => new { p.PartName, p.Price, Owner = p.Inventory.Adress, p.Inventory.InventoryName })
                        .ToList<dynamic>();

                case "Услуги":
                    return Entities.GetContext().Services
                        .Where(s => s.ServiceName.ToLower().Contains(searchText) ||
                                   s.Description.ToLower().Contains(searchText))
                        .Select(s => new { s.ServiceName, s.Description, s.Price })
                        .ToList<dynamic>();

                default:
                    return new List<dynamic>();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearch();
        }

        private void PageFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSearchMode)
            {
                PerformSearch();
            }
        }

        private void ButtonOtchet_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Otchet());
        }

        private void ButtonOtchetVIN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Otchet2());
        }

        private void ButtonService_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicePage());
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderPage());
        }

        private void ButtonWork_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkPage());
        }

        private void ButtonPay_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PayPage());
        }

        private void ButtonOtzv_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FeedbackPage());
        }

        private void ButtonCustomers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomersPage());
        }

        private void ButtonCars_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CarPage());
        }

        private void ButtonEmployees_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeesPage());
        }

        private void ButtonInventory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InventoryPage());
        }

        private void ButtonPart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartsPage());
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        { 
            Application.Current.Shutdown();
        }
    }
}

