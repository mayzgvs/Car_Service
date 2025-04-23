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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private string _searchText;
        public EmployeesPage(string searchText = "")
        {
            InitializeComponent();
            DGridEmployee.ItemsSource = Entities.GetContext().Employees.ToList();

            _searchText = searchText;
            LoadData();
        }

        private void LoadData()
        {
            var query = Entities.GetContext().Employees.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.FullName.ToLower().Contains(_searchText) ||
                    c.Position.ToLower().Contains(_searchText));
            }

            DGridEmployee.ItemsSource = query.ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var employeeForRemoving = DGridEmployee.SelectedItems.Cast<Employees>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {employeeForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Employees.RemoveRange(employeeForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridEmployee.ItemsSource = Entities.GetContext().Employees.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            if (DGridEmployee.SelectedItem != null)
            {
                NavigationService.Navigate(new EmployeeEditPage(DGridEmployee.SelectedItem as Employees));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
