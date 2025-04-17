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

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : Page
    {
        private string _searchText;
        public CustomersPage(string searchText = "")
        {
            InitializeComponent();
            DGridCustomer.ItemsSource = Entities.GetContext().Customers.ToList();

            _searchText = searchText;
            LoadData();

        }
        private void LoadData()
        {
            var query = Entities.GetContext().Customers.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.FullName.ToLower().Contains(_searchText) ||
                    c.Phone.ToLower().Contains(_searchText));
            }

            DGridCustomer.ItemsSource = query.ToList();
        }

        private void CustomersPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGridCustomer.ItemsSource = Entities.GetContext().Customers.ToList();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomersAddPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var customerForRemoving = DGridCustomer.SelectedItems.Cast<Customers>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {customerForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Customers.RemoveRange(customerForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridCustomer.ItemsSource = Entities.GetContext().Customers.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomersAddPage((sender as Button).DataContext as Customers));
        }
    }
}
