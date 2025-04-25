using Car_Service;
using Car_Service.Pages;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Car_service.Pages
{
    public partial class PartsAddPage : Page
    {
        private Parts _currentPart;

        public PartsAddPage(Parts selectedPart = null)
        {
            InitializeComponent();

            // Инициализация _currentPart
            _currentPart = selectedPart ?? new Parts();
            DataContext = _currentPart;

            // Загрузка данных для ComboBox
            LoadInventoryData();

            // Если редактируем существующую запчасть, устанавливаем выбранный склад
            if (selectedPart != null)
            {
                cbInventory.SelectedValue = _currentPart.InventoryID;
            }

            // Если редактируем существующую запчасть, загружаем изображение
            if (!string.IsNullOrEmpty(_currentPart.Picture))
            {
                LoadImage(_currentPart.Picture);
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                cbInventory.ItemsSource = Entities.GetContext().Inventory.ToList();
                cbInventory.DisplayMemberPath = "InventoryName";
                cbInventory.SelectedValuePath = "InventoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных склада: {ex.Message}");
            }
        }

        private void LoadImage(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PartImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                PartImage.Source = null;
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на null
            if (_currentPart == null)
            {
                MessageBox.Show("Ошибка: объект запчасти не инициализирован");
                return;
            }

            // Валидация данных
            if (string.IsNullOrWhiteSpace(_currentPart.PartName))
            {
                MessageBox.Show("Введите наименование запчасти!");
                return;
            }

            try
            {
                // Сохранение данных
                if (_currentPart.PartID == 0) // Новая запчасть
                {
                    Entities.GetContext().Parts.Add(_currentPart);
                }

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");

                // Возврат на предыдущую страницу
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Сохраняем только имя файла (или полный путь, в зависимости от вашей логики)
                    _currentPart.Picture = openFileDialog.FileName;
                    LoadImage(_currentPart.Picture);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }
    }
}