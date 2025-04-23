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
    /// Логика взаимодействия для FeedbackPage.xaml
    /// </summary>
    public partial class FeedbackPage : Page
    {
        public FeedbackPage()
        {
            InitializeComponent();
            DGridFeedback.ItemsSource = Entities.GetContext().Feedback.ToList();
        }
        private void FeedbackPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGridFeedback.ItemsSource = Entities.GetContext().Feedback.ToList();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FeedbackAddPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var feedbackForRemoving = DGridFeedback.SelectedItems.Cast<Feedback>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {feedbackForRemoving.Count()} элементов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Feedback.RemoveRange(feedbackForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DGridFeedback.ItemsSource = Entities.GetContext().Feedback.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            if (DGridFeedback.SelectedItem != null)
            {
                NavigationService.Navigate(new FeedbackAddPage(DGridFeedback.SelectedItem as Feedback));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
