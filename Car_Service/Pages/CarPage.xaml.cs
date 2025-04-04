using System;
using System.Collections.Generic;
using System.Dynamic;
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

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для CarPage.xaml
    /// </summary>
    public partial class CarPage : Page
    {
        public CarPage()
        {
            InitializeComponent();
            DGridCar.ItemsSource = Entities.GetContext().Vehicles.ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CarAddPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CarAddPage((sender as Button).DataContext as Vehicles));
        }
    }
}
