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
    /// Логика взаимодействия для FeedbackAddPage.xaml
    /// </summary>
    public partial class FeedbackAddPage : Page
    {
        public FeedbackAddPage(Feedback selectedFeedback)
        {
            InitializeComponent();
            if (selectedFeedback != null) _feedback = selectedFeedback;
            DataContext = _feedback;

            fName.ItemsSource = Entities.GetContext().Customers.ToList();
        }
        private Feedback _feedback = new Feedback();

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (fName.SelectedItem == null) errors.AppendLine("Выберите клиента");
            if (string.IsNullOrWhiteSpace(_feedback.Feedback1)) errors.AppendLine("Напишите отзыв");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }
            if (_feedback.FeedbackID == 0) Entities.GetContext().Feedback.Add(_feedback);

            if (fName.SelectedItem is Customers selectedCustomer)
            {
                _feedback.CustomerID = selectedCustomer.CustomerID;
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
