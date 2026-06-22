//////using Microsoft.Win32;
//////using SpaceTcpChat.Models;
//////using System;
//////using System.Linq;
//////using System.Windows;
//////using System.Windows.Media.Imaging;

//////namespace SpaceTcpChat.Windows
//////{
//////    public partial class ProfileWindow : Window
//////    {
//////        private UserModel user;

//////        public ProfileWindow(UserModel u)
//////        {
//////            InitializeComponent();

//////            user = u;

//////            NameBox.Text = u.Username;
//////            StatusBox.Text = u.Status;
//////            AboutBox.Text = u.About;
//////            IpText.Text = u.IpAddress;

//////            LoadAvatar();
//////        }

//////        private void LoadAvatar()
//////        {
//////            if (!string.IsNullOrEmpty(user.AvatarPath))
//////            {
//////                AvatarImage.Source = new BitmapImage(new Uri(user.AvatarPath));
//////            }
//////        }

//////        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
//////        {
//////            OpenFileDialog dlg = new()
//////            {
//////                Filter = "Images|*.png;*.jpg;*.jpeg"
//////            };

//////            if (dlg.ShowDialog() == true)
//////            {
//////                user.AvatarPath = dlg.FileName;
//////                AvatarImage.Source = new BitmapImage(new Uri(dlg.FileName));
//////            }
//////        }

//////        // 🔥 FIXED SAFE THEME READ
//////        private string GetSelectedTheme()
//////        {
//////            if (ThemeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
//////                return item.Content?.ToString() ?? "Dark";

//////            return "Dark";
//////        }

//////        private void SaveButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            user.Username = NameBox.Text;
//////            user.Status = StatusBox.Text;
//////            user.About = AboutBox.Text;

//////            // ✅ FIXED (NO VALUE ERROR)
//////            user.Theme = GetSelectedTheme();

//////            user.LastSeen = DateTime.Now;
//////            user.IsOnline = true;

//////            var main = Application.Current.Windows
//////                .OfType<MainWindow>()
//////                .FirstOrDefault();

//////            main?.SyncUser(user);

//////            Close();
//////        }
//////    }
//////}
//////using Microsoft.Win32;
//////using SpaceTcpChat.Models;
//////using System;
//////using System.Linq;
//////using System.Windows;
//////using System.Windows.Media.Imaging;

//////namespace WpfApp1.Views
//////{
//////    public partial class ProfileWindow : Window
//////    {
//////        private UserModel user;

//////        public ProfileWindow(UserModel u)
//////        {
//////            InitializeComponent();

//////            user = u;

//////            NameBox.Text = u.Username;
//////            StatusBox.Text = u.Status;
//////            AboutBox.Text = u.About;
//////            IpText.Text = u.IpAddress;

//////            LoadAvatar();
//////        }

//////        private void LoadAvatar()
//////        {
//////            if (!string.IsNullOrEmpty(user.AvatarPath))
//////            {
//////                AvatarImage.Source = new BitmapImage(new Uri(user.AvatarPath));
//////            }
//////        }

//////        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
//////        {
//////            OpenFileDialog dlg = new OpenFileDialog()  // ← ИСПРАВЛЕНО
//////            {
//////                Filter = "Images|*.png;*.jpg;*.jpeg"
//////            };

//////            if (dlg.ShowDialog() == true)
//////            {
//////                user.AvatarPath = dlg.FileName;
//////                AvatarImage.Source = new BitmapImage(new Uri(dlg.FileName));
//////            }
//////        }

//////        // SAFE THEME READ
//////        private string GetSelectedTheme()
//////        {
//////            if (ThemeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
//////                return item.Content?.ToString() ?? "Dark";

//////            return "Dark";
//////        }

//////        private void SaveButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            user.Username = NameBox.Text;
//////            user.Status = StatusBox.Text;
//////            user.About = AboutBox.Text;

//////            user.Theme = GetSelectedTheme();

//////            user.LastSeen = DateTime.Now;
//////            user.IsOnline = true;

//////            var main = Application.Current.Windows
//////                .OfType<MainWindow>()
//////                .FirstOrDefault();

//////            main?.SyncUser(user);

//////            Close();
//////        }
//////    }
//////}using Microsoft.Win32;
////using Microsoft.Win32;
////using SpaceTcpChat.Models;
////using System;
////using System.Linq;
////using System.Windows;
////using System.Windows.Media.Imaging;
////using WpfApp1.Models;

////namespace WpfApp1.Views
////{
////    public partial class ProfileWindow : Window
////    {
////        private UserModel user;

////        public ProfileWindow(UserModel u)
////        {
////            InitializeComponent();

////            user = u;

////            NameBox.Text = u.Username;
////            StatusBox.Text = u.Status;
////            AboutBox.Text = u.About;
////            IpText.Text = u.IpAddress;

////            LoadAvatar();
////        }

////        private void LoadAvatar()
////        {
////            if (!string.IsNullOrEmpty(user.AvatarPath))
////            {
////                AvatarImage.Source = new BitmapImage(new Uri(user.AvatarPath));
////            }
////        }

////        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
////        {
////            OpenFileDialog dlg = new OpenFileDialog()
////            {
////                Filter = "Images|*.png;*.jpg;*.jpeg"
////            };

////            if (dlg.ShowDialog() == true)
////            {
////                user.AvatarPath = dlg.FileName;
////                AvatarImage.Source = new BitmapImage(new Uri(dlg.FileName));
////            }
////        }

////        private string GetSelectedTheme()
////        {
////            if (ThemeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
////                return item.Content?.ToString() ?? "Dark";

////            return "Dark";
////        }

////        private void SaveButton_Click(object sender, RoutedEventArgs e)
////        {
////            user.Username = NameBox.Text;
////            user.Status = StatusBox.Text;
////            user.About = AboutBox.Text;

////            user.Theme = GetSelectedTheme();

////            user.LastSeen = DateTime.Now;
////            user.IsOnline = true;

////            var main = Application.Current.Windows
////                .OfType<MainWindow>()
////                .FirstOrDefault();

////            main?.SyncUser(user);

////            Close();
////        }
////    }
////}
//using Microsoft.Win32;
//using SpaceTcpChat.Models;
//using System;
//using System.Linq;
//using System.Windows;
//using System.Windows.Media.Imaging;
//using WpfApp1.Models;

//namespace WpfApp1.Views
//{
//    public partial class ProfileWindow : Window
//    {
//        private UserModel user;
//        private ChatClient _chatClient;
//        public ProfileWindow(UserModel u)
//        {
//            InitializeComponent();

//            user = u;

//            NameBox.Text = u.Username;
//            StatusBox.Text = u.Status;
//            AboutBox.Text = u.About;
//            IpText.Text = u.IpAddress;

//            // БЕЗОПАСНАЯ ЗАГРУЗКА АВАТАРА
//            LoadAvatarSafe();


//        }

//        public ProfileWindow(UserModel u, ChatClient chatClient) : this(u)
//        {
//            _chatClient = chatClient;
//        }


//        /// <summary>
//        /// Безопасная загрузка аватара с проверками
//        /// </summary>
//        private void LoadAvatarSafe()
//        {
//            try
//            {
//                // СНАЧАЛА ПРОВЕРЯЕМ БАЙТЫ АВАТАРА
//                if (user.AvatarBytes != null && user.AvatarBytes.Length > 0)
//                {
//                    using (var ms = new System.IO.MemoryStream(user.AvatarBytes))
//                    {
//                        var bitmap = new BitmapImage();
//                        bitmap.BeginInit();
//                        bitmap.StreamSource = ms;
//                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                        bitmap.EndInit();
//                        AvatarImage.Source = bitmap;
//                        return;
//                    }
//                }

//                // ПОТОМ ПРОВЕРЯЕМ ПУТЬ К ФАЙЛУ
//                if (!string.IsNullOrWhiteSpace(user.AvatarPath) && System.IO.File.Exists(user.AvatarPath))
//                {
//                    var bitmap = new BitmapImage();
//                    bitmap.BeginInit();
//                    bitmap.UriSource = new Uri(user.AvatarPath, UriKind.Absolute);
//                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                    bitmap.EndInit();
//                    AvatarImage.Source = bitmap;
//                    return;
//                }

//                SetDefaultAvatar();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки аватара: {ex.Message}");
//                SetDefaultAvatar();
//            }
//        }

//        /// <summary>
//        /// Установка дефолтного аватара
//        /// </summary>
//        private void SetDefaultAvatar()
//        {
//            try
//            {
//                // Пробуем загрузить дефолтный аватар из ресурсов
//                var uri = new Uri("pack://application:,,,/Images/default_avatar.png", UriKind.Absolute);
//                if (System.IO.File.Exists(uri.LocalPath))
//                {
//                    var bitmap = new BitmapImage();
//                    bitmap.BeginInit();
//                    bitmap.UriSource = uri;
//                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                    bitmap.EndInit();
//                    AvatarImage.Source = bitmap;
//                }
//                else
//                {
//                    // Если дефолтного аватара нет - оставляем пустым
//                    AvatarImage.Source = null;
//                }
//            }
//            catch
//            {
//                AvatarImage.Source = null;
//            }
//        }

//        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                OpenFileDialog dlg = new OpenFileDialog()
//                {
//                    Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
//                    Title = "Выберите аватар"
//                };

//                if (dlg.ShowDialog() == true)
//                {
//                    // Проверяем, что файл существует
//                    if (System.IO.File.Exists(dlg.FileName))
//                    {
//                        user.AvatarPath = dlg.FileName;

//                        // Загружаем новый аватар
//                        var bitmap = new BitmapImage();
//                        bitmap.BeginInit();
//                        bitmap.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
//                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
//                        bitmap.EndInit();
//                        AvatarImage.Source = bitmap;
//                    }
//                    else
//                    {
//                        MessageBox.Show("Файл не найден!", "Ошибка",
//                            MessageBoxButton.OK, MessageBoxImage.Warning);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка загрузки аватара: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private string GetSelectedTheme()
//        {
//            try
//            {
//                if (ThemeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
//                    return item.Content?.ToString() ?? "Dark";
//            }
//            catch { }
//            return "Dark";
//        }

//        private async void SaveButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                user.Username = NameBox.Text?.Trim() ?? "User";
//                user.Status = StatusBox.Text?.Trim() ?? "Online";
//                user.About = AboutBox.Text?.Trim() ?? "";
//                user.Theme = GetSelectedTheme();
//                user.LastSeen = DateTime.Now;
//                user.IsOnline = true;

//                // КОНВЕРТИРУЕМ АВАТАР В БАЙТЫ
//                byte[] avatarBytes = null;
//                if (!string.IsNullOrWhiteSpace(user.AvatarPath) && System.IO.File.Exists(user.AvatarPath))
//                {
//                    try
//                    {
//                        avatarBytes = System.IO.File.ReadAllBytes(user.AvatarPath);

//                        if (avatarBytes.Length > 8 * 1024 * 1024)
//                        {
//                            MessageBox.Show("Аватар слишком большой (макс. 8MB)", "Ошибка",
//                                MessageBoxButton.OK, MessageBoxImage.Warning);
//                            return;
//                        }

//                        user.AvatarBytes = avatarBytes;  // <-- СОХРАНЯЕМ БАЙТЫ
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Ошибка чтения аватара: {ex.Message}", "Ошибка",
//                            MessageBoxButton.OK, MessageBoxImage.Warning);
//                    }
//                }

//                // Отправляем на сервер
//                if (_chatClient != null)
//                {
//                    var updateRequest = new UpdateClientRequest
//                    {
//                        Name = user.Username,
//                        Login = user.Username,
//                        Avatar = avatarBytes  // <-- ОТПРАВЛЯЕМ БАЙТЫ
//                    };

//                    await _chatClient.UpdateClientAsync(updateRequest);
//                }

//                var main = Application.Current.Windows
//                    .OfType<MainWindow>()
//                    .FirstOrDefault();

//                if (main != null)
//                {
//                    main.SyncUser(user);
//                }

//                Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка сохранения профиля: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//    }
//}
using Microsoft.Win32;
using SpaceTcpChat.Models;
using System;
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

        public ProfileWindow(UserModel u)
        {
            InitializeComponent();

            user = u;

            NameBox.Text = u.Username;
            StatusBox.Text = u.Status;
            AboutBox.Text = u.About;
            IpText.Text = u.IpAddress;

            // Устанавливаем тему
            SetThemeComboBox(u.Theme);

            LoadAvatarSafe();
        }

        public ProfileWindow(UserModel u, ChatClient chatClient) : this(u)
        {
            _chatClient = chatClient;
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
                // Сначала проверяем байты аватара
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

                // Затем проверяем путь к файлу
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

                // Пробуем загрузить из ресурсов
                try
                {
                    var uri = new Uri("pack://application:,,,/Images/default_avatar.png", UriKind.Absolute);
                    if (System.IO.File.Exists(uri.LocalPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = uri;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        AvatarImage.Source = bitmap;
                        return;
                    }
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
                if (System.IO.File.Exists(uri.LocalPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    AvatarImage.Source = bitmap;
                }
                else
                {
                    AvatarImage.Source = null;
                }
            }
            catch
            {
                AvatarImage.Source = null;
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
                        // Сохраняем путь и загружаем изображение
                        user.AvatarPath = dlg.FileName;

                        // Читаем байты сразу
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

                // Отправляем на сервер
                if (_chatClient != null && _chatClient.IsConnected())
                {
                    var updateRequest = new UpdateClientRequest
                    {
                        Name = user.Username,
                        Login = user.Username,
                        Avatar = user.AvatarBytes
                    };

                    MessageBox.Show(
    updateRequest.Avatar == null
        ? "CLIENT → Avatar NULL"
        : $"CLIENT → Avatar size = {updateRequest.Avatar.Length}"
);

                    await _chatClient.UpdateClientAsync(updateRequest);
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
                MessageBox.Show($"Ошибка сохранения профиля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}