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
    /// Логика взаимодействия для ServiceAddPage.xaml
    /// </summary>
    public partial class ServiceAddPage : Page
    {
        public ServiceAddPage(Services selectedService)
        {
            InitializeComponent();
            if (selectedService != null) _service = selectedService;
            DataContext = _service;
        }
        private Services _service = new Services();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_service.ServiceName)) errors.AppendLine("Введите название услуги");
            if (string.IsNullOrWhiteSpace(_service.Description)) errors.AppendLine("Введите описание услуги");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_service.ServiceID == 0) Entities.GetContext().Services.Add(_service);

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
