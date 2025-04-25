using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Car_service.Pages;

namespace Car_Service.Pages
{
    public partial class PartsPage : Page
    {
        private string _searchText;

        public PartsPage(string searchText = "")
        {
            InitializeComponent();
            _searchText = searchText;
            LoadData();

        }

        public void LoadData()
        {
            var query = Entities.GetContext().Parts.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.PartName.ToLower().Contains(_searchText) ||
                    c.Inventory.InventoryName.ToLower().Contains(_searchText));
            }

            LViewParts.ItemsSource = query.ToList();
        }

        private void PartsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                LoadData();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartsAddPage(null));
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            if (LViewParts.SelectedItem != null)
            {
                NavigationService.Navigate(new PartsAddPage(LViewParts.SelectedItem as Parts));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запчасть для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var partsForRemoving = LViewParts.SelectedItems.Cast<Parts>().ToList();

            if (partsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите записи для удаления!", "Внимание!",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {partsForRemoving.Count} записей?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Parts.RemoveRange(partsForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Записи успешно удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}