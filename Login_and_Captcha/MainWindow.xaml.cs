using System;
using System.Collections.Generic;
using System.IO;
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

namespace Login_and_Captcha
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pathAuto = @"auto.txt";
        //ссылка на автоматический вход по сохраненным данным 
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                loginText.Text = File.ReadLines("auto.txt").First();
                passwordText.Password = File.ReadLines("auto.txt").Skip(1).First();
                //автоматическое прописание данных из файла
            }
            catch { }
        }

        private void loginB_Click(object sender, RoutedEventArgs e)
        {
            passwordText.Password = passwordText2.Text;
            WorkBD f = new WorkBD();
            Captcha c = new Captcha();
            //указание класс работающий с бд, с капчей
            c.ShowDialog();
            //отркытие капчи при авторизации
            if (c.DialogResult == true)
            {
                if (f.userHave(loginText.Text, passwordText.Password))
                    //проверка логина и пароля
                {
                    Menu m = new Menu();
                    string title = "null";
                    switch (f.getPerm(loginText.Text))
                        //проверка роли польз
                    {
                        case "заказчик":
                            title = "Экран заказчика";
                            break;
                        case "менеджер":
                            title = "Экран менеджера";
                            break;
                        case "мастер":
                            title = "Экран мастера";
                            break;
                        case "заместитель директора":
                            title = "Экран заместителя директора";
                            break;
                        case "директор":
                            title = "Экран директора";
                            break;
                    }
                    m.Title = title;
                    m.Show();
                    using (FileStream fs = File.Create(pathAuto))
                        //автоматическое заполнение данных логина и пароля
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes($"{loginText.Text}\n{passwordText.Password}");
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Такого пользователя не существует или такого пароля");
                }
            }
            
           

        }

        private void RegB_Click(object sender, RoutedEventArgs e)
            //переход на рег
        {
            Reg reg = new Reg();
            reg.Show();
            this.Close();
        }



        private void passwordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordText2.Text = passwordText.Password;
        }


        private void passwordText2_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void checkPass_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordText.Password = passwordText2.Text;
            passwordText.Visibility = Visibility.Visible;
            passwordText2.Visibility = Visibility.Hidden;
            
        }

        private void checkPass_Checked(object sender, RoutedEventArgs e)
        {
            passwordText.Visibility = Visibility.Hidden;
            passwordText2.Visibility = Visibility.Visible;
        }
        //создание видимости пароля

    }
}
