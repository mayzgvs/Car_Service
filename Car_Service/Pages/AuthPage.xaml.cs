using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
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

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbLogin.Text) || string.IsNullOrEmpty(psBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string _password = GetHash(psBox.Password);
            using (var db = new Entities())
            {
                var employer = db.Employees.AsNoTracking().FirstOrDefault(x => x.Login == tbLogin.Text && x.Password == _password);
                if (employer == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    return;
                }
                else
                {
                    NavigationService.Navigate(new MainPage());
                }
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
    }
}
