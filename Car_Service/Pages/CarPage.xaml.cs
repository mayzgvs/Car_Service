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
        private string _searchText;
        public CarPage(string searchText = "")
        {
            InitializeComponent();
            DGridCar.ItemsSource = Entities.GetContext().Vehicles.ToList();

            _searchText = searchText;
            LoadData();
        }

        private void LoadData()
        {
            var query = Entities.GetContext().Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.Make.ToLower().Contains(_searchText) ||
                    c.Model.ToLower().Contains(_searchText) ||
                    c.VIN.ToLower().Contains(_searchText) ||
                    c.Customers.FullName.ToLower().Contains(_searchText));
            }

            DGridCar.ItemsSource = query.ToList();
        }

        private void CarPage_IsVisibleChanged (object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGridCar.ItemsSource = Entities.GetContext().Vehicles.ToList();
            }
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
            var carForRemoving = DGridCar.SelectedItems.Cast<Vehicles>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {carForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Vehicles.RemoveRange(carForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridCar.ItemsSource = Entities.GetContext().Vehicles.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            if (DGridCar.SelectedItem != null)
            {
                NavigationService.Navigate(new CarAddPage(DGridCar.SelectedItem as Vehicles));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
