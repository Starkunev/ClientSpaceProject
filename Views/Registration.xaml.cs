using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class Registration : Window
    {
        private string _serverIp = "10.10.5.9";
        private int _port = 8000;

        // -------------------Свойства для возврата данных в форму входа----------
        public string RegisteredLogin => txtLogin.Text.Trim();
        public string RegisteredPassword => txtPassword.Password;

        public Registration()
        {
            InitializeComponent();
        }

        public Registration(string serverIp, int port) : this()
        {
            _serverIp = serverIp;
            _port = port;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
       
            if (!ValidateFields())
                return;

            btnRegister.IsEnabled = false;
            btnRegister.Content = "Подождите...";
            lblStatus.Text = "";

            try
            {
                using (var serverClient = new ClassForServerWork())
                {
                   
                    lblStatus.Text = "Подключение к серверу...";
                    bool connected = await serverClient.ConnectAsync(_serverIp, _port);

                    if (!connected)
                    {
                        lblStatus.Text = $"Не удалось подключиться к {_serverIp}:{_port}";
                        MessageBox.Show($"Не удалось подключиться к серверу {_serverIp}:{_port}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                   
                    lblStatus.Text = "Отправка данных...";
                    var result = await serverClient.RegisterAsync(
                        txtName.Text.Trim(),
                        txtLogin.Text.Trim(),
                        txtPassword.Password);
                    if (result.Success)
                    {
                        lblStatus.Text = "Регистрация успешна!";
                        MessageBox.Show($"Регистрация прошла успешно!\n{result.Message ?? "Добро пожаловать!"}",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        string errorMessage = result.Message ?? "Неизвестная ошибка";
                        lblStatus.Text = errorMessage;
                        MessageBox.Show($"Ошибка регистрации: {errorMessage}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        if (errorMessage.IndexOf("логин", StringComparison.OrdinalIgnoreCase) >= 0)
                            HighlightError(txtLogin);
                        else if (errorMessage.IndexOf("имя", StringComparison.OrdinalIgnoreCase) >= 0)
                            HighlightError(txtName);
                        else if (errorMessage.IndexOf("пароль", StringComparison.OrdinalIgnoreCase) >= 0)
                            HighlightError(txtPassword);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Произошла ошибка: {ex.Message}",
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnRegister.IsEnabled = true;
                btnRegister.Content = "Зарегистрироваться";
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || txtName.Text.Length < 3)
            {
                ShowError(txtName, "Имя должно содержать минимум 3 символа");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLogin.Text) || txtLogin.Text.Length < 3)
            {
                ShowError(txtLogin, "Логин должен содержать минимум 3 символа");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password) || txtPassword.Password.Length < 6)
            {
                ShowError(txtPassword, "Пароль должен содержать минимум 6 символов");
                return false;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                ShowError(txtConfirmPassword, "Пароли не совпадают");
                return false;
            }

            return true;
        }

        private void ShowError(Control control, string message)
        {
            lblStatus.Text = message;
            HighlightError(control);
            //HighlightError(control);
        }

        private void HighlightError(Control control)
        {
            control.Background = new SolidColorBrush(Colors.LightPink);

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, e) =>
            {
                control.Background = SystemColors.WindowBrush;
                timer.Stop();
            };
            timer.Start();
        }
    }
}