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
    /// Логика взаимодействия для PayPage.xaml
    /// </summary>
    public partial class PayPage : Page
    {
        public PayPage()
        {
            InitializeComponent();
            DGridPayments.ItemsSource = Entities.GetContext().Payments.ToList();
        }
        private void PayPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGridPayments.ItemsSource = Entities.GetContext().Payments.ToList();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PayAddPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var payForRemoving = DGridPayments.SelectedItems.Cast<Payments>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {payForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Payments.RemoveRange(payForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridPayments.ItemsSource = Entities.GetContext().Payments.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PayAddPage((sender as Button).DataContext as Payments));
        }
    }
}
