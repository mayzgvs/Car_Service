using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для CarAddPage.xaml
    /// </summary>
    public partial class CarAddPage : Page
    {
        public CarAddPage(Vehicles selectedVehicles/*, Customers selectedCustomers*/)
        {
            InitializeComponent();
            if (selectedVehicles != null) _vehicles = selectedVehicles;
            DataContext = _vehicles;

            //if (selectedCustomers != null) _customers = selectedCustomers;
            //DataContext = _customers;
        }

        private Vehicles _vehicles = new Vehicles();
        //private Customers _customers = new Customers();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new CarAddPage(null));
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_vehicles.Make)) errors.AppendLine("Введите марку транспортного средства");
            if (string.IsNullOrWhiteSpace(_vehicles.Model)) errors.AppendLine("Введите модель транспортного средства");
            if (string.IsNullOrWhiteSpace(_vehicles.VIN)) errors.AppendLine("Введите VIN номер транспортного средства");
            else if (!Regex.IsMatch(_vehicles.VIN, @"^[A-HJ-NPR-Z0-9]{17}$")) errors.AppendLine("Стандартный VIN состоит из 17 символов (цифры и латинские буквы, за исключением I, O и Q");
            //if (string.IsNullOrWhiteSpace(_customers.FirstName)) errors.AppendLine("Введите имя клиента");
            //if (string.IsNullOrWhiteSpace(_customers.LastName)) errors.AppendLine("Введите фамилию клиента");

            if (errors.Length > 0) {MessageBox.Show(errors.ToString()); return; }
            if (_vehicles.VehicleID == 0) Entities.GetContext().Vehicles.Add(_vehicles);
            //if (_customers.CustomerID == 0) Entities.GetContext().Customers.Add(_customers);

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
