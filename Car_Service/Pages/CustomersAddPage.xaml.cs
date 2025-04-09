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
    /// Логика взаимодействия для CustomersAddPage.xaml
    /// </summary>
    public partial class CustomersAddPage : Page
    {
        public CustomersAddPage(Customers selectedCustomer)
        {
            InitializeComponent();
            if (selectedCustomer != null) _customers = selectedCustomer;
            DataContext = _customers;
        }
        private Customers _customers = new Customers();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_customers.FullName)) errors.AppendLine("Введите ФИО клиента");
            if (string.IsNullOrWhiteSpace(_customers.Phone)) errors.AppendLine("Введите номер телефона");
            if (string.IsNullOrWhiteSpace(_customers.Email)) errors.AppendLine("Введите Email");
            if (string.IsNullOrWhiteSpace(_customers.Adress)) errors.AppendLine("Введите адресс");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_customers.CustomerID == 0) Entities.GetContext().Customers.Add(_customers);

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
