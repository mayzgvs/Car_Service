using Car_Service;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Car_service.Pages
{
    public partial class PartsAddPage : Page
    {
        private Parts _currentPart;

        public PartsAddPage(Parts selectedPart = null)
        {
            InitializeComponent();
            cbInventory.ItemsSource = Entities.GetContext().Inventory.ToList();
            _currentPart = selectedPart ?? new Parts();
            DataContext = _currentPart;
            

            // Если редактируем существующую запчасть, устанавливаем выбранный склад
            if (selectedPart != null)
            {
                cbInventory.SelectedValue = _currentPart.InventoryID;
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
                    
                    _currentPart.InventoryID = Convert.ToInt32(cbInventory.SelectedValue);
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
                    // Сохраняем только имя файла (или полный путь)
                    _currentPart.Picture = openFileDialog.FileName;
                    DataContext = null;
                    DataContext = _currentPart;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }
    }
}
