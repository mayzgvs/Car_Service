using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        public MainPage()
        {
            InitializeComponent();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
