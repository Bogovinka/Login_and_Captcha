using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;

namespace Login_and_Captcha
{
    /// <summary>
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Window
    {
        Regex hasNumber = new Regex(@"[0-9]+");
        Regex sym3 = new Regex(@"(.)\1{2}");
        Regex symbol = new Regex(@"[*&{}|+]");
        Regex hasUpperChar = new Regex(@"[A-ZА-Я]+");
        Regex hasUpperCharMini = new Regex(@"[a-zа-я]+");
        Regex hasMinimum8Chars = new Regex(@".{8,}");

        string pathAuto = @"auto.txt";
        public Reg()
        {
            InitializeComponent();
        }
        private void nextB_Click(object sender, RoutedEventArgs e)
        {
            passwordText.Password = passwordText3.Text;
            if (permis.Text != "")
            {
                if (loginText.Text.Replace(" ", "").Length >= 3 )
                {
                    if (passwordText.Password.Length >= 8 && passwordText.Password.Length <= 16)
                    {
                        if (symbol.IsMatch(passwordText.Password))
                        {
                            if (!sym3.IsMatch(passwordText.Password))
                            {
                                if (passwordText.Password == passwordText2.Password)
                                {
                                    string pass = passwordText.Password;
                                    int hardPass = 0;
                                    if (hasNumber.IsMatch(pass)) hardPass++;
                                    if (hasUpperChar.IsMatch(pass)) hardPass++;
                                    if (hasUpperCharMini.IsMatch(pass)) hardPass++;
                                    if (hasMinimum8Chars.IsMatch(pass)) hardPass++;
                                    WorkBD test = new WorkBD();
                                    if (hardPass > 1)
                                    {
                                        if (!test.userHave(loginText.Text))
                                        {
                                            using (FileStream fs = File.Create(pathAuto))
                                            {
                                                byte[] info = new UTF8Encoding(true).GetBytes($"{loginText.Text}\n{passwordText.Password}");
                                                // Add some information to the file.
                                                fs.Write(info, 0, info.Length);
                                            }

                                            // строка подключения к базе данных
                                            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DatabaseLogins.mdf;Integrated Security=True";
                                            SqlConnection connection = new SqlConnection(connectionString);
                                            connection.Open();
                                            string perm = permis.Text;
                                            string query = $"INSERT INTO Logins VALUES (N'{loginText.Text.ToString()}', N'{passwordText.Password.ToString()}', N'{perm}')";
                                            SqlCommand command = new SqlCommand(query, connection);
                                            // выполняем запрос
                                            command.ExecuteNonQuery();
                                            MainWindow m = new MainWindow();
                                            m.Show();
                                            connection.Close();

                                            this.Close();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Такой логин уже есть");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Слишком легкий пароль");
                                    }


                                }
                                else
                                {
                                    MessageBox.Show("Пароли не совпадают");
                                }
                            } else MessageBox.Show("В пароле не должны быть 3 подряд одинаковых символа");
                        } else MessageBox.Show("В пароле должен быть 1 символ из * & { } | +");
                    } else MessageBox.Show("Пароль должен быть от 6 символов и не больше 16");

                }
                else
                {
                    MessageBox.Show("Логин должен быть минимум 3 символа");
                }
            } else MessageBox.Show("Выбери роль");

        }



        private void RegB_Click(object sender, RoutedEventArgs e)
        //вернуться назад

        {
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        private void passwordCheck()
        //проверка сложности пароля

        {
            string pass = passwordText.Password;
            int hardPass = 0;
            if (hasNumber.IsMatch(pass) && pass.Length >= 4) hardPass++;
            if (hasUpperChar.IsMatch(pass) && pass.Length >= 4) hardPass++;
            if (hasUpperCharMini.IsMatch(pass) && pass.Length >= 4) hardPass++;
            if (hasMinimum8Chars.IsMatch(pass) && pass.Length >= 4) hardPass++;
            if (hardPass == 0) { prov.Background = new SolidColorBrush(Colors.Gray); prov.Content = "Не подходит"; }
            else if (hardPass == 1) { prov.Background = new SolidColorBrush(Colors.Red); prov.Content = "Плохой"; }
            else if (hardPass == 2) { prov.Background = new SolidColorBrush(Colors.Orange); prov.Content = "Нормальный"; }
            else if (hardPass == 3) { prov.Background = new SolidColorBrush(Colors.Yellow); prov.Content = "Хороший"; }
            else if (hardPass == 4) { prov.Background = new SolidColorBrush(Colors.LightGreen); prov.Content = "Отличный"; }
        }
        private void passwordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordText3.Text = passwordText.Password;
            passwordCheck();
            
        }

        private void passwordText2_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            passwordCheck();
        }

        private void checkPass_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordText.Password = passwordText3.Text;
            passwordText.Visibility = Visibility.Visible;
            passwordText3.Visibility = Visibility.Hidden;
        }

        private void checkPass_Checked(object sender, RoutedEventArgs e)
        {
            passwordText.Visibility = Visibility.Hidden;
            passwordText3.Visibility = Visibility.Visible;
        }
        

    }
}
