using Microsoft.Win32;
using SpaceTcpChat.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class ProfileWindow : Window
    {
        private UserModel user;
        private ChatClient _chatClient;
        private string _userLogin;
        private string _userPassword;

        public ProfileWindow(UserModel u)
        {
            InitializeComponent();
            user = u;
            NameBox.Text = u.Username;
            StatusBox.Text = u.Status;
            AboutBox.Text = u.About;
            IpText.Text = u.IpAddress;
            SetThemeComboBox(u.Theme);
            LoadAvatarSafe();
        }

        public ProfileWindow(UserModel u, ChatClient chatClient, string login, string password = null) : this(u)
        {
            _chatClient = chatClient;
            _userLogin = login;
            _userPassword = password ?? "";

            System.Diagnostics.Debug.WriteLine($"=== ProfileWindow создан ===");
            System.Diagnostics.Debug.WriteLine($"Login: {_userLogin}");
            System.Diagnostics.Debug.WriteLine($"Password: [{(string.IsNullOrEmpty(_userPassword) ? "ПУСТОЙ" : "ЕСТЬ")}]");
        }

        private void SetThemeComboBox(string theme)
        {
            try
            {
                for (int i = 0; i < ThemeBox.Items.Count; i++)
                {
                    if (ThemeBox.Items[i] is System.Windows.Controls.ComboBoxItem item)
                    {
                        if (item.Content?.ToString() == theme)
                        {
                            ThemeBox.SelectedIndex = i;
                            return;
                        }
                    }
                }
                ThemeBox.SelectedIndex = 0;
            }
            catch
            {
                ThemeBox.SelectedIndex = 0;
            }
        }

        private void LoadAvatarSafe()
        {
            try
            {
                if (user.AvatarBytes != null && user.AvatarBytes.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream(user.AvatarBytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        AvatarImage.Source = bitmap;
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(user.AvatarPath) && System.IO.File.Exists(user.AvatarPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(user.AvatarPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    AvatarImage.Source = bitmap;
                    return;
                }

                try
                {
                    var uri = new Uri("pack://application:,,,/Images/default_avatar.png", UriKind.Absolute);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    AvatarImage.Source = bitmap;
                    return;
                }
                catch { }

                SetDefaultAvatar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки аватара: {ex.Message}");
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Images/default_avatar.png", UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                AvatarImage.Source = bitmap;
            }
            catch
            {
                AvatarImage.Source = null;
            }
        }


        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                ChangePasswordButton.IsEnabled = false;
                ChangePasswordButton.Content = "Ожидание...";

                string newPassword = NewPasswordBox.Password;
                string confirmPassword = ConfirmPasswordBox.Password;

               
                if (string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Введите новый пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPasswordBox.Focus();
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPasswordBox.Focus();
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    ConfirmPasswordBox.Focus();
                    return;
                }

              
                if (_chatClient == null || !_chatClient.IsConnected())
                {
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

               
                var result = await _chatClient.ChangePasswordAsync(newPassword);

                if (result.Success)
                {
                    MessageBox.Show("Пароль успешно изменен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                   
                    NewPasswordBox.Password = "";
                    ConfirmPasswordBox.Password = "";

                   
                    _userPassword = newPassword;
                }
                else
                {
                    MessageBox.Show($"Ошибка смены пароля: {result.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
               
                ChangePasswordButton.IsEnabled = true;
                ChangePasswordButton.Content = "СМЕНИТЬ ПАРОЛЬ";
            }
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                    Title = "Выберите аватар"
                };

                if (dlg.ShowDialog() == true)
                {
                    if (System.IO.File.Exists(dlg.FileName))
                    {
                        user.AvatarPath = dlg.FileName;

                        var bytes = System.IO.File.ReadAllBytes(dlg.FileName);
                        if (bytes.Length > 8 * 1024 * 1024)
                        {
                            MessageBox.Show("Аватар слишком большой (макс. 8MB)", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        user.AvatarBytes = bytes;

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        AvatarImage.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки аватара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedTheme()
        {
            try
            {
                if (ThemeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
                    return item.Content?.ToString() ?? "Dark";
            }
            catch { }
            return "Dark";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                user.Username = NameBox.Text?.Trim() ?? "User";
                user.Status = StatusBox.Text?.Trim() ?? "Online";
                user.About = AboutBox.Text?.Trim() ?? "";
                user.Theme = GetSelectedTheme();
                user.LastSeen = DateTime.Now;
                user.IsOnline = true;

                if (!string.IsNullOrEmpty(user.AvatarPath) && File.Exists(user.AvatarPath))
                {
                    user.AvatarBytes = File.ReadAllBytes(user.AvatarPath);
                }

                System.Diagnostics.Debug.WriteLine("=== Сохранение профиля ===");
                System.Diagnostics.Debug.WriteLine($"Username: {user.Username}");
                System.Diagnostics.Debug.WriteLine($"Login: {_userLogin}");
                System.Diagnostics.Debug.WriteLine($"Password: [{(string.IsNullOrEmpty(_userPassword) ? "ПУСТОЙ" : "ЕСТЬ")}]");
                System.Diagnostics.Debug.WriteLine($"Avatar null: {user.AvatarBytes == null}");
                System.Diagnostics.Debug.WriteLine($"Avatar length: {user.AvatarBytes?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"ChatClient null: {_chatClient == null}");
                System.Diagnostics.Debug.WriteLine($"IsConnected: {_chatClient?.IsConnected()}");

                if (_chatClient != null && _chatClient.IsConnected())
                {
                    var updateRequest = new UpdateClientRequest
                    {
                        Name = user.Username,
                        Login = _userLogin,
                        PasswordHash = _userPassword ?? " ",
                        Avatar = user.AvatarBytes
                    };

                    System.Diagnostics.Debug.WriteLine($"UpdateRequest создан:");
                    System.Diagnostics.Debug.WriteLine($"  Name: {updateRequest.Name}");
                    System.Diagnostics.Debug.WriteLine($"  Login: {updateRequest.Login}");
                    System.Diagnostics.Debug.WriteLine($"  Password: [{(string.IsNullOrEmpty(updateRequest.PasswordHash) ? "ПУСТОЙ" : "ЕСТЬ")}]");
                    System.Diagnostics.Debug.WriteLine($"  Avatar null: {updateRequest.Avatar == null}");
                    System.Diagnostics.Debug.WriteLine($"  Avatar length: {updateRequest.Avatar?.Length ?? 0}");

                    if (updateRequest.Avatar != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Avatar first bytes: {BitConverter.ToString(updateRequest.Avatar.Take(10).ToArray())}");
                    }

                    await _chatClient.UpdateClientAsync(updateRequest);
                    System.Diagnostics.Debug.WriteLine("UpdateClientAsync вызван успешно");
                }
                else
                {
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var main = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                if (main != null)
                {
                    main.SyncUser(user);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex}");
                MessageBox.Show($"Ошибка сохранения профиля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}