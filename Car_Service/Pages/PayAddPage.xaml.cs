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
using System.Xml.Linq;

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для PayAddPage.xaml
    /// </summary>
    public partial class PayAddPage : Page
    {
        public PayAddPage(Payments selectedPay)
        {
            InitializeComponent();
            if (selectedPay != null) _pay = selectedPay;
            DataContext = _pay;

            fName.ItemsSource = Entities.GetContext().Customers.ToList();
            NumOrder.ItemsSource = Entities.GetContext().Orders.ToList();
        }
        private Payments _pay = new Payments();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (NumOrder.SelectedItem == null) errors.AppendLine("Выберите номер заказа");
            if (fName.SelectedItem == null) errors.AppendLine("Выберите клиента");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_pay.PaymentID == 0) Entities.GetContext().Payments.Add(_pay);

            if (fName.SelectedItem is Customers selectedCustomer)
            {
                _pay.CustomerID = selectedCustomer.CustomerID;
            }
            if (NumOrder.SelectedItem is Orders selectedOrder)
            {
                _pay.OrderID = selectedOrder.OrderID;
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
