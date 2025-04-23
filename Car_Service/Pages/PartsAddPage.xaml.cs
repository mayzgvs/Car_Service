using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Car_Service.Pages
{
    public partial class PartsAddPage : Page
    {
        public PartsAddPage(Parts selectedParts)
        {
            InitializeComponent();
            if (selectedParts != null) _parts = selectedParts;
            DataContext = _parts;

            cbInventory.ItemsSource = Entities.GetContext().Inventory.ToList();
        }
        private Parts _parts = new Parts();

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Создаем и устанавливаем изображение
                    var image = new Image();
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    image.Source = bitmap;

                    // Сохраняем путь к изображению в модели
                    _parts.Picture = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_parts.PartName))
                errors.AppendLine("Укажите наименование запчасти");

            if (_parts.Price <= 0)
                errors.AppendLine("Укажите стоимость");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                if (_parts.PartID == 0)
                    Entities.GetContext().Parts.Add(_parts);

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}