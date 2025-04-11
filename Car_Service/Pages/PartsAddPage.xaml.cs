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
    /// Логика взаимодействия для PartsAddPage.xaml
    /// </summary>
    public partial class PartsAddPage : Page
    {
        public PartsAddPage(Parts selectedParts)
        {
            InitializeComponent();
            if (selectedParts != null) _parts = selectedParts;
            DataContext = _parts;
        }
        private Parts _parts = new Parts();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            //if (string.IsNullOrWhiteSpace(_parts.Status)) errors.AppendLine("Введите статус заказа");
            //if (string.IsNullOrWhiteSpace(_parts.Problem)) errors.AppendLine("Опишите проблему");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_parts.PartID == 0) Entities.GetContext().Parts.Add(_parts);

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
