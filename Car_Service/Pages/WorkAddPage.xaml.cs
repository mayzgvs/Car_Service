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
    /// Логика взаимодействия для WorkAddPage.xaml
    /// </summary>
    public partial class WorkAddPage : Page
    {
        public WorkAddPage(Work selectedWork)
        {
            InitializeComponent();
            if (selectedWork != null) _work = selectedWork;

            NumOrder.ItemsSource = Entities.GetContext().Orders.ToList();
            Position.ItemsSource = Entities.GetContext().Employees.ToList();
            sName.ItemsSource = Entities.GetContext().Services.ToList();
            pName.ItemsSource = Entities.GetContext().Parts.ToList();
        }

        private Work _work = new Work();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (NumOrder.SelectedItem == null) errors.AppendLine("Выберите проблему");
            if (Position.SelectedItem == null) errors.AppendLine("Выберите сотрудника");
            if (sName.SelectedItem == null) errors.AppendLine("Выберите услугу");
            if (pName.SelectedItem == null) errors.AppendLine("Выберите запчасть");

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
