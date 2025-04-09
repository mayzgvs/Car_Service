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
    /// Логика взаимодействия для InventoryAddPage.xaml
    /// </summary>
    public partial class InventoryAddPage : Page
    {
        public InventoryAddPage(Inventory selectedInventory)
        {
            InitializeComponent();
            if (selectedInventory != null) _inventory = selectedInventory;
            DataContext = _inventory;
        }
        private Inventory _inventory = new Inventory();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_inventory.InventoryName)) errors.AppendLine("Введите название склада");
            if (string.IsNullOrWhiteSpace(_inventory.Adress)) errors.AppendLine("Введите адресс склада");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_inventory.InventoryID == 0) Entities.GetContext().Inventory.Add(_inventory);

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
