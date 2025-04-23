using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeEditPage.xaml
    /// </summary>
    public partial class EmployeeEditPage : Page
    {
        public EmployeeEditPage(Employees selectedEmployees)
        {
            InitializeComponent();
            if (selectedEmployees != null) _employees = selectedEmployees;
            DataContext = _employees;
        }

        private Employees _employees = new Employees();
        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_employees.FullName)) errors.AppendLine("Введите ФИО сотрудника");
            if (string.IsNullOrWhiteSpace(_employees.Position)) errors.AppendLine("Введите должность сотрудника");
            if (string.IsNullOrWhiteSpace(_employees.Email)) errors.AppendLine("Введите Email");
            if (string.IsNullOrWhiteSpace(_employees.Phone)) errors.AppendLine("Введите номер телефона");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }


            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
