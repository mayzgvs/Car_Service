using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Policy;
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

namespace Car_Service.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }


        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbLogin.Text.Length > 0)
            {
                using (var db = new Entities())
                {
                    var user = db.Employees.AsNoTracking().FirstOrDefault(u => u.Login == tbLogin.Text);
                    if (user != null)
                    {
                        MessageBox.Show("Пользователь стакими данными уже существует!");
                        return;
                    }
                }
                bool en = true;
                bool number = false;
                for (int i = 0; i < psBox.Password.Length; i++)
                {
                    if (psBox.Password[i] >= 'А' && psBox.Password[i] <= 'Я') en = false;
                    if (psBox.Password[i] >= '0' && psBox.Password[i] <= '9') number = true;
                }
                var regex = new Regex(@"^((\+7))\d{10}$");
                var regex1 = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                StringBuilder errors = new StringBuilder();

                if (psBox.Password.Length < 6) errors.AppendLine("Пароль должен быть больше 6 символов!");
                if (!regex.IsMatch(phoneText.Text)) errors.AppendLine("Укажите номер телефона в формате +7ХХХХХХХХХХ");
                if (!en) errors.AppendLine("Пароль должен быть на английском языке");
                if (!number) errors.AppendLine("Пароль должен содержать хотя бы одну цифру");
                if (!regex1.IsMatch(emailText.Text)) errors.AppendLine("Введите корректный email");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                else
                {
                    Entities db = new Entities();
                    Employees employerObject = new Employees();
                    {
                        employerObject.FullName = nameText.Text;
                        employerObject.Position = posText.Text;
                        employerObject.Phone = phoneText.Text;
                        employerObject.Email = emailText.Text;
                        employerObject.Login = tbLogin.Text;
                        employerObject.Password = GetHash(psBox.Password);
                    }
                    ;
                    db.Employees.Add(employerObject);
                    db.SaveChanges();
                    MessageBox.Show("Вы успешно зарегистрировались!", "Успешно!", MessageBoxButton.OK);
                    NavigationService.GoBack();
                }
            }
            else MessageBox.Show("Укажите логин!");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
