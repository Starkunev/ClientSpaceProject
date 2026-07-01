using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.Views
{
    public partial class Autarisation : Window
    {
        private string _serverIp = "10.10.5.1";
        private int _port = 8000;
        private readonly ChatClient _serverClient = new ChatClient();

        public Autarisation()
        {
            InitializeComponent();
            Loaded += Autarisation_Loaded;
        }

        private void Autarisation_Loaded(object sender, RoutedEventArgs e)
        {
            txtServerIp.Text = _serverIp;
            txtPort.Text = _port.ToString();
            txtLogin.Focus();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            _serverIp = txtServerIp.Text.Trim();
            int.TryParse(txtPort.Text, out _port);

            var registrationForm = new Registration(_serverIp, _port);
            if (registrationForm.ShowDialog() == true)
            {
                txtLogin.Text = registrationForm.RegisteredLogin;
                txtPassword.Password = registrationForm.RegisteredPassword;

                if (MessageBox.Show("Регистрация прошла успешно! Выполнить вход?",
                    "Успех", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    BtnLogin_Click(sender, e);
                }
            }
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            _serverIp = txtServerIp.Text.Trim();
            if (!int.TryParse(txtPort.Text, out _port))
            {
                ShowError("Неверный номер порта");
                return;
            }

            btnLogin.IsEnabled = false;
            btnLogin.Content = "Вход...";
            lblStatus.Text = "";

            try
            {
                var serverClient = new ChatClient();
                {
                    lblStatus.Text = "Подключение к серверу...";
                    bool connected = await _serverClient.ConnectAsync(_serverIp, _port);

                    if (!connected)
                    {
                        ShowError($"Не удалось подключиться к {_serverIp}:{_port}");
                        return;
                    }

                    lblStatus.Text = "Авторизация...";
                    var result = await _serverClient.LoginAsync(txtLogin.Text.Trim(), txtPassword.Password);

                    if (result.Success)
                    {
                        lblStatus.Text = $"Добро пожаловать, {result.Name}!";
                        lblStatus.Foreground = new SolidColorBrush(Colors.Green);

                        MessageBox.Show($"Добро пожаловать в чат, {result.Name}!",
                            "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                      
                        var chatWindow = new MainWindow(_serverClient,result.Id,result.Name,txtLogin.Text.Trim(),txtPassword.Password);
                        chatWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        string errorMessage = result?.Message ?? "Неизвестная ошибка";
                        ShowError(errorMessage);

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            if (errorMessage.IndexOf("логин", StringComparison.OrdinalIgnoreCase) >= 0)
                                HighlightError(txtLogin);
                            else if (errorMessage.IndexOf("пароль", StringComparison.OrdinalIgnoreCase) >= 0)
                                HighlightError(txtPassword);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
                MessageBox.Show(ex.Message, "Критическая ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
                btnLogin.Content = "Вход";
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                ShowError("Введите логин");
                HighlightError(txtLogin);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowError("Введите пароль");
                HighlightError(txtPassword);
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message ?? "Неизвестная ошибка";
            lblStatus.Foreground = new SolidColorBrush(Colors.Red);
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