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
    /// Логика взаимодействия для InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        private string _searchText;
        public InventoryPage(string searchText = "")
        {
            InitializeComponent();
            DGridInventory.ItemsSource = Entities.GetContext().Inventory.ToList();

            _searchText = searchText;
            LoadData();
        }

        private void LoadData()
        {
            var query = Entities.GetContext().Inventory.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.InventoryName.ToLower().Contains(_searchText) ||
                    c.Adress.ToLower().Contains(_searchText));
            }

            DGridInventory.ItemsSource = query.ToList();
        }

        private void InventoryPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGridInventory.ItemsSource = Entities.GetContext().Inventory.ToList();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InventoryAddPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var inventoryForRemoving = DGridInventory.SelectedItems.Cast<Inventory>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {inventoryForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Inventory.RemoveRange(inventoryForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridInventory.ItemsSource = Entities.GetContext().Inventory.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            if (DGridInventory.SelectedItem != null)
            {
                NavigationService.Navigate(new InventoryAddPage(DGridInventory.SelectedItem as Inventory));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
