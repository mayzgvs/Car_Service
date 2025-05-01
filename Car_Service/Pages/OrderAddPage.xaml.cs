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

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderAddPage.xaml
    /// </summary>
    public partial class OrderAddPage : Page
    {
        public OrderAddPage(Orders selectedOrder)
        {
            InitializeComponent();
            if (selectedOrder != null) _order = selectedOrder;
            DataContext = _order;

            fullName.ItemsSource = Entities.GetContext().Customers.ToList();
            CustCar.ItemsSource = Entities.GetContext().Vehicles.ToList();

            // Если редактируем существующую запчасть, устанавливаем выбранный склад
            if (selectedOrder != null)
            {
                fullName.SelectedValue = _order.CustomerID;
                CustCar.SelectedValue = _order.VehicleID;
            }
        }
        private Orders _order = new Orders();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (fullName.SelectedItem == null) errors.AppendLine("Выберите клиента");
            if (CustCar.SelectedItem == null) errors.AppendLine("Выберите транспортное средство");
            if (string.IsNullOrWhiteSpace(_order.Status)) errors.AppendLine("Введите статус заказа");
            if (string.IsNullOrWhiteSpace(_order.Problem)) errors.AppendLine("Опишите проблему");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_order.OrderID == 0) Entities.GetContext().Orders.Add(_order);

            if (fullName.SelectedItem is Customers selectedCustomer)
            {
                _order.CustomerID = selectedCustomer.CustomerID;
            }

            if (CustCar.SelectedItem is Vehicles selectedVehicles)
            {
                _order.VehicleID = selectedVehicles.VehicleID;
            }

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
