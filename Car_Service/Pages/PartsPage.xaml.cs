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
    /// Логика взаимодействия для PartsPage.xaml
    /// </summary>
    public partial class PartsPage : Page
    {
        private string _searchText;
        public PartsPage(string searchText = "")
        {
            InitializeComponent();
            LViewParts.ItemsSource = Entities.GetContext().Parts.ToList();

            _searchText = searchText;
            LoadData();
        }
        private void LoadData()
        {
            var query = Entities.GetContext().Parts.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.PartName.ToLower().Contains(_searchText));
                    //c.Price.ToLower().Contains(_searchText));
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartsAddPage(null));
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            var selectedParts = (sender as Button).DataContext as Parts;
            NavigationService.Navigate(new PartsAddPage(selectedParts));
        }
    }
}
