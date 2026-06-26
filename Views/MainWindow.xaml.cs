////////////using Microsoft.Win32;
////////////using SpaceTcpChat.Models;
////////////using SpaceTcpChat.Windows;
////////////using System;
////////////using System.Collections.ObjectModel;
////////////using System.Linq;
////////////using System.Windows;
////////////using System.Windows.Threading;

////////////namespace SpaceTcpChat
////////////{
////////////    public partial class MainWindow : Window
////////////    {
////////////        public ObservableCollection<UserModel> Users = new();
////////////        public ObservableCollection<MessageModel> Messages = new();

////////////        private UserModel CurrentUser;

////////////        private DispatcherTimer typingTimer;
////////////        private DispatcherTimer onlineTimer;

////////////        public MainWindow()
////////////        {
////////////            InitializeComponent();

////////////            UsersList.ItemsSource = Users;
////////////            MessagesList.ItemsSource = Messages;

////////////            SetupTimers();
////////////            LoadUser();
////////////        }

////////////        // ================= TIMERS =================
////////////        private void SetupTimers()
////////////        {
////////////            typingTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(1)
////////////            };

////////////            typingTimer.Tick += (s, e) =>
////////////            {
////////////                TypingText.Text = "";
////////////                typingTimer.Stop();
////////////            };

////////////            onlineTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(3)
////////////            };

////////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////////////            onlineTimer.Start();
////////////        }

////////////        // ================= USER LOAD =================
////////////        private void LoadUser()
////////////        {
////////////            CurrentUser = new UserModel
////////////            {
////////////                Username = "SpaceUser",
////////////                Status = "Online",
////////////                IpAddress = "127.0.0.1",
////////////                IsOnline = true,
////////////                LastSeen = DateTime.Now,
////////////                Theme = "Dark",
////////////                AvatarPath = ""
////////////            };

////////////            Users.Add(CurrentUser);
////////////        }

////////////        // ================= SEND =================
////////////        private void SendButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
////////////                return;

////////////            Messages.Add(new MessageModel
////////////            {
////////////                Username = CurrentUser.Username,
////////////                Message = MessageTextBox.Text,
////////////                Time = DateTime.Now.ToShortTimeString(),
////////////                IsMine = true,
////////////                Sender = CurrentUser,
////////////                SenderAvatar = CurrentUser.AvatarPath
////////////            });

////////////            MessageTextBox.Clear();
////////////        }

////////////        // ================= FILE =================
////////////        private void FileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            OpenFileDialog dlg = new()
////////////            {
////////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
////////////            };

////////////            if (dlg.ShowDialog() == true)
////////////            {
////////////                Messages.Add(new MessageModel
////////////                {
////////////                    Username = CurrentUser.Username,
////////////                    Message = "File",
////////////                    FilePath = dlg.FileName,
////////////                    IsMine = true,
////////////                    Sender = CurrentUser,
////////////                    Time = DateTime.Now.ToShortTimeString(),
////////////                    SenderAvatar = CurrentUser.AvatarPath
////////////                });
////////////            }
////////////        }

////////////        // ================= EMOJI =================
////////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            MessageTextBox.Text += " 😊";
////////////        }

////////////        // ================= TYPING =================
////////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////////////        {
////////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
////////////            typingTimer.Stop();
////////////            typingTimer.Start();
////////////        }

////////////        // ================= TOUCH USER =================
////////////        private void TouchUser()
////////////        {
////////////            CurrentUser.LastSeen = DateTime.Now;
////////////            CurrentUser.IsOnline = true;
////////////        }

////////////        // ================= ONLINE SYSTEM =================
////////////        private void UpdateOnlineStatus()
////////////        {
////////////            foreach (var user in Users)
////////////            {
////////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////////////            }

////////////            UsersList.Items.Refresh();
////////////        }

////////////        // ================= APPLY THEME =================
////////////        public void ApplyTheme(string theme)
////////////        {
////////////            Background = theme switch
////////////            {
////////////                "Purple" => new System.Windows.Media.SolidColorBrush(
////////////                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D")),

////////////                "NeonBlue" => new System.Windows.Media.SolidColorBrush(
////////////                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E")),

////////////                "SpaceBlack" => System.Windows.Media.Brushes.Black,

////////////                _ => new System.Windows.Media.SolidColorBrush(
////////////                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"))
////////////            };
////////////        }

////////////        // ================= SYNC USER =================
////////////        public void SyncUser(UserModel updated)
////////////        {
////////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////////////            if (user == null) return;

////////////            user.Username = updated.Username;
////////////            user.Status = updated.Status;
////////////            user.About = updated.About;
////////////            user.AvatarPath = updated.AvatarPath;
////////////            user.Theme = updated.Theme;
////////////            user.LastSeen = DateTime.Now;
////////////            user.IsOnline = true;

////////////            UsersList.Items.Refresh();

////////////            if (updated.Username == CurrentUser.Username)
////////////            {
////////////                CurrentUser = user;
////////////                ApplyTheme(user.Theme);
////////////                RefreshMessagesAvatars();
////////////            }
////////////        }

////////////        // ================= AVATAR SYNC =================
////////////        public void RefreshMessagesAvatars()
////////////        {
////////////            foreach (var msg in Messages)
////////////            {
////////////                if (msg.Username == CurrentUser.Username)
////////////                {
////////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////////////                }
////////////            }

////////////            MessagesList.Items.Refresh();
////////////        }

////////////        // ================= PROFILE =================
////////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            var w = new ProfileWindow(CurrentUser);
////////////            w.ShowDialog();
////////////        }

////////////        // ================= DOUBLE CLICK =================
////////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
////////////        {
////////////            if (UsersList.SelectedItem is UserModel u)
////////////                new ProfileWindow(u).ShowDialog();
////////////        }

////////////        // ================= EXIT =================
////////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            Application.Current.Shutdown();
////////////        }
////////////    }
////////////}

////////////using Microsoft.Win32;
////////////using SpaceTcpChat.Models;
////////////using SpaceTcpChat.Windows;
////////////using System;
////////////using System.Collections.ObjectModel;
////////////using System.Linq;
////////////using System.Windows;
////////////using System.Windows.Threading;

////////////namespace WpfApp1.Views
////////////{
////////////    public partial class MainWindow : Window
////////////    {
////////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
////////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

////////////        private UserModel CurrentUser;

////////////        private DispatcherTimer typingTimer;
////////////        private DispatcherTimer onlineTimer;

////////////        public MainWindow()
////////////        {
////////////            InitializeComponent();

////////////            UsersList.ItemsSource = Users;
////////////            MessagesList.ItemsSource = Messages;

////////////            SetupTimers();
////////////            LoadUser();
////////////        }

////////////        // ================= TIMERS =================
////////////        private void SetupTimers()
////////////        {
////////////            typingTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(1)
////////////            };

////////////            typingTimer.Tick += (s, e) =>
////////////            {
////////////                TypingText.Text = "";
////////////                typingTimer.Stop();
////////////            };

////////////            onlineTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(3)
////////////            };

////////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////////////            onlineTimer.Start();
////////////        }

////////////        // ================= USER LOAD =================
////////////        private void LoadUser()
////////////        {
////////////            CurrentUser = new UserModel
////////////            {
////////////                Username = "SpaceUser",
////////////                Status = "Online",
////////////                IpAddress = "127.0.0.1",
////////////                IsOnline = true,
////////////                LastSeen = DateTime.Now,
////////////                Theme = "Dark",
////////////                AvatarPath = ""
////////////            };

////////////            Users.Add(CurrentUser);
////////////        }

////////////        // ================= SEND =================
////////////        private void SendButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
////////////                return;

////////////            Messages.Add(new MessageModel
////////////            {
////////////                Username = CurrentUser.Username,
////////////                Message = MessageTextBox.Text,
////////////                Time = DateTime.Now.ToShortTimeString(),
////////////                IsMine = true,
////////////                Sender = CurrentUser,
////////////                SenderAvatar = CurrentUser.AvatarPath
////////////            });

////////////            MessageTextBox.Clear();
////////////        }

////////////        // ================= FILE =================
////////////        private void FileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            OpenFileDialog dlg = new OpenFileDialog()  // ← ИСПРАВЛЕНО
////////////            {
////////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
////////////            };

////////////            if (dlg.ShowDialog() == true)
////////////            {
////////////                Messages.Add(new MessageModel
////////////                {
////////////                    Username = CurrentUser.Username,
////////////                    Message = "File",
////////////                    FilePath = dlg.FileName,
////////////                    IsMine = true,
////////////                    Sender = CurrentUser,
////////////                    Time = DateTime.Now.ToShortTimeString(),
////////////                    SenderAvatar = CurrentUser.AvatarPath
////////////                });
////////////            }
////////////        }

////////////        // ================= EMOJI =================
////////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            MessageTextBox.Text += " 😊";
////////////        }

////////////        // ================= TYPING =================
////////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////////////        {
////////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
////////////            typingTimer.Stop();
////////////            typingTimer.Start();
////////////        }

////////////        // ================= TOUCH USER =================
////////////        private void TouchUser()
////////////        {
////////////            CurrentUser.LastSeen = DateTime.Now;
////////////            CurrentUser.IsOnline = true;
////////////        }

////////////        // ================= ONLINE SYSTEM =================
////////////        private void UpdateOnlineStatus()
////////////        {
////////////            foreach (var user in Users)
////////////            {
////////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////////////            }

////////////            UsersList.Items.Refresh();
////////////        }

////////////        // ================= APPLY THEME =================
////////////        public void ApplyTheme(string theme)  // ← ИСПРАВЛЕНО
////////////        {
////////////            switch (theme)
////////////            {
////////////                case "Purple":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
////////////                    break;
////////////                case "NeonBlue":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
////////////                    break;
////////////                case "SpaceBlack":
////////////                    Background = System.Windows.Media.Brushes.Black;
////////////                    break;
////////////                default:
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"));
////////////                    break;
////////////            }
////////////        }

////////////        // ================= SYNC USER =================
////////////        public void SyncUser(UserModel updated)
////////////        {
////////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////////////            if (user == null) return;

////////////            user.Username = updated.Username;
////////////            user.Status = updated.Status;
////////////            user.About = updated.About;
////////////            user.AvatarPath = updated.AvatarPath;
////////////            user.Theme = updated.Theme;
////////////            user.LastSeen = DateTime.Now;
////////////            user.IsOnline = true;

////////////            UsersList.Items.Refresh();

////////////            if (updated.Username == CurrentUser.Username)
////////////            {
////////////                CurrentUser = user;
////////////                ApplyTheme(user.Theme);
////////////                RefreshMessagesAvatars();
////////////            }
////////////        }

////////////        // ================= AVATAR SYNC =================
////////////        public void RefreshMessagesAvatars()
////////////        {
////////////            foreach (var msg in Messages)
////////////            {
////////////                if (msg.Username == CurrentUser.Username)
////////////                {
////////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////////////                }
////////////            }

////////////            MessagesList.Items.Refresh();
////////////        }

////////////        // ================= PROFILE =================
////////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            var w = new ProfileWindow(CurrentUser);
////////////            w.ShowDialog();
////////////        }

////////////        // ================= DOUBLE CLICK =================
////////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
////////////        {
////////////            if (UsersList.SelectedItem is UserModel u)
////////////                new ProfileWindow(u).ShowDialog();
////////////        }

////////////        // ================= EXIT =================
////////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            Application.Current.Shutdown();
////////////        }
////////////    }
////////////}

////////////using Microsoft.Win32;
////////////using SpaceTcpChat.Models;
////////////using System;
////////////using System.Collections.ObjectModel;
////////////using System.Linq;
////////////using System.Windows;
////////////using System.Windows.Threading;
////////////using TcpChatClient.Services;

////////////namespace WpfApp1.Views
////////////{
////////////    public partial class MainWindow : Window
////////////    {
////////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
////////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

////////////        private UserModel CurrentUser;
////////////        private TcpClientService _chatService;
////////////        private string _currentUsername;

////////////        private DispatcherTimer typingTimer;
////////////        private DispatcherTimer onlineTimer;

////////////        // Конструктор без параметров (для совместимости)
////////////        public MainWindow()
////////////        {
////////////            InitializeComponent();
////////////            SetupTimers();
////////////            LoadUser("SpaceUser");
////////////        }

////////////        // Основной конструктор
////////////        public MainWindow(TcpClientService chatService, string username)
////////////        {
////////////            InitializeComponent();

////////////            _chatService = chatService;
////////////            _currentUsername = username;

////////////            UsersList.ItemsSource = Users;
////////////            MessagesList.ItemsSource = Messages;

////////////            SetupTimers();
////////////            LoadUser(username);

////////////            if (_chatService != null)
////////////            {
////////////                StartReceivingMessages();
////////////            }
////////////        }

////////////        private async void StartReceivingMessages()
////////////        {
////////////            try
////////////            {
////////////                while (true)
////////////                {
////////////                    string message = await _chatService.ReceiveMessageAsync();

////////////                    Dispatcher.Invoke(() =>
////////////                    {
////////////                        int separatorIndex = message.IndexOf(':');
////////////                        if (separatorIndex > 0)
////////////                        {
////////////                            string sender = message.Substring(0, separatorIndex);
////////////                            string content = message.Substring(separatorIndex + 1).TrimStart();

////////////                            Messages.Add(new MessageModel
////////////                            {
////////////                                Username = sender,
////////////                                Message = content,
////////////                                Time = DateTime.Now.ToShortTimeString(),
////////////                                IsMine = sender == _currentUsername,
////////////                                Sender = new UserModel { Username = sender }
////////////                            });

////////////                            if (MessagesList.Items.Count > 0)
////////////                                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////////                        }
////////////                    });
////////////                }
////////////            }
////////////            catch
////////////            {
////////////                Dispatcher.Invoke(() =>
////////////                {
////////////                    MessageBox.Show("Соединение с сервером потеряно", "Ошибка",
////////////                        MessageBoxButton.OK, MessageBoxImage.Error);
////////////                });
////////////            }
////////////        }

////////////        private void SetupTimers()
////////////        {
////////////            typingTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(1)
////////////            };

////////////            typingTimer.Tick += (s, e) =>
////////////            {
////////////                TypingText.Text = "";
////////////                typingTimer.Stop();
////////////            };

////////////            onlineTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(3)
////////////            };

////////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////////////            onlineTimer.Start();
////////////        }

////////////        private void LoadUser(string username)
////////////        {
////////////            CurrentUser = new UserModel
////////////            {
////////////                Username = username,
////////////                Status = "Online",
////////////                IpAddress = "127.0.0.1",
////////////                IsOnline = true,
////////////                LastSeen = DateTime.Now,
////////////                Theme = "Dark",
////////////                AvatarPath = ""
////////////            };

////////////            Users.Add(CurrentUser);
////////////        }

////////////        private async void SendButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
////////////                return;

////////////            string messageText = MessageTextBox.Text;

////////////            if (_chatService != null)
////////////            {
////////////                await _chatService.SendMessageAsync($"{_currentUsername}: {messageText}");
////////////            }

////////////            Messages.Add(new MessageModel
////////////            {
////////////                Username = CurrentUser.Username,
////////////                Message = messageText,
////////////                Time = DateTime.Now.ToShortTimeString(),
////////////                IsMine = true,
////////////                Sender = CurrentUser,
////////////                SenderAvatar = CurrentUser.AvatarPath
////////////            });

////////////            MessageTextBox.Clear();

////////////            if (MessagesList.Items.Count > 0)
////////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////////        }

////////////        private void FileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            OpenFileDialog dlg = new OpenFileDialog()
////////////            {
////////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
////////////            };

////////////            if (dlg.ShowDialog() == true)
////////////            {
////////////                Messages.Add(new MessageModel
////////////                {
////////////                    Username = CurrentUser.Username,
////////////                    Message = "File",
////////////                    FilePath = dlg.FileName,
////////////                    IsMine = true,
////////////                    Sender = CurrentUser,
////////////                    Time = DateTime.Now.ToShortTimeString(),
////////////                    SenderAvatar = CurrentUser.AvatarPath
////////////                });
////////////            }
////////////        }

////////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            MessageTextBox.Text += " 😊";
////////////        }

////////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////////////        {
////////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
////////////            typingTimer.Stop();
////////////            typingTimer.Start();
////////////        }

////////////        private void TouchUser()
////////////        {
////////////            CurrentUser.LastSeen = DateTime.Now;
////////////            CurrentUser.IsOnline = true;
////////////        }

////////////        private void UpdateOnlineStatus()
////////////        {
////////////            foreach (var user in Users)
////////////            {
////////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////////////            }

////////////            UsersList.Items.Refresh();
////////////        }

////////////        public void ApplyTheme(string theme)
////////////        {
////////////            switch (theme)
////////////            {
////////////                case "Purple":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
////////////                    break;
////////////                case "NeonBlue":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
////////////                    break;
////////////                case "SpaceBlack":
////////////                    Background = System.Windows.Media.Brushes.Black;
////////////                    break;
////////////                default:
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"));
////////////                    break;
////////////            }
////////////        }

////////////        public void SyncUser(UserModel updated)
////////////        {
////////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////////////            if (user == null) return;

////////////            user.Username = updated.Username;
////////////            user.Status = updated.Status;
////////////            user.About = updated.About;
////////////            user.AvatarPath = updated.AvatarPath;
////////////            user.Theme = updated.Theme;
////////////            user.LastSeen = DateTime.Now;
////////////            user.IsOnline = true;

////////////            UsersList.Items.Refresh();

////////////            if (updated.Username == CurrentUser.Username)
////////////            {
////////////                CurrentUser = user;
////////////                ApplyTheme(user.Theme);
////////////                RefreshMessagesAvatars();
////////////            }
////////////        }

////////////        public void RefreshMessagesAvatars()
////////////        {
////////////            foreach (var msg in Messages)
////////////            {
////////////                if (msg.Username == CurrentUser.Username)
////////////                {
////////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////////////                }
////////////            }

////////////            MessagesList.Items.Refresh();
////////////        }

////////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            var w = new ProfileWindow(CurrentUser);
////////////            w.ShowDialog();
////////////        }

////////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
////////////        {
////////////            if (UsersList.SelectedItem is UserModel u)
////////////                new ProfileWindow(u).ShowDialog();
////////////        }

////////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            _chatService?.Disconnect();
////////////            Application.Current.Shutdown();
////////////        }

////////////        protected override void OnClosed(EventArgs e)
////////////        {
////////////            _chatService?.Disconnect();
////////////            base.OnClosed(e);
////////////        }
////////////    }
////////////}
////////////using Microsoft.Win32;
////////////using SpaceTcpChat.Models;
////////////using System;
////////////using System.Collections.ObjectModel;
////////////using System.Linq;
////////////using System.Windows;
////////////using System.Windows.Threading;
////////////using TcpChatClient.Services;
////////////using WpfApp1.Models;

////////////namespace WpfApp1.Views
////////////{
////////////    public partial class MainWindow : Window
////////////    {
////////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
////////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

////////////        private UserModel CurrentUser;
////////////        private TcpClientService _chatService;
////////////        private string _currentUsername;

////////////        private DispatcherTimer typingTimer;
////////////        private DispatcherTimer onlineTimer;

////////////        private readonly ChatClient _chatClient;
////////////        private readonly int _userId;
////////////        private readonly string _userName;

////////////        // ОРИГИНАЛЬНЫЙ КОНСТРУКТОР (НЕ ТРОГАЕМ)
////////////        public MainWindow(ChatClient chatClient,int userId,string userName)
////////////        {


////////////            InitializeComponent();

////////////            _chatClient = chatClient;
////////////            _userId = userId;
////////////            _userName = userName;

////////////            //UsersList.ItemsSource = Users;
////////////            //MessagesList.ItemsSource = Messages;

////////////            SetupTimers();
////////////            LoadUser();
////////////        }

////////////        // НОВЫЙ КОНСТРУКТОР (ДОБАВЛЕН)
////////////        //public MainWindow(TcpClientService chatService, string username)
////////////        //{
////////////        //    InitializeComponent();

////////////        //    _chatService = chatService;
////////////        //    _currentUsername = username;

////////////        //    UsersList.ItemsSource = Users;
////////////        //    MessagesList.ItemsSource = Messages;

////////////        //    SetupTimers();
////////////        //    LoadUser(username);

////////////        //    if (_chatService != null)
////////////        //    {
////////////        //        StartReceivingMessages();
////////////        //    }
////////////        //}



////////////        // НОВЫЙ МЕТОД ДЛЯ ПРИЁМА СООБЩЕНИЙ
////////////        private async void StartReceivingMessages()
////////////        {
////////////            try
////////////            {
////////////                while (true)
////////////                {
////////////                    string message = await _chatService.ReceiveMessageAsync();

////////////                    Dispatcher.Invoke(() =>
////////////                    {
////////////                        int separatorIndex = message.IndexOf(':');
////////////                        if (separatorIndex > 0)
////////////                        {
////////////                            string sender = message.Substring(0, separatorIndex);
////////////                            string content = message.Substring(separatorIndex + 1).TrimStart();

////////////                            Messages.Add(new MessageModel
////////////                            {
////////////                                Username = sender,
////////////                                Message = content,
////////////                                Time = DateTime.Now.ToShortTimeString(),
////////////                                IsMine = sender == _currentUsername,
////////////                                Sender = new UserModel { Username = sender }
////////////                            });

////////////                            if (MessagesList.Items.Count > 0)
////////////                                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////////                        }
////////////                    });
////////////                }
////////////            }
////////////            catch
////////////            {
////////////                Dispatcher.Invoke(() =>
////////////                {
////////////                    MessageBox.Show("Соединение с сервером потеряно", "Ошибка",
////////////                        MessageBoxButton.OK, MessageBoxImage.Error);
////////////                });
////////////            }
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ SetupTimers (НЕ ТРОГАЕМ)
////////////        private void SetupTimers()
////////////        {
////////////            typingTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(1)
////////////            };

////////////            typingTimer.Tick += (s, e) =>
////////////            {
////////////                TypingText.Text = "";
////////////                typingTimer.Stop();
////////////            };

////////////            onlineTimer = new DispatcherTimer
////////////            {
////////////                Interval = TimeSpan.FromSeconds(3)
////////////            };

////////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////////////            onlineTimer.Start();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ LoadUser (БЕЗ ПАРАМЕТРОВ)
////////////        private void LoadUser()
////////////        {
////////////            CurrentUser = new UserModel
////////////            {
////////////                Username = "SpaceUser",
////////////                Status = "Online",
////////////                IpAddress = "127.0.0.1",
////////////                IsOnline = true,
////////////                LastSeen = DateTime.Now,
////////////                Theme = "Dark",
////////////                AvatarPath = ""
////////////            };

////////////            Users.Add(CurrentUser);
////////////        }

////////////        // НОВЫЙ LoadUser (С ПАРАМЕТРОМ)
////////////        private void LoadUser(string username)
////////////        {
////////////            CurrentUser = new UserModel
////////////            {
////////////                Username = username,
////////////                Status = "Online",
////////////                IpAddress = "127.0.0.1",
////////////                IsOnline = true,
////////////                LastSeen = DateTime.Now,
////////////                Theme = "Dark",
////////////                AvatarPath = ""
////////////            };

////////////            Users.Add(CurrentUser);
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ SendButton_Click (НЕ ТРОГАЕМ, НО ДОБАВЛЕНА ОТПРАВКА НА СЕРВЕР)
////////////        private async void SendButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
////////////                return;

////////////            string messageText = MessageTextBox.Text;

////////////            // ДОБАВЛЕНО: отправка на сервер
////////////            if (_chatService != null)
////////////            {
////////////                await _chatService.SendMessageAsync($"{_currentUsername}: {messageText}");
////////////            }

////////////            Messages.Add(new MessageModel
////////////            {
////////////                Username = CurrentUser.Username,
////////////                Message = messageText,
////////////                Time = DateTime.Now.ToShortTimeString(),
////////////                IsMine = true,
////////////                Sender = CurrentUser,
////////////                SenderAvatar = CurrentUser.AvatarPath
////////////            });

////////////            MessageTextBox.Clear();

////////////            if (MessagesList.Items.Count > 0)
////////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ FileButton_Click (НЕ ТРОГАЕМ)
////////////        private void FileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            TouchUser();

////////////            OpenFileDialog dlg = new OpenFileDialog()
////////////            {
////////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
////////////            };

////////////            if (dlg.ShowDialog() == true)
////////////            {
////////////                Messages.Add(new MessageModel
////////////                {
////////////                    Username = CurrentUser.Username,
////////////                    Message = "File",
////////////                    FilePath = dlg.FileName,
////////////                    IsMine = true,
////////////                    Sender = CurrentUser,
////////////                    Time = DateTime.Now.ToShortTimeString(),
////////////                    SenderAvatar = CurrentUser.AvatarPath
////////////                });
////////////            }
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ EmojiButton_Click (НЕ ТРОГАЕМ)
////////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            MessageTextBox.Text += " 😊";
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ MessageTextBox_TextChanged (НЕ ТРОГАЕМ)
////////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////////////        {
////////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
////////////            typingTimer.Stop();
////////////            typingTimer.Start();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ TouchUser (НЕ ТРОГАЕМ)
////////////        private void TouchUser()
////////////        {
////////////            CurrentUser.LastSeen = DateTime.Now;
////////////            CurrentUser.IsOnline = true;
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ UpdateOnlineStatus (НЕ ТРОГАЕМ)
////////////        private void UpdateOnlineStatus()
////////////        {
////////////            foreach (var user in Users)
////////////            {
////////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////////////            }
////////////            UsersList.Items.Refresh();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ ApplyTheme (НЕ ТРОГАЕМ)
////////////        public void ApplyTheme(string theme)
////////////        {
////////////            switch (theme)
////////////            {
////////////                case "Purple":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
////////////                    break;
////////////                case "NeonBlue":
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
////////////                    break;
////////////                case "SpaceBlack":
////////////                    Background = System.Windows.Media.Brushes.Black;
////////////                    break;
////////////                default:
////////////                    Background = new System.Windows.Media.SolidColorBrush(
////////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"));
////////////                    break;
////////////            }
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ SyncUser (НЕ ТРОГАЕМ)
////////////        public void SyncUser(UserModel updated)
////////////        {
////////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////////////            if (user == null) return;

////////////            user.Username = updated.Username;
////////////            user.Status = updated.Status;
////////////            user.About = updated.About;
////////////            user.AvatarPath = updated.AvatarPath;
////////////            user.Theme = updated.Theme;
////////////            user.LastSeen = DateTime.Now;
////////////            user.IsOnline = true;

////////////            UsersList.Items.Refresh();

////////////            if (updated.Username == CurrentUser.Username)
////////////            {
////////////                CurrentUser = user;
////////////                ApplyTheme(user.Theme);
////////////                RefreshMessagesAvatars();
////////////            }
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ RefreshMessagesAvatars (НЕ ТРОГАЕМ)
////////////        public void RefreshMessagesAvatars()
////////////        {
////////////            foreach (var msg in Messages)
////////////            {
////////////                if (msg.Username == CurrentUser.Username)
////////////                {
////////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////////////                }
////////////            }
////////////            MessagesList.Items.Refresh();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ ProfileButton_Click (НЕ ТРОГАЕМ)
////////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            var w = new ProfileWindow(CurrentUser);
////////////            w.ShowDialog();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ UsersList_MouseDoubleClick (НЕ ТРОГАЕМ)
////////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
////////////        {
////////////            if (UsersList.SelectedItem is UserModel u)
////////////                new ProfileWindow(u).ShowDialog();
////////////        }

////////////        // ОРИГИНАЛЬНЫЙ ExitButton_Click (НЕ ТРОГАЕМ, НО ДОБАВЛЕНО ОТКЛЮЧЕНИЕ)
////////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////////////        {
////////////            _chatService?.Disconnect();
////////////            Application.Current.Shutdown();
////////////        }

////////////        // ДОБАВЛЕНО: закрытие окна
////////////        protected override void OnClosed(EventArgs e)
////////////        {
////////////            _chatService?.Disconnect();
////////////            base.OnClosed(e);
////////////        }
////////////    }
////////////}

//////////using Microsoft.Win32;
//////////using SpaceTcpChat.Models;
//////////using System;
//////////using System.Collections.Generic;
//////////using System.Collections.ObjectModel;
//////////using System.Linq;
//////////using System.Text.Json;
//////////using System.Windows;
//////////using System.Windows.Threading;
//////////using WpfApp1.Models;

//////////namespace WpfApp1.Views
//////////{
//////////    public partial class MainWindow : Window
//////////    {
//////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
//////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

//////////        private UserModel CurrentUser;
//////////        private DispatcherTimer typingTimer;
//////////        private DispatcherTimer onlineTimer;

//////////        private readonly ChatClient _chatClient;
//////////        private readonly int _userId;
//////////        private readonly string _userName;

//////////        // КОНСТРУКТОР
//////////        public MainWindow(ChatClient chatClient, int userId, string userName)
//////////        {
//////////            InitializeComponent();

//////////            _chatClient = chatClient;
//////////            _userId = userId;
//////////            _userName = userName;

//////////            UsersList.ItemsSource = Users;
//////////            MessagesList.ItemsSource = Messages;

//////////            SetupTimers();
//////////            LoadUser();

//////////            // ЗАПУСКАЕМ ПРИЁМ СООБЩЕНИЙ
//////////            StartReceivingMessages();

//////////            // ЗАГРУЖАЕМ ИСТОРИЮ
//////////            LoadMessageHistory();
//////////        }

//////////        private void LoadUser()
//////////        {
//////////            CurrentUser = new UserModel
//////////            {
//////////                Id = _userId,  // Важно: добавляем Id
//////////                Username = _userName,
//////////                Status = "Online",
//////////                IpAddress = "127.0.0.1",
//////////                IsOnline = true,
//////////                LastSeen = DateTime.Now,
//////////                Theme = "Dark",
//////////                AvatarPath = ""
//////////            };

//////////            Users.Add(CurrentUser);
//////////        }

//////////        // ПРИЁМ СООБЩЕНИЙ ОТ СЕРВЕРА
//////////        private async void StartReceivingMessages()
//////////        {
//////////            try
//////////            {
//////////                while (_chatClient.IsConnected())
//////////                {
//////////                    Packet? packet = await _chatClient.ReceivePacketAsync();

//////////                    if (packet == null)
//////////                        continue;

//////////                    // Обновляем UI в потоке UI
//////////                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
//////////                }
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                await Dispatcher.InvokeAsync(() =>
//////////                {
//////////                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
//////////                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//////////                });
//////////            }
//////////        }

//////////        // ОБРАБОТКА ПАКЕТОВ ОТ СЕРВЕРА
//////////        private void ProcessPacket(Packet packet)
//////////        {
//////////            switch (packet.Type)
//////////            {
//////////                case PacketType.MessageReceived:
//////////                    // Сообщение от другого клиента
//////////                    var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//////////                    if (messageResponse != null && messageResponse.FromClientId != _userId)
//////////                    {
//////////                        AddMessageToUI(messageResponse);
//////////                    }
//////////                    break;

//////////                case PacketType.MessageAdded:
//////////                    // Подтверждение отправки (своё сообщение)
//////////                    var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//////////                    if (confirmResponse?.Success == true)
//////////                    {
//////////                        // Своё сообщение уже добавлено, можно ничего не делать
//////////                        System.Diagnostics.Debug.WriteLine("Message sent successfully");
//////////                    }
//////////                    break;

//////////                case PacketType.MessageHistoryReceived:
//////////                    // История сообщений
//////////                    var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//////////                    if (messages != null)
//////////                    {
//////////                        LoadMessageHistory(messages);
//////////                    }
//////////                    break;

//////////                case PacketType.ClientStatusChanged:
//////////                    // Обновление статуса пользователя (если есть)
//////////                    try
//////////                    {
//////////                        var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//////////                        if (statusResponse != null)
//////////                        {
//////////                            UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//////////                        }
//////////                    }
//////////                    catch { }
//////////                    break;
//////////            }
//////////        }

//////////        // ДОБАВЛЕНИЕ СООБЩЕНИЯ В UI
//////////        private void AddMessageToUI(MessageResponse message)
//////////        {
//////////            var messageModel = new MessageModel
//////////            {
//////////                Username = message.SenderName,
//////////                Message = message.Text,
//////////                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////////                IsMine = false,
//////////                Sender = new UserModel { Id = message.FromClientId, Username = message.SenderName }
//////////            };

//////////            Messages.Add(messageModel);

//////////            // Прокрутка вниз
//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////////        }

//////////        // ЗАГРУЗКА ИСТОРИИ СООБЩЕНИЙ
//////////        private void LoadMessageHistory(List<MessageResponse> messages)
//////////        {
//////////            Messages.Clear();

//////////            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
//////////            {
//////////                Messages.Add(new MessageModel
//////////                {
//////////                    Username = msg.SenderName,
//////////                    Message = msg.Text,
//////////                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////////                    IsMine = msg.FromClientId == _userId,
//////////                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
//////////                });
//////////            }

//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////////        }

//////////        // ЗАПРОС ИСТОРИИ ПРИ СТАРТЕ
//////////        private async void LoadMessageHistory()
//////////        {
//////////            try
//////////            {
//////////                await _chatClient.GetAllMessages();
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
//////////            }
//////////        }

//////////        // ОБНОВЛЕНИЕ СТАТУСА ПОЛЬЗОВАТЕЛЯ
//////////        private void UpdateUserStatus(int clientId, bool isOnline)
//////////        {
//////////            var user = Users.FirstOrDefault(u => u.Id == clientId);
//////////            if (user != null)
//////////            {
//////////                user.IsOnline = isOnline;
//////////                user.Status = isOnline ? "Online" : "Offline";
//////////                UsersList?.Items.Refresh();
//////////            }
//////////        }

//////////        // ОТПРАВКА СООБЩЕНИЯ
//////////        private async void SendButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
//////////                return;

//////////            string messageText = MessageTextBox.Text;
//////////            string time = DateTime.Now.ToString("HH:mm");

//////////            // Добавляем своё сообщение в UI сразу
//////////            var myMessage = new MessageModel
//////////            {
//////////                Username = _userName,
//////////                Message = messageText,
//////////                Time = time,
//////////                IsMine = true,
//////////                Sender = CurrentUser,
//////////                SenderAvatar = CurrentUser.AvatarPath
//////////            };

//////////            Messages.Add(myMessage);

//////////            // Прокрутка вниз
//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

//////////            // Очищаем поле ввода
//////////            MessageTextBox.Clear();

//////////            // Отправляем на сервер
//////////            try
//////////            {
//////////                await _chatClient.SendMessageAsync(_userId, messageText);
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
//////////                    MessageBoxButton.OK, MessageBoxImage.Error);

//////////                // Удаляем сообщение, если не отправилось
//////////                Messages.Remove(myMessage);
//////////            }
//////////        }

//////////        // ОСТАЛЬНЫЕ МЕТОДЫ (НЕ ИЗМЕНЯЕМ)
//////////        private void SetupTimers()
//////////        {
//////////            typingTimer = new DispatcherTimer
//////////            {
//////////                Interval = TimeSpan.FromSeconds(1)
//////////            };

//////////            typingTimer.Tick += (s, e) =>
//////////            {
//////////                TypingText.Text = "";
//////////                typingTimer.Stop();
//////////            };

//////////            onlineTimer = new DispatcherTimer
//////////            {
//////////                Interval = TimeSpan.FromSeconds(3)
//////////            };

//////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
//////////            onlineTimer.Start();
//////////        }

//////////        private void TouchUser()
//////////        {
//////////            CurrentUser.LastSeen = DateTime.Now;
//////////            CurrentUser.IsOnline = true;
//////////        }

//////////        private void UpdateOnlineStatus()
//////////        {
//////////            foreach (var user in Users)
//////////            {
//////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
//////////            }
//////////            UsersList?.Items.Refresh();
//////////        }

//////////        private void FileButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            TouchUser();
//////////            OpenFileDialog dlg = new OpenFileDialog()
//////////            {
//////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
//////////            };

//////////            if (dlg.ShowDialog() == true)
//////////            {
//////////                Messages.Add(new MessageModel
//////////                {
//////////                    Username = CurrentUser.Username,
//////////                    Message = "File",
//////////                    FilePath = dlg.FileName,
//////////                    IsMine = true,
//////////                    Sender = CurrentUser,
//////////                    Time = DateTime.Now.ToShortTimeString(),
//////////                    SenderAvatar = CurrentUser.AvatarPath
//////////                });
//////////            }
//////////        }

//////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            MessageTextBox.Text += " 😊";
//////////        }

//////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//////////        {
//////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
//////////            typingTimer.Stop();
//////////            typingTimer.Start();
//////////        }

//////////        public void ApplyTheme(string theme)
//////////        {
//////////            switch (theme)
//////////            {
//////////                case "Purple":
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
//////////                    break;
//////////                case "NeonBlue":
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
//////////                    break;
//////////                case "SpaceBlack":
//////////                    Background = System.Windows.Media.Brushes.Black;
//////////                    break;
//////////                default:
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"));
//////////                    break;
//////////            }
//////////        }

//////////        public void SyncUser(UserModel updated)
//////////        {
//////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
//////////            if (user == null) return;

//////////            user.Username = updated.Username;
//////////            user.Status = updated.Status;
//////////            user.About = updated.About;
//////////            user.AvatarPath = updated.AvatarPath;
//////////            user.Theme = updated.Theme;
//////////            user.LastSeen = DateTime.Now;
//////////            user.IsOnline = true;

//////////            UsersList?.Items.Refresh();

//////////            if (updated.Username == CurrentUser.Username)
//////////            {
//////////                CurrentUser = user;
//////////                ApplyTheme(user.Theme);
//////////                RefreshMessagesAvatars();
//////////            }
//////////        }

//////////        public void RefreshMessagesAvatars()
//////////        {
//////////            foreach (var msg in Messages)
//////////            {
//////////                if (msg.Username == CurrentUser.Username)
//////////                {
//////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
//////////                }
//////////            }
//////////            MessagesList?.Items.Refresh();
//////////        }

//////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            var w = new ProfileWindow(CurrentUser);
//////////            w.ShowDialog();
//////////        }

//////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
//////////        {
//////////            if (UsersList.SelectedItem is UserModel u)
//////////                new ProfileWindow(u).ShowDialog();
//////////        }

//////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            _chatClient?.Disconnect();
//////////            Application.Current.Shutdown();
//////////        }

//////////        protected override void OnClosed(EventArgs e)
//////////        {
//////////            _chatClient?.Disconnect();
//////////            base.OnClosed(e);
//////////        }
//////////    }
//////////}

//////////using Microsoft.Win32;
//////////using SpaceTcpChat.Models;
//////////using System;
//////////using System.Collections.Generic;
//////////using System.Collections.ObjectModel;
//////////using System.Linq;
//////////using System.Text.Json;
//////////using System.Windows;
//////////using System.Windows.Threading;
//////////using WpfApp1.Models;

//////////namespace WpfApp1.Views
//////////{
//////////    public partial class MainWindow : Window
//////////    {
//////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
//////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

//////////        private UserModel CurrentUser;
//////////        private DispatcherTimer typingTimer;
//////////        private DispatcherTimer onlineTimer;

//////////        private readonly ChatClient _chatClient;
//////////        private readonly int _userId;
//////////        private readonly string _userName;

//////////        // КОНСТРУКТОР
//////////        public MainWindow(ChatClient chatClient, int userId, string userName)
//////////        {
//////////            InitializeComponent();

//////////            _chatClient = chatClient;
//////////            _userId = userId;
//////////            _userName = userName;

//////////            UsersList.ItemsSource = Users;
//////////            MessagesList.ItemsSource = Messages;

//////////            SetupTimers();
//////////            LoadUser();

//////////            // ЗАПУСКАЕМ ПРИЁМ СООБЩЕНИЙ
//////////            StartReceivingMessages();

//////////            // ЗАГРУЖАЕМ ИСТОРИЮ
//////////            LoadMessageHistory();
//////////        }

//////////        private void LoadUser()
//////////        {
//////////            CurrentUser = new UserModel
//////////            {
//////////                Id = _userId,
//////////                Username = _userName,
//////////                Status = "Online",
//////////                IpAddress = "127.0.0.1",
//////////                IsOnline = true,
//////////                LastSeen = DateTime.Now,
//////////                Theme = "Dark",
//////////                AvatarPath = ""
//////////            };

//////////            Users.Add(CurrentUser);
//////////        }

//////////        // ПРИЁМ СООБЩЕНИЙ ОТ СЕРВЕРА
//////////        private async void StartReceivingMessages()
//////////        {
//////////            try
//////////            {
//////////                while (_chatClient.IsConnected())
//////////                {
//////////                    Packet packet = await _chatClient.ReceivePacketAsync();

//////////                    if (packet == null)
//////////                        continue;

//////////                    // Обновляем UI в потоке UI
//////////                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
//////////                }
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                await Dispatcher.InvokeAsync(() =>
//////////                {
//////////                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
//////////                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//////////                });
//////////            }
//////////        }

//////////        // ОБРАБОТКА ПАКЕТОВ ОТ СЕРВЕРА
//////////        private void ProcessPacket(Packet packet)
//////////        {
//////////            try
//////////            {
//////////                switch (packet.Type)
//////////                {
//////////                    case PacketType.MessageReceived:
//////////                        // Сообщение от другого клиента
//////////                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//////////                        if (messageResponse != null && messageResponse.FromClientId != _userId)
//////////                        {
//////////                            AddMessageToUI(messageResponse);
//////////                        }
//////////                        break;

//////////                    case PacketType.MessageAdded:
//////////                        // Подтверждение отправки (своё сообщение)
//////////                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//////////                        if (confirmResponse?.Success == true)
//////////                        {
//////////                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
//////////                        }
//////////                        break;

//////////                    case PacketType.MessageHistoryReceived:
//////////                        // История сообщений
//////////                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//////////                        if (messages != null)
//////////                        {
//////////                            LoadMessageHistory(messages);
//////////                        }
//////////                        break;

//////////                    case PacketType.ClientStatusChanged:
//////////                        // Обновление статуса пользователя
//////////                        try
//////////                        {
//////////                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//////////                            if (statusResponse != null)
//////////                            {
//////////                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//////////                            }
//////////                        }
//////////                        catch { }
//////////                        break;
//////////                }
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
//////////            }
//////////        }

//////////        // ДОБАВЛЕНИЕ СООБЩЕНИЯ В UI
//////////        private void AddMessageToUI(MessageResponse message)
//////////        {
//////////            var messageModel = new MessageModel
//////////            {
//////////                Username = message.SenderName,
//////////                Message = message.Text,
//////////                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////////                IsMine = false,
//////////                Sender = new UserModel { Id = message.FromClientId, Username = message.SenderName }
//////////            };

//////////            Messages.Add(messageModel);

//////////            // Прокрутка вниз
//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////////        }

//////////        // ЗАГРУЗКА ИСТОРИИ СООБЩЕНИЙ
//////////        private void LoadMessageHistory(List<MessageResponse> messages)
//////////        {
//////////            Messages.Clear();

//////////            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
//////////            {
//////////                Messages.Add(new MessageModel
//////////                {
//////////                    Username = msg.SenderName,
//////////                    Message = msg.Text,
//////////                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////////                    IsMine = msg.FromClientId == _userId,
//////////                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
//////////                });
//////////            }

//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////////        }

//////////        // ЗАПРОС ИСТОРИИ ПРИ СТАРТЕ
//////////        private async void LoadMessageHistory()
//////////        {
//////////            try
//////////            {
//////////                await _chatClient.GetAllMessages();
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
//////////            }
//////////        }

//////////        // ОБНОВЛЕНИЕ СТАТУСА ПОЛЬЗОВАТЕЛЯ
//////////        private void UpdateUserStatus(int clientId, bool isOnline)
//////////        {
//////////            var user = Users.FirstOrDefault(u => u.Id == clientId);
//////////            if (user != null)
//////////            {
//////////                user.IsOnline = isOnline;
//////////                user.Status = isOnline ? "Online" : "Offline";
//////////                UsersList?.Items.Refresh();
//////////            }
//////////        }

//////////        // ОТПРАВКА СООБЩЕНИЯ
//////////        private async void SendButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
//////////                return;

//////////            string messageText = MessageTextBox.Text;
//////////            string time = DateTime.Now.ToString("HH:mm");

//////////            // Добавляем своё сообщение в UI сразу
//////////            var myMessage = new MessageModel
//////////            {
//////////                Username = _userName,
//////////                Message = messageText,
//////////                Time = time,
//////////                IsMine = true,
//////////                Sender = CurrentUser,
//////////                SenderAvatar = CurrentUser.AvatarPath
//////////            };

//////////            Messages.Add(myMessage);

//////////            // Прокрутка вниз
//////////            if (MessagesList.Items.Count > 0)
//////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

//////////            // Очищаем поле ввода
//////////            MessageTextBox.Clear();

//////////            // Отправляем на сервер
//////////            try
//////////            {
//////////                await _chatClient.SendMessageAsync(_userId, messageText);
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
//////////                    MessageBoxButton.OK, MessageBoxImage.Error);

//////////                // Удаляем сообщение, если не отправилось
//////////                Messages.Remove(myMessage);
//////////            }
//////////        }

//////////        private void SetupTimers()
//////////        {
//////////            typingTimer = new DispatcherTimer
//////////            {
//////////                Interval = TimeSpan.FromSeconds(1)
//////////            };

//////////            typingTimer.Tick += (s, e) =>
//////////            {
//////////                TypingText.Text = "";
//////////                typingTimer.Stop();
//////////            };

//////////            onlineTimer = new DispatcherTimer
//////////            {
//////////                Interval = TimeSpan.FromSeconds(3)
//////////            };

//////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
//////////            onlineTimer.Start();
//////////        }

//////////        private void TouchUser()
//////////        {
//////////            CurrentUser.LastSeen = DateTime.Now;
//////////            CurrentUser.IsOnline = true;
//////////        }

//////////        private void UpdateOnlineStatus()
//////////        {
//////////            foreach (var user in Users)
//////////            {
//////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
//////////            }
//////////            UsersList?.Items.Refresh();
//////////        }

//////////        private void FileButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            TouchUser();
//////////            OpenFileDialog dlg = new OpenFileDialog()
//////////            {
//////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*"
//////////            };

//////////            if (dlg.ShowDialog() == true)
//////////            {
//////////                Messages.Add(new MessageModel
//////////                {
//////////                    Username = CurrentUser.Username,
//////////                    Message = "File",
//////////                    FilePath = dlg.FileName,
//////////                    IsMine = true,
//////////                    Sender = CurrentUser,
//////////                    Time = DateTime.Now.ToShortTimeString(),
//////////                    SenderAvatar = CurrentUser.AvatarPath
//////////                });
//////////            }
//////////        }

//////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            MessageTextBox.Text += " 😊";
//////////        }

//////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//////////        {
//////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
//////////            typingTimer.Stop();
//////////            typingTimer.Start();
//////////        }

//////////        public void ApplyTheme(string theme)
//////////        {
//////////            switch (theme)
//////////            {
//////////                case "Purple":
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
//////////                    break;
//////////                case "NeonBlue":
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
//////////                    break;
//////////                case "SpaceBlack":
//////////                    Background = System.Windows.Media.Brushes.Black;
//////////                    break;
//////////                default:
//////////                    Background = new System.Windows.Media.SolidColorBrush(
//////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#050816"));
//////////                    break;
//////////            }
//////////        }

//////////        public void SyncUser(UserModel updated)
//////////        {
//////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
//////////            if (user == null) return;

//////////            user.Username = updated.Username;
//////////            user.Status = updated.Status;
//////////            user.About = updated.About;
//////////            user.AvatarPath = updated.AvatarPath;
//////////            user.Theme = updated.Theme;
//////////            user.LastSeen = DateTime.Now;
//////////            user.IsOnline = true;

//////////            UsersList?.Items.Refresh();

//////////            if (updated.Username == CurrentUser.Username)
//////////            {
//////////                CurrentUser = user;
//////////                ApplyTheme(user.Theme);
//////////                RefreshMessagesAvatars();
//////////            }
//////////        }

//////////        public void RefreshMessagesAvatars()
//////////        {
//////////            foreach (var msg in Messages)
//////////            {
//////////                if (msg.Username == CurrentUser.Username)
//////////                {
//////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
//////////                }
//////////            }
//////////            MessagesList?.Items.Refresh();
//////////        }

//////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            var w = new ProfileWindow(CurrentUser);
//////////            w.ShowDialog();
//////////        }

//////////        private void UsersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
//////////        {
//////////            if (UsersList.SelectedItem is UserModel u)
//////////                new ProfileWindow(u).ShowDialog();
//////////        }

//////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
//////////        {
//////////            _chatClient?.Disconnect();
//////////            Application.Current.Shutdown();
//////////        }

//////////        protected override void OnClosed(EventArgs e)
//////////        {
//////////            _chatClient?.Disconnect();
//////////            base.OnClosed(e);
//////////        }
//////////    }
//////////}
////////using Microsoft.Win32;
////////using SpaceTcpChat.Models;
////////using System;
////////using System.Collections.Generic;
////////using System.Collections.ObjectModel;
////////using System.Linq;
////////using System.Text.Json;
////////using System.Windows;
////////using System.Windows.Input;
////////using System.Windows.Threading;
////////using WpfApp1.Models;

////////namespace WpfApp1.Views
////////{
////////    public partial class MainWindow : Window
////////    {
////////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
////////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

////////        private UserModel CurrentUser;
////////        private DispatcherTimer typingTimer;
////////        private DispatcherTimer onlineTimer;

////////        private readonly ChatClient _chatClient;
////////        private readonly int _userId;
////////        private readonly string _userName;

////////        // Конструктор
////////        public MainWindow(ChatClient chatClient, int userId, string userName)
////////        {
////////            InitializeComponent();
////////            DataContext = this;

////////            _chatClient = chatClient;
////////            _userId = userId;
////////            _userName = userName;

////////            UsersList.ItemsSource = Users;
////////            MessagesList.ItemsSource = Messages;

////////            SetupTimers();
////////            LoadUser();

////////            // Запускаем приём сообщений
////////            StartReceivingMessages();

////////            // Загружаем историю
////////            LoadMessageHistory();

////////            // Устанавливаем фокус на поле ввода при загрузке
////////            Loaded += (s, e) => MessageTextBox.Focus();
////////        }

////////        /// <summary>
////////        /// Обработчик нажатия клавиш в поле ввода
////////        /// Enter - отправка, Shift+Enter - перенос строки
////////        /// </summary>
////////        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
////////        {
////////            // Проверяем нажатие Enter без модификаторов
////////            if (e.Key == Key.Enter && !e.KeyboardDevice.IsKeyDown(Key.LeftShift) &&
////////                !e.KeyboardDevice.IsKeyDown(Key.RightShift))
////////            {
////////                // Если текст не пустой - отправляем
////////                if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
////////                {
////////                    SendButton_Click(sender, null);
////////                }
////////                // Блокируем стандартную обработку Enter (чтобы не добавлялась строка)
////////                e.Handled = true;
////////            }
////////            // Если Shift+Enter - разрешаем стандартную обработку (перенос строки)
////////            else if (e.Key == Key.Enter && (e.KeyboardDevice.IsKeyDown(Key.LeftShift) ||
////////                                          e.KeyboardDevice.IsKeyDown(Key.RightShift)))
////////            {
////////                // Разрешаем стандартное поведение - вставка перевода строки
////////                e.Handled = false;
////////            }
////////        }

////////        // Приём сообщений от сервера
////////        private async void StartReceivingMessages()
////////        {
////////            try
////////            {
////////                while (_chatClient.IsConnected())
////////                {
////////                    Packet packet = await _chatClient.ReceivePacketAsync();

////////                    if (packet == null)
////////                        continue;

////////                    // Обновляем UI в потоке UI
////////                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
////////                }
////////            }
////////            catch (Exception ex)
////////            {
////////                await Dispatcher.InvokeAsync(() =>
////////                {
////////                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
////////                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
////////                });
////////            }
////////        }

////////        // Обработка пакетов от сервера
////////        private void ProcessPacket(Packet packet)
////////        {
////////            try
////////            {
////////                switch (packet.Type)
////////                {
////////                    case PacketType.MessageReceived:
////////                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
////////                        if (messageResponse != null && messageResponse.FromClientId != _userId)
////////                        {
////////                            AddMessageToUI(messageResponse);
////////                        }
////////                        break;

////////                    case PacketType.MessageAdded:
////////                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
////////                        if (confirmResponse?.Success == true)
////////                        {
////////                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
////////                        }
////////                        break;

////////                    case PacketType.MessageHistoryReceived:
////////                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
////////                        if (messages != null)
////////                        {
////////                            LoadMessageHistory(messages);
////////                        }
////////                        break;

////////                    case PacketType.ClientStatusChanged:
////////                        try
////////                        {
////////                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
////////                            if (statusResponse != null)
////////                            {
////////                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
////////                            }
////////                        }
////////                        catch { }
////////                        break;
////////                }
////////            }
////////            catch (Exception ex)
////////            {
////////                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
////////            }
////////        }

////////        // Добавление сообщения в UI
////////        private void AddMessageToUI(MessageResponse message)
////////        {
////////            var messageModel = new MessageModel
////////            {
////////                Username = message.SenderName,
////////                Message = message.Text,
////////                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
////////                IsMine = false,
////////                Sender = new UserModel { Id = message.FromClientId, Username = message.SenderName }
////////            };

////////            Messages.Add(messageModel);

////////            // Прокрутка вниз
////////            if (MessagesList.Items.Count > 0)
////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////        }

////////        // Загрузка истории сообщений
////////        private void LoadMessageHistory(List<MessageResponse> messages)
////////        {
////////            Messages.Clear();

////////            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
////////            {
////////                Messages.Add(new MessageModel
////////                {
////////                    Username = msg.SenderName,
////////                    Message = msg.Text,
////////                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
////////                    IsMine = msg.FromClientId == _userId,
////////                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
////////                });
////////            }

////////            if (MessagesList.Items.Count > 0)
////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////////        }

////////        // Запрос истории при старте
////////        private async void LoadMessageHistory()
////////        {
////////            try
////////            {
////////                await _chatClient.GetAllMessages();
////////            }
////////            catch (Exception ex)
////////            {
////////                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
////////            }
////////        }

////////        // Обновление статуса пользователя
////////        private void UpdateUserStatus(int clientId, bool isOnline)
////////        {
////////            var user = Users.FirstOrDefault(u => u.Id == clientId);
////////            if (user != null)
////////            {
////////                user.IsOnline = isOnline;
////////                user.Status = isOnline ? "Online" : "Offline";
////////                UsersList?.Items.Refresh();
////////            }
////////        }

////////        // Отправка сообщения
////////        private async void SendButton_Click(object sender, RoutedEventArgs e)
////////        {
////////            // Получаем текст, удаляя лишние пробелы в начале и конце
////////            string messageText = MessageTextBox.Text?.Trim();

////////            if (string.IsNullOrWhiteSpace(messageText))
////////                return;

////////            string time = DateTime.Now.ToString("HH:mm");

////////            // Добавляем своё сообщение в UI сразу
////////            var myMessage = new MessageModel
////////            {
////////                Username = _userName,
////////                Message = messageText,
////////                Time = time,
////////                IsMine = true,
////////                Sender = CurrentUser,
////////                SenderAvatar = CurrentUser.AvatarPath
////////            };

////////            Messages.Add(myMessage);

////////            // Прокрутка вниз
////////            if (MessagesList.Items.Count > 0)
////////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

////////            // Очищаем поле ввода
////////            MessageTextBox.Clear();

////////            // Отправляем на сервер
////////            try
////////            {
////////                await _chatClient.SendMessageAsync(_userId, messageText);
////////            }
////////            catch (Exception ex)
////////            {
////////                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
////////                    MessageBoxButton.OK, MessageBoxImage.Error);

////////                // Удаляем сообщение, если не отправилось
////////                Messages.Remove(myMessage);
////////            }
////////        }

////////        private void SetupTimers()
////////        {
////////            typingTimer = new DispatcherTimer
////////            {
////////                Interval = TimeSpan.FromSeconds(1.5)
////////            };

////////            typingTimer.Tick += (s, e) =>
////////            {
////////                TypingText.Text = "";
////////                typingTimer.Stop();
////////            };

////////            onlineTimer = new DispatcherTimer
////////            {
////////                Interval = TimeSpan.FromSeconds(3)
////////            };

////////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////////            onlineTimer.Start();
////////        }

////////        private void TouchUser()
////////        {
////////            CurrentUser.LastSeen = DateTime.Now;
////////            CurrentUser.IsOnline = true;
////////        }

////////        private void UpdateOnlineStatus()
////////        {
////////            foreach (var user in Users)
////////            {
////////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////////            }
////////            UsersList?.Items.Refresh();
////////        }

////////        private void FileButton_Click(object sender, RoutedEventArgs e)
////////        {
////////            TouchUser();
////////            OpenFileDialog dlg = new OpenFileDialog()
////////            {
////////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
////////                Title = "Выберите файл"
////////            };

////////            if (dlg.ShowDialog() == true)
////////            {
////////                Messages.Add(new MessageModel
////////                {
////////                    Username = CurrentUser.Username,
////////                    Message = "📎 Файл",
////////                    FilePath = dlg.FileName,
////////                    IsMine = true,
////////                    Sender = CurrentUser,
////////                    Time = DateTime.Now.ToShortTimeString(),
////////                    SenderAvatar = CurrentUser.AvatarPath
////////                });
////////            }
////////        }

////////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////////        {
////////            MessageTextBox.Text += " 😊";
////////            MessageTextBox.Focus();
////////            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
////////        }

////////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////////        {
////////            TypingText.Text = $"{CurrentUser.Username} печатает...";
////////            typingTimer.Stop();
////////            typingTimer.Start();
////////        }

////////        public void ApplyTheme(string theme)
////////        {
////////            switch (theme)
////////            {
////////                case "Purple":
////////                    Background = new System.Windows.Media.SolidColorBrush(
////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
////////                    break;
////////                case "NeonBlue":
////////                    Background = new System.Windows.Media.SolidColorBrush(
////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
////////                    break;
////////                case "SpaceBlack":
////////                    Background = System.Windows.Media.Brushes.Black;
////////                    break;
////////                default:
////////                    Background = new System.Windows.Media.SolidColorBrush(
////////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
////////                    break;
////////            }
////////        }

////////        public void SyncUser(UserModel updated)
////////        {
////////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////////            if (user == null) return;

////////            user.Username = updated.Username;
////////            user.Status = updated.Status;
////////            user.About = updated.About;
////////            user.AvatarPath = updated.AvatarPath;
////////            user.Theme = updated.Theme;
////////            user.LastSeen = DateTime.Now;
////////            user.IsOnline = true;

////////            UsersList?.Items.Refresh();

////////            if (updated.Username == CurrentUser.Username)
////////            {
////////                CurrentUser = user;
////////                ApplyTheme(user.Theme);
////////                RefreshMessagesAvatars();
////////            }
////////        }

////////        public void RefreshMessagesAvatars()
////////        {
////////            foreach (var msg in Messages)
////////            {
////////                if (msg.Username == CurrentUser.Username)
////////                {
////////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////////                }
////////            }
////////            MessagesList?.Items.Refresh();
////////        }

////////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////////        {
////////            var w = new ProfileWindow(CurrentUser);
////////            w.ShowDialog();
////////        }

////////        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
////////        {
////////            if (UsersList.SelectedItem is UserModel u)
////////                new ProfileWindow(u).ShowDialog();
////////        }

////////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////////        {
////////            _chatClient?.Disconnect();
////////            Application.Current.Shutdown();
////////        }

////////        protected override void OnClosed(EventArgs e)
////////        {
////////            _chatClient?.Disconnect();
////////            base.OnClosed(e);
////////        }
////////    }
////////}
//////using Microsoft.Win32;
//////using SpaceTcpChat.Models;
//////using System;
//////using System.Collections.Generic;
//////using System.Collections.ObjectModel;
//////using System.Linq;
//////using System.Text.Json;
//////using System.Windows;
//////using System.Windows.Input;
//////using System.Windows.Threading;
//////using WpfApp1.Models;

//////namespace WpfApp1.Views
//////{
//////    public partial class MainWindow : Window
//////    {
//////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
//////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

//////        private UserModel CurrentUser;
//////        private DispatcherTimer typingTimer;
//////        private DispatcherTimer onlineTimer;

//////        private readonly ChatClient _chatClient;
//////        private readonly int _userId;
//////        private readonly string _userName;

//////        // Конструктор
//////        public MainWindow(ChatClient chatClient, int userId, string userName)
//////        {
//////            InitializeComponent();
//////            DataContext = this;

//////            _chatClient = chatClient;
//////            _userId = userId;
//////            _userName = userName;

//////            UsersList.ItemsSource = Users;
//////            MessagesList.ItemsSource = Messages;

//////            SetupTimers();
//////            LoadUser(); // <-- Вызов метода

//////            // Запускаем приём сообщений
//////            StartReceivingMessages();

//////            // Загружаем историю
//////            LoadMessageHistory();

//////            // Устанавливаем фокус на поле ввода при загрузке
//////            Loaded += (s, e) => MessageTextBox.Focus();
//////        }

//////        /// <summary>
//////        /// Загрузка текущего пользователя
//////        /// </summary>
//////        private void LoadUser()
//////        {
//////            CurrentUser = new UserModel
//////            {
//////                Id = _userId,
//////                Username = _userName,
//////                Status = "Online",
//////                IpAddress = "127.0.0.1",
//////                IsOnline = true,
//////                LastSeen = DateTime.Now,
//////                Theme = "Dark",
//////                AvatarPath = "/Images/default_avatar.png",
//////                About = "Привет! Я в Space Chat"
//////            };

//////            Users.Add(CurrentUser);
//////        }

//////        /// <summary>
//////        /// Обработчик нажатия клавиш в поле ввода
//////        /// Enter - отправка, Shift+Enter - перенос строки
//////        /// </summary>
//////        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
//////        {
//////            // Проверяем нажатие Enter без модификаторов
//////            if (e.Key == Key.Enter && !e.KeyboardDevice.IsKeyDown(Key.LeftShift) &&
//////                !e.KeyboardDevice.IsKeyDown(Key.RightShift))
//////            {
//////                // Если текст не пустой - отправляем
//////                if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
//////                {
//////                    SendButton_Click(sender, null);
//////                }
//////                // Блокируем стандартную обработку Enter (чтобы не добавлялась строка)
//////                e.Handled = true;
//////            }
//////            // Если Shift+Enter - разрешаем стандартную обработку (перенос строки)
//////            else if (e.Key == Key.Enter && (e.KeyboardDevice.IsKeyDown(Key.LeftShift) ||
//////                                          e.KeyboardDevice.IsKeyDown(Key.RightShift)))
//////            {
//////                // Разрешаем стандартное поведение - вставка перевода строки
//////                e.Handled = false;
//////            }
//////        }

//////        // Приём сообщений от сервера
//////        private async void StartReceivingMessages()
//////        {
//////            try
//////            {
//////                while (_chatClient.IsConnected())
//////                {
//////                    Packet packet = await _chatClient.ReceivePacketAsync();

//////                    if (packet == null)
//////                        continue;

//////                    // Обновляем UI в потоке UI
//////                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                await Dispatcher.InvokeAsync(() =>
//////                {
//////                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
//////                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//////                });
//////            }
//////        }

//////        // Обработка пакетов от сервера
//////        private void ProcessPacket(Packet packet)
//////        {
//////            try
//////            {
//////                switch (packet.Type)
//////                {
//////                    case PacketType.MessageReceived:
//////                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//////                        if (messageResponse != null && messageResponse.FromClientId != _userId)
//////                        {
//////                            AddMessageToUI(messageResponse);
//////                        }
//////                        break;

//////                    case PacketType.MessageAdded:
//////                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//////                        if (confirmResponse?.Success == true)
//////                        {
//////                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
//////                        }
//////                        break;

//////                    case PacketType.MessageHistoryReceived:
//////                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//////                        if (messages != null)
//////                        {
//////                            LoadMessageHistory(messages);
//////                        }
//////                        break;

//////                    case PacketType.ClientStatusChanged:
//////                        try
//////                        {
//////                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//////                            if (statusResponse != null)
//////                            {
//////                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//////                            }
//////                        }
//////                        catch { }
//////                        break;
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
//////            }
//////        }

//////        // Добавление сообщения в UI
//////        private void AddMessageToUI(MessageResponse message)
//////        {
//////            var messageModel = new MessageModel
//////            {
//////                Username = message.SenderName,
//////                Message = message.Text,
//////                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////                IsMine = false,
//////                Sender = new UserModel { Id = message.FromClientId, Username = message.SenderName }
//////            };

//////            Messages.Add(messageModel);

//////            // Прокрутка вниз
//////            if (MessagesList.Items.Count > 0)
//////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////        }

//////        // Загрузка истории сообщений
//////        private void LoadMessageHistory(List<MessageResponse> messages)
//////        {
//////            Messages.Clear();

//////            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
//////            {
//////                Messages.Add(new MessageModel
//////                {
//////                    Username = msg.SenderName,
//////                    Message = msg.Text,
//////                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
//////                    IsMine = msg.FromClientId == _userId,
//////                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
//////                });
//////            }

//////            if (MessagesList.Items.Count > 0)
//////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//////        }

//////        // Запрос истории при старте
//////        private async void LoadMessageHistory()
//////        {
//////            try
//////            {
//////                await _chatClient.GetAllMessages();
//////            }
//////            catch (Exception ex)
//////            {
//////                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
//////            }
//////        }

//////        // Обновление статуса пользователя
//////        private void UpdateUserStatus(int clientId, bool isOnline)
//////        {
//////            var user = Users.FirstOrDefault(u => u.Id == clientId);
//////            if (user != null)
//////            {
//////                user.IsOnline = isOnline;
//////                user.Status = isOnline ? "Online" : "Offline";
//////                UsersList?.Items.Refresh();
//////            }
//////        }

//////        // Отправка сообщения
//////        private async void SendButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            // Получаем текст, удаляя лишние пробелы в начале и конце
//////            string messageText = MessageTextBox.Text?.Trim();

//////            if (string.IsNullOrWhiteSpace(messageText))
//////                return;

//////            string time = DateTime.Now.ToString("HH:mm");

//////            // Добавляем своё сообщение в UI сразу
//////            var myMessage = new MessageModel
//////            {
//////                Username = _userName,
//////                Message = messageText,
//////                Time = time,
//////                IsMine = true,
//////                Sender = CurrentUser,
//////                SenderAvatar = CurrentUser?.AvatarPath
//////            };

//////            Messages.Add(myMessage);

//////            // Прокрутка вниз
//////            if (MessagesList.Items.Count > 0)
//////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

//////            // Очищаем поле ввода
//////            MessageTextBox.Clear();

//////            // Отправляем на сервер
//////            try
//////            {
//////                await _chatClient.SendMessageAsync(_userId, messageText);
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
//////                    MessageBoxButton.OK, MessageBoxImage.Error);

//////                // Удаляем сообщение, если не отправилось
//////                Messages.Remove(myMessage);
//////            }
//////        }

//////        private void SetupTimers()
//////        {
//////            typingTimer = new DispatcherTimer
//////            {
//////                Interval = TimeSpan.FromSeconds(1.5)
//////            };

//////            typingTimer.Tick += (s, e) =>
//////            {
//////                TypingText.Text = "";
//////                typingTimer.Stop();
//////            };

//////            onlineTimer = new DispatcherTimer
//////            {
//////                Interval = TimeSpan.FromSeconds(3)
//////            };

//////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
//////            onlineTimer.Start();
//////        }

//////        private void TouchUser()
//////        {
//////            if (CurrentUser != null)
//////            {
//////                CurrentUser.LastSeen = DateTime.Now;
//////                CurrentUser.IsOnline = true;
//////            }
//////        }

//////        private void UpdateOnlineStatus()
//////        {
//////            foreach (var user in Users)
//////            {
//////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
//////            }
//////            UsersList?.Items.Refresh();
//////        }

//////        private void FileButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            TouchUser();
//////            OpenFileDialog dlg = new OpenFileDialog()
//////            {
//////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
//////                Title = "Выберите файл"
//////            };

//////            if (dlg.ShowDialog() == true)
//////            {
//////                Messages.Add(new MessageModel
//////                {
//////                    Username = CurrentUser?.Username ?? _userName,
//////                    Message = "📎 Файл",
//////                    FilePath = dlg.FileName,
//////                    IsMine = true,
//////                    Sender = CurrentUser,
//////                    Time = DateTime.Now.ToShortTimeString(),
//////                    SenderAvatar = CurrentUser?.AvatarPath
//////                });
//////            }
//////        }

//////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            MessageTextBox.Text += " 😊";
//////            MessageTextBox.Focus();
//////            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
//////        }

//////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//////        {
//////            if (CurrentUser != null)
//////            {
//////                TypingText.Text = $"{CurrentUser.Username} печатает...";
//////                typingTimer.Stop();
//////                typingTimer.Start();
//////            }
//////        }

//////        public void ApplyTheme(string theme)
//////        {
//////            switch (theme)
//////            {
//////                case "Purple":
//////                    Background = new System.Windows.Media.SolidColorBrush(
//////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
//////                    break;
//////                case "NeonBlue":
//////                    Background = new System.Windows.Media.SolidColorBrush(
//////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
//////                    break;
//////                case "SpaceBlack":
//////                    Background = System.Windows.Media.Brushes.Black;
//////                    break;
//////                default:
//////                    Background = new System.Windows.Media.SolidColorBrush(
//////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
//////                    break;
//////            }
//////        }

//////        public void SyncUser(UserModel updated)
//////        {
//////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
//////            if (user == null) return;

//////            user.Username = updated.Username;
//////            user.Status = updated.Status;
//////            user.About = updated.About;
//////            user.AvatarPath = updated.AvatarPath;
//////            user.Theme = updated.Theme;
//////            user.LastSeen = DateTime.Now;
//////            user.IsOnline = true;

//////            UsersList?.Items.Refresh();

//////            if (updated.Username == CurrentUser?.Username)
//////            {
//////                CurrentUser = user;
//////                ApplyTheme(user.Theme);
//////                RefreshMessagesAvatars();
//////            }
//////        }

//////        public void RefreshMessagesAvatars()
//////        {
//////            foreach (var msg in Messages)
//////            {
//////                if (msg.Username == CurrentUser?.Username)
//////                {
//////                    msg.SenderAvatar = CurrentUser.AvatarPath;
//////                }
//////            }
//////            MessagesList?.Items.Refresh();
//////        }

//////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            if (CurrentUser != null)
//////            {
//////                var w = new ProfileWindow(CurrentUser);
//////                w.ShowDialog();
//////            }
//////        }

//////        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//////        {
//////            if (UsersList.SelectedItem is UserModel u)
//////                new ProfileWindow(u).ShowDialog();
//////        }

//////        private void ExitButton_Click(object sender, RoutedEventArgs e)
//////        {
//////            _chatClient?.Disconnect();
//////            Application.Current.Shutdown();
//////        }

//////        protected override void OnClosed(EventArgs e)
//////        {
//////            _chatClient?.Disconnect();
//////            base.OnClosed(e);
//////        }
//////    }
//////}
////using Microsoft.Win32;
////using SpaceTcpChat.Models;
////using System;
////using System.Collections.Generic;
////using System.Collections.ObjectModel;
////using System.Linq;
////using System.Text.Json;
////using System.Windows;
////using System.Windows.Input;
////using System.Windows.Threading;
////using WpfApp1.Models;

////namespace WpfApp1.Views
////{
////    public partial class MainWindow : Window
////    {
////        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
////        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

////        private UserModel CurrentUser;
////        private DispatcherTimer typingTimer;
////        private DispatcherTimer onlineTimer;

////        private readonly ChatClient _chatClient;
////        private readonly int _userId;
////        private readonly string _userName;

////        // Конструктор
////        public MainWindow(ChatClient chatClient, int userId, string userName)
////        {
////            InitializeComponent();
////            DataContext = this;

////            _chatClient = chatClient;
////            _userId = userId;
////            _userName = userName;

////            UsersList.ItemsSource = Users;
////            MessagesList.ItemsSource = Messages;

////            // Подписываемся на событие PreviewKeyDown для перехвата Enter до стандартной обработки
////            MessageTextBox.PreviewKeyDown += MessageTextBox_PreviewKeyDown;

////            SetupTimers();
////            LoadUser();

////            // Запускаем приём сообщений
////            StartReceivingMessages();

////            // Загружаем историю
////            LoadMessageHistory();

////            // Устанавливаем фокус на поле ввода при загрузке
////            Loaded += (s, e) => MessageTextBox.Focus();
////        }

////        /// <summary>
////        /// Загрузка текущего пользователя
////        /// </summary>
////        private void LoadUser()
////        {
////            CurrentUser = new UserModel
////            {
////                Id = _userId,
////                Username = _userName,
////                Status = "Online",
////                IpAddress = "127.0.0.1",
////                IsOnline = true,
////                LastSeen = DateTime.Now,
////                Theme = "Dark",
////                AvatarPath = "/Images/default_avatar.png",
////                About = "Привет! Я в Space Chat"
////            };

////            Users.Add(CurrentUser);
////        }

////        /// <summary>
////        /// Обработчик PreviewKeyDown - перехватывает Enter до стандартной обработки
////        /// </summary>
////        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
////        {
////            // Проверяем нажатие Enter
////            if (e.Key == Key.Enter)
////            {
////                // Проверяем, зажат ли Shift
////                bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

////                if (!isShiftPressed)
////                {
////                    // Enter без Shift - отправляем сообщение
////                    e.Handled = true; // Блокируем стандартную обработку

////                    if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
////                    {
////                        SendButton_Click(sender, null);
////                    }
////                }
////                // Если Shift+Enter - разрешаем стандартную обработку (перенос строки)
////                // e.Handled = false; - это значение по умолчанию
////            }
////        }

////        // Старый обработчик KeyDown оставляем для других клавиш
////        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
////        {
////            // Здесь можно обрабатывать другие клавиши, если нужно
////            // Enter обрабатывается в PreviewKeyDown
////        }

////        // Приём сообщений от сервера
////        private async void StartReceivingMessages()
////        {
////            try
////            {
////                while (_chatClient.IsConnected())
////                {
////                    Packet packet = await _chatClient.ReceivePacketAsync();

////                    if (packet == null)
////                        continue;

////                    // Обновляем UI в потоке UI
////                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
////                }
////            }
////            catch (Exception ex)
////            {
////                await Dispatcher.InvokeAsync(() =>
////                {
////                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
////                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
////                });
////            }
////        }

////        // Обработка пакетов от сервера
////        private void ProcessPacket(Packet packet)
////        {
////            try
////            {
////                switch (packet.Type)
////                {
////                    case PacketType.MessageReceived:
////                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
////                        if (messageResponse != null && messageResponse.FromClientId != _userId)
////                        {
////                            AddMessageToUI(messageResponse);
////                        }
////                        break;

////                    case PacketType.MessageAdded:
////                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
////                        if (confirmResponse?.Success == true)
////                        {
////                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
////                        }
////                        break;

////                    case PacketType.MessageHistoryReceived:
////                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
////                        if (messages != null)
////                        {
////                            LoadMessageHistory(messages);
////                        }
////                        break;

////                    case PacketType.ClientStatusChanged:
////                        try
////                        {
////                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
////                            if (statusResponse != null)
////                            {
////                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
////                            }
////                        }
////                        catch { }
////                        break;
////                }
////            }
////            catch (Exception ex)
////            {
////                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
////            }
////        }

////        // Добавление сообщения в UI
////        private void AddMessageToUI(MessageResponse message)
////        {
////            var messageModel = new MessageModel
////            {
////                Username = message.SenderName,
////                Message = message.Text,
////                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
////                IsMine = false,
////                Sender = new UserModel { Id = message.FromClientId, Username = message.SenderName }
////            };

////            Messages.Add(messageModel);

////            // Прокрутка вниз
////            if (MessagesList.Items.Count > 0)
////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////        }

////        // Загрузка истории сообщений
////        private void LoadMessageHistory(List<MessageResponse> messages)
////        {
////            Messages.Clear();

////            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
////            {
////                Messages.Add(new MessageModel
////                {
////                    Username = msg.SenderName,
////                    Message = msg.Text,
////                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
////                    IsMine = msg.FromClientId == _userId,
////                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
////                });
////            }

////            if (MessagesList.Items.Count > 0)
////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
////        }

////        // Запрос истории при старте
////        private async void LoadMessageHistory()
////        {
////            try
////            {
////                await _chatClient.GetAllMessages();
////            }
////            catch (Exception ex)
////            {
////                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
////            }
////        }

////        // Обновление статуса пользователя
////        private void UpdateUserStatus(int clientId, bool isOnline)
////        {
////            var user = Users.FirstOrDefault(u => u.Id == clientId);
////            if (user != null)
////            {
////                user.IsOnline = isOnline;
////                user.Status = isOnline ? "Online" : "Offline";
////                UsersList?.Items.Refresh();
////            }
////        }

////        // Отправка сообщения
////        private async void SendButton_Click(object sender, RoutedEventArgs e)
////        {
////            // Получаем текст, удаляя лишние пробелы в начале и конце
////            string messageText = MessageTextBox.Text?.Trim();

////            if (string.IsNullOrWhiteSpace(messageText))
////                return;

////            string time = DateTime.Now.ToString("HH:mm");

////            // Добавляем своё сообщение в UI сразу
////            var myMessage = new MessageModel
////            {
////                Username = _userName,
////                Message = messageText,
////                Time = time,
////                IsMine = true,
////                Sender = CurrentUser,
////                SenderAvatar = CurrentUser?.AvatarPath
////            };

////            Messages.Add(myMessage);

////            // Прокрутка вниз
////            if (MessagesList.Items.Count > 0)
////                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

////            // Очищаем поле ввода
////            MessageTextBox.Clear();

////            // Отправляем на сервер
////            try
////            {
////                await _chatClient.SendMessageAsync(_userId, messageText);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
////                    MessageBoxButton.OK, MessageBoxImage.Error);

////                // Удаляем сообщение, если не отправилось
////                Messages.Remove(myMessage);
////            }
////        }

////        private void SetupTimers()
////        {
////            typingTimer = new DispatcherTimer
////            {
////                Interval = TimeSpan.FromSeconds(1.5)
////            };

////            typingTimer.Tick += (s, e) =>
////            {
////                TypingText.Text = "";
////                typingTimer.Stop();
////            };

////            onlineTimer = new DispatcherTimer
////            {
////                Interval = TimeSpan.FromSeconds(3)
////            };

////            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
////            onlineTimer.Start();
////        }

////        private void TouchUser()
////        {
////            if (CurrentUser != null)
////            {
////                CurrentUser.LastSeen = DateTime.Now;
////                CurrentUser.IsOnline = true;
////            }
////        }

////        private void UpdateOnlineStatus()
////        {
////            foreach (var user in Users)
////            {
////                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
////            }
////            UsersList?.Items.Refresh();
////        }

////        private void FileButton_Click(object sender, RoutedEventArgs e)
////        {
////            TouchUser();
////            OpenFileDialog dlg = new OpenFileDialog()
////            {
////                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
////                Title = "Выберите файл"
////            };

////            if (dlg.ShowDialog() == true)
////            {
////                Messages.Add(new MessageModel
////                {
////                    Username = CurrentUser?.Username ?? _userName,
////                    Message = "📎 Файл",
////                    FilePath = dlg.FileName,
////                    IsMine = true,
////                    Sender = CurrentUser,
////                    Time = DateTime.Now.ToShortTimeString(),
////                    SenderAvatar = CurrentUser?.AvatarPath
////                });
////            }
////        }

////        private void EmojiButton_Click(object sender, RoutedEventArgs e)
////        {
////            MessageTextBox.Text += " 😊";
////            MessageTextBox.Focus();
////            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
////        }

////        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
////        {
////            if (CurrentUser != null)
////            {
////                TypingText.Text = $"{CurrentUser.Username} печатает...";
////                typingTimer.Stop();
////                typingTimer.Start();
////            }
////        }

////        public void ApplyTheme(string theme)
////        {
////            switch (theme)
////            {
////                case "Purple":
////                    Background = new System.Windows.Media.SolidColorBrush(
////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
////                    break;
////                case "NeonBlue":
////                    Background = new System.Windows.Media.SolidColorBrush(
////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
////                    break;
////                case "SpaceBlack":
////                    Background = System.Windows.Media.Brushes.Black;
////                    break;
////                default:
////                    Background = new System.Windows.Media.SolidColorBrush(
////                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
////                    break;
////            }
////        }

////        public void SyncUser(UserModel updated)
////        {
////            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
////            if (user == null) return;

////            user.Username = updated.Username;
////            user.Status = updated.Status;
////            user.About = updated.About;
////            user.AvatarPath = updated.AvatarPath;
////            user.Theme = updated.Theme;
////            user.LastSeen = DateTime.Now;
////            user.IsOnline = true;

////            UsersList?.Items.Refresh();

////            if (updated.Username == CurrentUser?.Username)
////            {
////                CurrentUser = user;
////                ApplyTheme(user.Theme);
////                RefreshMessagesAvatars();
////            }
////        }

////        public void RefreshMessagesAvatars()
////        {
////            foreach (var msg in Messages)
////            {
////                if (msg.Username == CurrentUser?.Username)
////                {
////                    msg.SenderAvatar = CurrentUser.AvatarPath;
////                }
////            }
////            MessagesList?.Items.Refresh();
////        }

////        private void ProfileButton_Click(object sender, RoutedEventArgs e)
////        {
////            if (CurrentUser != null)
////            {
////                var w = new ProfileWindow(CurrentUser);
////                w.ShowDialog();
////            }
////        }

////        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
////        {
////            if (UsersList.SelectedItem is UserModel u)
////                new ProfileWindow(u).ShowDialog();
////        }

////        private void ExitButton_Click(object sender, RoutedEventArgs e)
////        {
////            _chatClient?.Disconnect();
////            Application.Current.Shutdown();
////        }

////        protected override void OnClosed(EventArgs e)
////        {
////            _chatClient?.Disconnect();
////            base.OnClosed(e);
////        }
////    }
////}
//using Microsoft.Win32;
//using SpaceTcpChat.Models;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text.Json;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Threading;
//using WpfApp1.Models;

//namespace WpfApp1.Views
//{
//    public partial class MainWindow : Window
//    {
//        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
//        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

//        private UserModel CurrentUser;
//        private DispatcherTimer typingTimer;
//        private DispatcherTimer onlineTimer;

//        private readonly ChatClient _chatClient;
//        private readonly int _userId;
//        private readonly string _userName;

//        // Конструктор
//        public MainWindow(ChatClient chatClient, int userId, string userName)
//        {
//            InitializeComponent();
//            DataContext = this;

//            _chatClient = chatClient;
//            _userId = userId;
//            _userName = userName;

//            UsersList.ItemsSource = Users;
//            MessagesList.ItemsSource = Messages;

//            // Подписываемся на событие PreviewKeyDown для перехвата Enter до стандартной обработки
//            MessageTextBox.PreviewKeyDown += MessageTextBox_PreviewKeyDown;

//            SetupTimers();
//            LoadUser();

//            // Запускаем приём сообщений
//            StartReceivingMessages();

//            // Загружаем историю
//            LoadMessageHistory();

//            // Устанавливаем фокус на поле ввода при загрузке
//            Loaded += (s, e) => MessageTextBox.Focus();
//        }

//        /// <summary>
//        /// Загрузка текущего пользователя
//        /// </summary>
//        private void LoadUser()
//        {
//            CurrentUser = new UserModel
//            {
//                Id = _userId,
//                Username = _userName,
//                Status = "Online",
//                IpAddress = "127.0.0.1",
//                IsOnline = true,
//                LastSeen = DateTime.Now,
//                Theme = "Dark",
//                AvatarPath = "/Images/default_avatar.png",
//                About = "Привет! Я в Space Chat"
//            };

//            Users.Add(CurrentUser);
//        }

//        /// <summary>
//        /// Обработчик PreviewKeyDown - перехватывает Enter до стандартной обработки
//        /// </summary>
//        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
//        {
//            // Проверяем нажатие Enter
//            if (e.Key == Key.Enter)
//            {
//                // Проверяем, зажат ли Shift
//                bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

//                if (!isShiftPressed)
//                {
//                    // Enter без Shift - отправляем сообщение
//                    e.Handled = true; // Блокируем стандартную обработку

//                    if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
//                    {
//                        SendButton_Click(sender, null);
//                    }
//                }
//                // Если Shift+Enter - разрешаем стандартную обработку (перенос строки)
//                // e.Handled = false; - это значение по умолчанию
//            }
//        }

//        // Старый обработчик KeyDown оставляем для других клавиш
//        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
//        {
//            // Здесь можно обрабатывать другие клавиши, если нужно
//            // Enter обрабатывается в PreviewKeyDown
//        }

//        // Приём сообщений от сервера
//        private async void StartReceivingMessages()
//        {
//            try
//            {
//                while (_chatClient.IsConnected())
//                {
//                    Packet packet = await _chatClient.ReceivePacketAsync();

//                    if (packet == null)
//                        continue;

//                    // Обновляем UI в потоке UI
//                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
//                }
//            }
//            catch (Exception ex)
//            {
//                await Dispatcher.InvokeAsync(() =>
//                {
//                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}\nСоединение будет закрыто.",
//                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//                });
//            }
//        }

//        // Обработка пакетов от сервера
//        private void ProcessPacket(Packet packet)
//        {
//            try
//            {
//                switch (packet.Type)
//                {
//                    case PacketType.MessageReceived:
//                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//                        if (messageResponse != null && messageResponse.FromClientId != _userId)
//                        {
//                            AddMessageToUI(messageResponse);
//                        }
//                        break;

//                    case PacketType.MessageAdded:
//                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//                        if (confirmResponse?.Success == true)
//                        {
//                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
//                        }
//                        break;

//                    case PacketType.MessageHistoryReceived:
//                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//                        if (messages != null)
//                        {
//                            LoadMessageHistory(messages);
//                        }
//                        break;

//                    case PacketType.ClientStatusChanged:
//                        try
//                        {
//                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//                            if (statusResponse != null)
//                            {
//                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//                            }
//                        }
//                        catch { }
//                        break;
//                    case PacketType.ClientList:
//                        try
//                        {
//                            var clients = JsonSerializer.Deserialize<List<ClientResponse>>(packet.Data.GetRawText());
//                            if (clients != null)
//                            {
//                                LoadClients(clients);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientList error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientUpdated:
//                        try
//                        {
//                            var updatedClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (updatedClient != null)
//                            {
//                                UpdateClientInUI(updatedClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientUpdated error: {ex.Message}");
//                        }
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
//            }
//        }
//        private void LoadClients(List<ClientResponse> clients)
//        {
//            Users.Clear();

//            foreach (var client in clients)
//            {
//                var user = new UserModel
//                {
//                    Id = client.Id,
//                    Username = client.Name,
//                    AvatarBytes = client.Avatar, // Сохраняем байты аватара
//                    IsOnline = client.IsOnline,
//                    Status = client.IsOnline ? "Online" : "Offline",
//                    IpAddress = "127.0.0.1",
//                    LastSeen = DateTime.Now,
//                    Theme = "Dark",
//                    About = ""
//                };
//                Users.Add(user);
//            }

//            // Добавляем текущего пользователя, если его нет в списке
//            if (!Users.Any(u => u.Id == _userId))
//            {
//                Users.Add(CurrentUser);
//            }

//            UsersList?.Items.Refresh();
//        }

//        private void UpdateClientInUI(ClientResponse client)
//        {
//            var user = Users.FirstOrDefault(u => u.Id == client.Id);
//            if (user != null)
//            {
//                user.Username = client.Name;
//                user.AvatarBytes = client.Avatar;
//                user.IsOnline = client.IsOnline;
//                user.Status = client.IsOnline ? "Online" : "Offline";

//                UsersList?.Items.Refresh();
//            }
//        }
//        // Добавление сообщения в UI
//        private void AddMessageToUI(MessageResponse message)
//        {
//            var sender = Users.FirstOrDefault(u => u.Id == message.FromClientId);

//            var messageModel = new MessageModel
//            {
//                Username = message.SenderName,
//                Message = message.Text,
//                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
//                IsMine = message.FromClientId == _userId,
//                Sender = sender ?? new UserModel { Id = message.FromClientId, Username = message.SenderName },
//                SenderAvatar = sender?.AvatarBytes != null ? Convert.ToBase64String(sender.AvatarBytes) : null
//            };

//            Messages.Add(messageModel);

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//        }

//        // Загрузка истории сообщений
//        private void LoadMessageHistory(List<MessageResponse> messages)
//        {
//            Messages.Clear();

//            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
//            {
//                Messages.Add(new MessageModel
//                {
//                    Username = msg.SenderName,
//                    Message = msg.Text,
//                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
//                    IsMine = msg.FromClientId == _userId,
//                    Sender = new UserModel { Id = msg.FromClientId, Username = msg.SenderName }
//                });
//            }

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//        }

//        // Запрос истории при старте
//        private async void LoadMessageHistory()
//        {
//            try
//            {
//                await _chatClient.GetAllMessages();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
//            }
//        }

//        // Обновление статуса пользователя
//        private void UpdateUserStatus(int clientId, bool isOnline)
//        {
//            var user = Users.FirstOrDefault(u => u.Id == clientId);
//            if (user != null)
//            {
//                user.IsOnline = isOnline;
//                user.Status = isOnline ? "Online" : "Offline";
//                UsersList?.Items.Refresh();
//            }
//        }

//        // Отправка сообщения
//        private async void SendButton_Click(object sender, RoutedEventArgs e)
//        {
//            string messageText = MessageTextBox.Text?.Trim();

//            if (string.IsNullOrWhiteSpace(messageText))
//                return;

//            string time = DateTime.Now.ToString("HH:mm");

//            var myMessage = new MessageModel
//            {
//                Username = _userName,
//                Message = messageText,
//                Time = time,
//                IsMine = true,
//                Sender = CurrentUser,
//                SenderAvatar = CurrentUser?.AvatarBytes != null ? Convert.ToBase64String(CurrentUser.AvatarBytes) : null
//            };

//            Messages.Add(myMessage);

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

//            MessageTextBox.Clear();

//            try
//            {
//                await _chatClient.SendMessageAsync(_userId, messageText);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//                Messages.Remove(myMessage);
//            }
//        }

//        private void SetupTimers()
//        {
//            typingTimer = new DispatcherTimer
//            {
//                Interval = TimeSpan.FromSeconds(1.5)
//            };

//            typingTimer.Tick += (s, e) =>
//            {
//                TypingText.Text = "";
//                typingTimer.Stop();
//            };

//            onlineTimer = new DispatcherTimer
//            {
//                Interval = TimeSpan.FromSeconds(3)
//            };

//            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
//            onlineTimer.Start();
//        }

//        private void TouchUser()
//        {
//            if (CurrentUser != null)
//            {
//                CurrentUser.LastSeen = DateTime.Now;
//                CurrentUser.IsOnline = true;
//            }
//        }

//        private void UpdateOnlineStatus()
//        {
//            foreach (var user in Users)
//            {
//                user.IsOnline = (DateTime.Now - user.LastSeen).TotalSeconds < 8;
//            }
//            UsersList?.Items.Refresh();
//        }

//        private void FileButton_Click(object sender, RoutedEventArgs e)
//        {
//            TouchUser();
//            OpenFileDialog dlg = new OpenFileDialog()
//            {
//                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
//                Title = "Выберите файл"
//            };

//            if (dlg.ShowDialog() == true)
//            {
//                Messages.Add(new MessageModel
//                {
//                    Username = CurrentUser?.Username ?? _userName,
//                    Message = "📎 Файл",
//                    FilePath = dlg.FileName,
//                    IsMine = true,
//                    Sender = CurrentUser,
//                    Time = DateTime.Now.ToShortTimeString(),
//                    SenderAvatar = CurrentUser?.AvatarBytes != null ? Convert.ToBase64String(CurrentUser.AvatarBytes) : null
//                });
//            }
//        }

//        private void EmojiButton_Click(object sender, RoutedEventArgs e)
//        {
//            MessageTextBox.Text += " 😊";
//            MessageTextBox.Focus();
//            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
//        }

//        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//        {
//            if (CurrentUser != null)
//            {
//                TypingText.Text = $"{CurrentUser.Username} печатает...";
//                typingTimer.Stop();
//                typingTimer.Start();
//            }
//        }

//        public void ApplyTheme(string theme)
//        {
//            switch (theme)
//            {
//                case "Purple":
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
//                    break;
//                case "NeonBlue":
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
//                    break;
//                case "SpaceBlack":
//                    Background = System.Windows.Media.Brushes.Black;
//                    break;
//                default:
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
//                    break;
//            }
//        }

//        public void SyncUser(UserModel updated)
//        {
//            var user = Users.FirstOrDefault(u => u.Username == updated.Username);
//            if (user == null) return;

//            user.Username = updated.Username;
//            user.Status = updated.Status;
//            user.About = updated.About;
//            user.AvatarPath = updated.AvatarPath;
//            user.Theme = updated.Theme;
//            user.LastSeen = DateTime.Now;
//            user.IsOnline = true;
//            user.AvatarBytes = updated.AvatarBytes;  // ДОБАВИТЬ

//            UsersList?.Items.Refresh();

//            if (updated.Username == CurrentUser?.Username)
//            {
//                CurrentUser = user;
//                ApplyTheme(user.Theme);
//                RefreshMessagesAvatars();
//            }
//        }

//        public void RefreshMessagesAvatars()
//        {
//            foreach (var msg in Messages)
//            {
//                if (msg.Username == CurrentUser?.Username)
//                {
//                    msg.SenderAvatar = CurrentUser?.AvatarBytes != null ? Convert.ToBase64String(CurrentUser.AvatarBytes) : null;
//                }
//            }
//            MessagesList?.Items.Refresh();
//        }

//        /// <summary>
//        /// Открытие профиля
//        /// </summary>
//        private void ProfileButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (CurrentUser != null)
//                {
//                    // Передаем _chatClient в ProfileWindow
//                    var profileWindow = new ProfileWindow(CurrentUser, _chatClient);
//                    profileWindow.Owner = this;
//                    profileWindow.ShowDialog();

//                    UsersList?.Items.Refresh();
//                    MessagesList?.Items.Refresh();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при открытии профиля: {ex.Message}",
//                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            try
//            {
//                if (UsersList.SelectedItem is UserModel u)
//                {
//                    var profileWindow = new ProfileWindow(u, _chatClient);
//                    profileWindow.Owner = this;
//                    profileWindow.ShowDialog();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        private void ExitButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                _chatClient?.Disconnect();
//            }
//            catch { }
//            Application.Current.Shutdown();
//        }

//        protected override void OnClosed(EventArgs e)
//        {
//            try
//            {
//                _chatClient?.Disconnect();
//            }
//            catch { }
//            base.OnClosed(e);
//        }
//    }
//}





































using Microsoft.Win32;
using SpaceTcpChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApp1.Models;
using System.Windows.Media.Imaging;

namespace WpfApp1.Views
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

        private UserModel CurrentUser;
        private DispatcherTimer typingTimer;
        private DispatcherTimer onlineTimer;

        private readonly ChatClient _chatClient;
        private readonly Guid _userId;
        private readonly string _userName;

        public MainWindow(ChatClient chatClient, Guid userId, string userName)
        {
            InitializeComponent();
            DataContext = this;

            _chatClient = chatClient;
            _userId = userId;
            _userName = userName;

            UsersList.ItemsSource = Users;
            MessagesList.ItemsSource = Messages;

            MessageTextBox.PreviewKeyDown += MessageTextBox_PreviewKeyDown;

            SetupTimers();
            LoadUser();

            StartReceivingMessages();
            LoadMessageHistory();
            LoadAllClients(); 
            

            Loaded += (s, e) => MessageTextBox.Focus();
        }

        private void LoadUser()
        {
            CurrentUser = new UserModel
            {
                Id = _userId,
                Username = _userName,
                Status = "Online",
                IpAddress = "127.0.0.1",
                IsOnline = true,
                LastSeen = DateTime.Now,
                Theme = "Dark",
                AvatarPath = "/Images/default_avatar.png",
                About = "Привет! Я в Space Chat"
            };

            Users.Add(CurrentUser);
        }

        // ЗАГРУЗКА ВСЕХ КЛИЕНТОВ
        private async void LoadAllClients()
        {
            try
            {
                if (_chatClient.IsConnected())
                {
                    await _chatClient.GetAllClientsAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

                if (!isShiftPressed)
                {
                    e.Handled = true;

                    if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
                    {
                        SendButton_Click(sender, null);
                    }
                }
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Обработка других клавиш
        }

        private async void StartReceivingMessages()
        {
            try
            {
                while (_chatClient.IsConnected())
                {
                    Packet packet = await _chatClient.ReceivePacketAsync();

                    if (packet == null)
                        continue;

                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

       

        private void ProcessPacket(Packet packet)
        {
            try
            {
                if (packet == null)
                    return;

                if (packet.Data.ValueKind == JsonValueKind.Undefined ||
                    packet.Data.ValueKind == JsonValueKind.Null)
                {
                    System.Diagnostics.Debug.WriteLine($"Empty data: {packet.Type}");
                    return;
                }

                switch (packet.Type)
                {
                    case PacketType.MessageReceived:
                        {
                            var messageResponse = packet.Data.Deserialize<MessageResponse>();
                            if (messageResponse != null)
                                AddMessageToUI(messageResponse);
                            break;
                        }

                    case PacketType.MessageAdded:  // ← ИСПРАВЛЕНО: Добавлен обработчик
                        {
                            var confirmResponse = packet.Data.Deserialize<BaseResponse>();
                            if (confirmResponse?.Success == true)
                            {
                                System.Diagnostics.Debug.WriteLine("Сообщение успешно отправлено");
                            }
                            break;
                        }

                    case PacketType.MessageHistoryReceived:
                        {
                            var messages = packet.Data.Deserialize<List<MessageResponse>>();
                            if (messages != null)
                                LoadMessageHistory(messages);
                            break;
                        }

                    case PacketType.ClientStatusChanged:  // ← ИСПРАВЛЕНО: Добавлен обработчик
                        {
                            var statusResponse = packet.Data.Deserialize<ClientStatusResponse>();
                            if (statusResponse != null)
                            {
                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
                            }
                            break;
                        }

                    case PacketType.ClientList:  // ← ИСПРАВЛЕНО: Добавлен обработчик
                        {
                            var clients = packet.Data.Deserialize<List<ClientResponse>>();
                            if (clients != null)
                                LoadClients(clients);
                            break;
                        }
                    case PacketType.ClientRegistered:  // ← ИСПРАВЛЕНО: Добавлен обработчик
                        {
                            var newClient = packet.Data.Deserialize<ClientResponse>();
                            if (newClient != null)
                                AddClientToUI(newClient);
                            break;
                        }

                    case PacketType.ClientLogged:  // ← ИСПРАВЛЕНО: Добавлен обработчик
                        {
                            var loggedClient = packet.Data.Deserialize<ClientResponse>();
                            if (loggedClient != null)
                                UpdateClientInUI(loggedClient);
                            break;
                        }

                    case PacketType.ClientUpdated:
                        {
                            var updatedClient = packet.Data.Deserialize<ClientResponse>();
                            if (updatedClient != null)
                                UpdateClientInUI(updatedClient);
                            break;

                        }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex}");
            }
        }
        private void LoadClients(List<ClientResponse> clients)
        {
            Users.Clear();

            foreach (var client in clients)
            {
                var user = new UserModel
                {
                    Id = client.Id,
                    Username = client.Name,
                    AvatarBytes = client.Avatar,
                    IsOnline = client.IsOnline,
                    Status = client.IsOnline ? "Online" : "Offline",
                    IpAddress = "127.0.0.1",
                    LastSeen = DateTime.Now,
                    Theme = "Dark",
                    About = ""
                };
                Users.Add(user);
            }

            // Добавляем текущего пользователя, если его нет в списке
            if (!Users.Any(u => u.Id == _userId))
            {
                Users.Add(CurrentUser);
            }

            UsersList?.Items.Refresh();
        }

        private void AddClientToUI(ClientResponse client)
        {
            // Проверяем, есть ли уже такой пользователь
            if (Users.Any(u => u.Id == client.Id))
                return;

            var user = new UserModel
            {
                Id = client.Id,
                Username = client.Name,
                AvatarBytes = client.Avatar,
                IsOnline = client.IsOnline,
                Status = client.IsOnline ? "Online" : "Offline",
                IpAddress = "127.0.0.1",
                LastSeen = DateTime.Now,
                Theme = "Dark",
                About = ""
            };

            Users.Add(user);
            UsersList?.Items.Refresh();
        }

        private void UpdateClientInUI(ClientResponse client)
        {
            var user = Users.FirstOrDefault(u => u.Id == client.Id);
            if (user != null)
            {
                user.Username = client.Name;
                user.AvatarBytes = client.Avatar;
             //   MessageBox.Show(
             //    client.Avatar == null
             //? "Avatar NULL"
             //: $"Avatar bytes: {client.Avatar.Length}"
             //  );
                user.IsOnline = client.IsOnline;
                user.Status = client.IsOnline ? "Online" : "Offline";

                UsersList?.Items.Refresh();
            }
        }

        // ИСПРАВЛЕННОЕ ДОБАВЛЕНИЕ СООБЩЕНИЯ С АВАТАРОМ
        private void AddMessageToUI(MessageResponse message)
        {
            // Ищем отправителя в списке пользователей
            var sender = Users.FirstOrDefault(u => u.Id == message.FromClientId);

            // Если отправитель не найден - создаем временного
            if (sender == null)
            {
                sender = new UserModel
                {
                    Id = message.FromClientId,
                    Username = message.SenderName
                };
            }

            // Получаем аватар в правильном формате для отображения
            string avatarBase64 = null;
            if (sender.AvatarBytes != null && sender.AvatarBytes.Length > 0)
            {
                avatarBase64 = Convert.ToBase64String(sender.AvatarBytes);
            }

            var messageModel = new MessageModel
            {
                Username = message.SenderName,
                Message = message.Text,
                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
                IsMine = message.FromClientId == _userId,
                Sender = sender,
                SenderAvatarBytes = sender?.AvatarBytes
            };

            Messages.Add(messageModel);

            if (MessagesList.Items.Count > 0)
                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
        }

        private void LoadMessageHistory(List<MessageResponse> messages)
        {
            Messages.Clear();

            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
            {
                // Ищем отправителя
                var sender = Users.FirstOrDefault(u => u.Id == msg.FromClientId);
                string avatarBase64 = null;

                if (sender?.AvatarBytes != null && sender.AvatarBytes.Length > 0)
                {
                    avatarBase64 = Convert.ToBase64String(sender.AvatarBytes);
                }

                Messages.Add(new MessageModel
                {
                    Username = msg.SenderName,
                    Message = msg.Text,
                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
                    IsMine = msg.FromClientId == _userId,
                    Sender = sender ?? new UserModel { Id = msg.FromClientId, Username = msg.SenderName },
                    SenderAvatarBytes = sender?.AvatarBytes
                });
            }

            if (MessagesList.Items.Count > 0)
                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
        }

        private async void LoadMessageHistory()
        {
            try
            {
                await _chatClient.GetAllMessages();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
            }
        }

        private void UpdateUserStatus(Guid clientId, bool isOnline)
        {
            var user = Users.FirstOrDefault(u => u.Id == clientId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.Status = isOnline ? "Online" : "Offline";
                UsersList?.Items.Refresh();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageTextBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(messageText))
                return;

            // Создаем сообщение для UI
            string avatarBase64 = null;
            if (CurrentUser?.AvatarBytes != null && CurrentUser.AvatarBytes.Length > 0)
            {
                avatarBase64 = Convert.ToBase64String(CurrentUser.AvatarBytes);
            }

            var myMessage = new MessageModel
            {
                Username = _userName,
                Message = messageText,
                Time = DateTime.Now.ToString("HH:mm"),
                IsMine = true,
                Sender = CurrentUser,
                SenderAvatarBytes = CurrentUser?.AvatarBytes
            };

            Messages.Add(myMessage);

            if (MessagesList.Items.Count > 0)
                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

            MessageTextBox.Clear();

            try
            {
                await _chatClient.SendMessageAsync(_userId, messageText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Messages.Remove(myMessage);
            }
        }

        private void SetupTimers()
        {
            typingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };

            typingTimer.Tick += (s, e) =>
            {
                TypingText.Text = "";
                typingTimer.Stop();
            };

            onlineTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            onlineTimer.Tick += (s, e) => UpdateOnlineStatus();
            onlineTimer.Start();
        }

        private void UpdateOnlineStatus()
        {
            foreach (var user in Users)
            {
                // Проверяем онлайн статус через сервер
                if (user.Id != _userId)
                {
                    // Статус уже обновляется через ClientStatusChanged от сервера
                }
            }
            UsersList?.Items.Refresh();
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
                Title = "Выберите файл"
            };

            if (dlg.ShowDialog() == true)
            {
                string avatarBase64 = null;
                if (CurrentUser?.AvatarBytes != null && CurrentUser.AvatarBytes.Length > 0)
                {
                    avatarBase64 = Convert.ToBase64String(CurrentUser.AvatarBytes);
                }

                Messages.Add(new MessageModel
                {
                    Username = CurrentUser?.Username ?? _userName,
                    Message = "📎 Файл",
                    FilePath = dlg.FileName,
                    IsMine = true,
                    Sender = CurrentUser,
                    Time = DateTime.Now.ToShortTimeString(),
                    SenderAvatarBytes = CurrentUser?.AvatarBytes
                });
            }
        }

        private void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += " 😊";
            MessageTextBox.Focus();
            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
        }

        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (CurrentUser != null)
            {
                TypingText.Text = $"{CurrentUser.Username} печатает...";
                typingTimer.Stop();
                typingTimer.Start();
            }
        }

        public void ApplyTheme(string theme)
        {
            switch (theme)
            {
                case "Purple":
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
                    break;
                case "NeonBlue":
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
                    break;
                case "SpaceBlack":
                    Background = System.Windows.Media.Brushes.Black;
                    break;
                default:
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
                    break;
            }
        }

        public void SyncUser(UserModel updated)
        {
            var user = Users.FirstOrDefault(u => u.Id == updated.Id);
            if (user == null) return;

            user.Username = updated.Username;
            user.Status = updated.Status;
            user.About = updated.About;
            user.AvatarPath = updated.AvatarPath;
            user.Theme = updated.Theme;
            user.LastSeen = DateTime.Now;
            user.IsOnline = true;
            user.AvatarBytes = updated.AvatarBytes;

            UsersList?.Items.Refresh();

            if (updated.Id == CurrentUser?.Id)
            {
                CurrentUser = user;
                ApplyTheme(user.Theme);
                RefreshMessagesAvatars();
            }
        }

        public void RefreshMessagesAvatars()
        {
            foreach (var msg in Messages)
            {
                if (msg.Sender?.Id == CurrentUser?.Id)
                {
                    msg.SenderAvatarBytes = CurrentUser?.AvatarBytes;
                }
            }
            MessagesList?.Items.Refresh();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentUser != null)
                {
                    var profileWindow = new ProfileWindow(CurrentUser, _chatClient);
                    profileWindow.Owner = this;
                    profileWindow.ShowDialog();

                    UsersList?.Items.Refresh();
                    MessagesList?.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии профиля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (UsersList.SelectedItem is UserModel u)
                {
                    var profileWindow = new ProfileWindow(u, _chatClient);
                    profileWindow.Owner = this;
                    profileWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _chatClient?.Disconnect();
            }
            catch { }
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                _chatClient?.Disconnect();
            }
            catch { }
            base.OnClosed(e);
        }
    }
}












//using Microsoft.Win32;
//using SpaceTcpChat.Models;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text.Json;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Threading;
//using WpfApp1.Models;
//using System.Windows.Media.Imaging;

//namespace WpfApp1.Views
//{
//    public partial class MainWindow : Window
//    {
//        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
//        public ObservableCollection<MessageModel> Messages = new ObservableCollection<MessageModel>();

//        private UserModel CurrentUser;
//        private DispatcherTimer typingTimer;
//        private DispatcherTimer refreshUsersTimer; // Таймер для обновления списка пользователей

//        private readonly ChatClient _chatClient;
//        private readonly int _userId;
//        private readonly string _userName;

//        public MainWindow(ChatClient chatClient, int userId, string userName)
//        {
//            InitializeComponent();
//            DataContext = this;

//            _chatClient = chatClient;
//            _userId = userId;
//            _userName = userName;

//            UsersList.ItemsSource = Users;
//            MessagesList.ItemsSource = Messages;

//            MessageTextBox.PreviewKeyDown += MessageTextBox_PreviewKeyDown;

//            SetupTimers();
//            LoadUser();

//            StartReceivingMessages();
//            LoadMessageHistory();

//            // Загружаем список пользователей сразу после подключения
//            LoadAllClients();

//            Loaded += (s, e) => MessageTextBox.Focus();
//        }

//        private void LoadUser()
//        {
//            CurrentUser = new UserModel
//            {
//                Id = _userId,
//                Username = _userName,
//                Status = "Online",
//                IpAddress = "127.0.0.1",
//                IsOnline = true,
//                LastSeen = DateTime.Now,
//                Theme = "Dark",
//                AvatarPath = "/Images/default_avatar.png",
//                About = "Привет! Я в Space Chat"
//            };

//            Users.Add(CurrentUser);
//        }

//        private async void LoadAllClients()
//        {
//            try
//            {
//                if (_chatClient.IsConnected())
//                {
//                    await _chatClient.GetAllClientsAsync();
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки клиентов: {ex.Message}");
//            }
//        }

//        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.Key == Key.Enter)
//            {
//                bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

//                if (!isShiftPressed)
//                {
//                    e.Handled = true;

//                    if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
//                    {
//                        SendButton_Click(sender, null);
//                    }
//                }
//            }
//        }

//        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
//        {
//            // Обработка других клавиш
//        }

//        private async void StartReceivingMessages()
//        {
//            try
//            {
//                while (_chatClient.IsConnected())
//                {
//                    Packet packet = await _chatClient.ReceivePacketAsync();

//                    if (packet == null)
//                        continue;

//                    await Dispatcher.InvokeAsync(() => ProcessPacket(packet));
//                }
//            }
//            catch (Exception ex)
//            {
//                await Dispatcher.InvokeAsync(() =>
//                {
//                    MessageBox.Show($"Ошибка приёма сообщений: {ex.Message}",
//                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//                });
//            }
//        }

//        private void ProcessPacket(Packet packet)
//        {
//            try
//            {
//                switch (packet.Type)
//                {
//                    case PacketType.MessageReceived:
//                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//                        if (messageResponse != null)
//                        {
//                            AddMessageToUI(messageResponse);
//                        }
//                        break;

//                    case PacketType.MessageAdded:
//                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//                        if (confirmResponse?.Success == true)
//                        {
//                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
//                        }
//                        break;

//                    case PacketType.MessageHistoryReceived:
//                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//                        if (messages != null)
//                        {
//                            LoadMessageHistory(messages);
//                        }
//                        break;

//                    case PacketType.ClientStatusChanged:
//                        try
//                        {
//                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//                            if (statusResponse != null)
//                            {
//                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//                            }
//                        }
//                        catch { }
//                        break;

//                    case PacketType.ClientList:
//                        try
//                        {
//                            var clients = JsonSerializer.Deserialize<List<ClientResponse>>(packet.Data.GetRawText());
//                            if (clients != null)
//                            {
//                                LoadClients(clients);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientList error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientUpdated:
//                        try
//                        {
//                            var updatedClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (updatedClient != null)
//                            {
//                                UpdateClientInUI(updatedClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientUpdated error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientRegistered:
//                        try
//                        {
//                            var newClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (newClient != null)
//                            {
//                                AddClientToUI(newClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientRegistered error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientLogged:
//                        try
//                        {
//                            var loggedClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (loggedClient != null)
//                            {
//                                UpdateClientInUI(loggedClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientLogged error: {ex.Message}");
//                        }
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
//            }
//        }

//        private void LoadClients(List<ClientResponse> clients)
//        {
//            // Сохраняем текущего пользователя
//            var currentUser = Users.FirstOrDefault(u => u.Id == _userId);

//            Users.Clear();

//            foreach (var client in clients)
//            {
//                var user = new UserModel
//                {
//                    Id = client.Id,
//                    Username = client.Name,
//                    AvatarBytes = client.Avatar,
//                    IsOnline = client.IsOnline,
//                    Status = client.IsOnline ? "Online" : "Offline",
//                    IpAddress = "127.0.0.1",
//                    LastSeen = DateTime.Now,
//                    Theme = "Dark",
//                    About = ""
//                };
//                Users.Add(user);
//            }

//            // Добавляем текущего пользователя, если его нет в списке
//            if (!Users.Any(u => u.Id == _userId))
//            {
//                if (currentUser != null)
//                {
//                    Users.Add(currentUser);
//                }
//                else
//                {
//                    Users.Add(CurrentUser);
//                }
//            }

//            UsersList?.Items.Refresh();
//        }

//        private void AddClientToUI(ClientResponse client)
//        {
//            // Проверяем, есть ли уже такой пользователь
//            if (Users.Any(u => u.Id == client.Id))
//                return;

//            var user = new UserModel
//            {
//                Id = client.Id,
//                Username = client.Name,
//                AvatarBytes = client.Avatar,
//                IsOnline = client.IsOnline,
//                Status = client.IsOnline ? "Online" : "Offline",
//                IpAddress = "127.0.0.1",
//                LastSeen = DateTime.Now,
//                Theme = "Dark",
//                About = ""
//            };

//            Users.Add(user);
//            UsersList?.Items.Refresh();
//        }

//        private void UpdateClientInUI(ClientResponse client)
//        {
//            var user = Users.FirstOrDefault(u => u.Id == client.Id);
//            if (user != null)
//            {
//                user.Username = client.Name;
//                user.AvatarBytes = client.Avatar;
//                user.IsOnline = client.IsOnline;
//                user.Status = client.IsOnline ? "Online" : "Offline";

//                UsersList?.Items.Refresh();
//            }
//        }

//        private void AddMessageToUI(MessageResponse message)
//        {
//            var sender = Users.FirstOrDefault(u => u.Id == message.FromClientId);

//            if (sender == null)
//            {
//                sender = new UserModel
//                {
//                    Id = message.FromClientId,
//                    Username = message.SenderName
//                };
//            }

//            string avatarBase64 = null;
//            if (sender.AvatarBytes != null && sender.AvatarBytes.Length > 0)
//            {
//                avatarBase64 = Convert.ToBase64String(sender.AvatarBytes);
//            }

//            var messageModel = new MessageModel
//            {
//                //Id = message.Id,
//                Username = message.SenderName,
//                Message = message.Text,
//                Time = message.CreatedAt.ToLocalTime().ToString("HH:mm"),
//                IsMine = message.FromClientId == _userId,
//                Sender = sender,
//                SenderAvatar = avatarBase64
//            };

//            Messages.Add(messageModel);

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//        }

//        private void LoadMessageHistory(List<MessageResponse> messages)
//        {
//            Messages.Clear();

//            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
//            {
//                var sender = Users.FirstOrDefault(u => u.Id == msg.FromClientId);
//                string avatarBase64 = null;

//                if (sender?.AvatarBytes != null && sender.AvatarBytes.Length > 0)
//                {
//                    avatarBase64 = Convert.ToBase64String(sender.AvatarBytes);
//                }

//                Messages.Add(new MessageModel
//                {
//                    //Id = msg.Id,
//                    Username = msg.SenderName,
//                    Message = msg.Text,
//                    Time = msg.CreatedAt.ToLocalTime().ToString("HH:mm"),
//                    IsMine = msg.FromClientId == _userId,
//                    Sender = sender ?? new UserModel { Id = msg.FromClientId, Username = msg.SenderName },
//                    SenderAvatar = avatarBase64
//                });
//            }

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
//        }

//        private async void LoadMessageHistory()
//        {
//            try
//            {
//                await _chatClient.GetAllMessages();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
//            }
//        }

//        private void UpdateUserStatus(int clientId, bool isOnline)
//        {
//            var user = Users.FirstOrDefault(u => u.Id == clientId);
//            if (user != null)
//            {
//                user.IsOnline = isOnline;
//                user.Status = isOnline ? "Online" : "Offline";
//                UsersList?.Items.Refresh();
//            }
//        }

//        private async void SendButton_Click(object sender, RoutedEventArgs e)
//        {
//            string messageText = MessageTextBox.Text?.Trim();

//            if (string.IsNullOrWhiteSpace(messageText))
//                return;

//            string avatarBase64 = null;
//            if (CurrentUser?.AvatarBytes != null && CurrentUser.AvatarBytes.Length > 0)
//            {
//                avatarBase64 = Convert.ToBase64String(CurrentUser.AvatarBytes);
//            }

//            var myMessage = new MessageModel
//            {
//                Username = _userName,
//                Message = messageText,
//                Time = DateTime.Now.ToString("HH:mm"),
//                IsMine = true,
//                Sender = CurrentUser,
//                SenderAvatar = avatarBase64
//            };

//            Messages.Add(myMessage);

//            if (MessagesList.Items.Count > 0)
//                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);

//            MessageTextBox.Clear();

//            try
//            {
//                await _chatClient.SendMessageAsync(_userId, messageText);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//                Messages.Remove(myMessage);
//            }
//        }

//        private void SetupTimers()
//        {
//            typingTimer = new DispatcherTimer
//            {
//                Interval = TimeSpan.FromSeconds(1.5)
//            };

//            typingTimer.Tick += (s, e) =>
//            {
//                TypingText.Text = "";
//                typingTimer.Stop();
//            };

//            // ТАЙМЕР ДЛЯ ОБНОВЛЕНИЯ СПИСКА ПОЛЬЗОВАТЕЛЕЙ - КАЖДЫЕ 3 СЕКУНДЫ
//            refreshUsersTimer = new DispatcherTimer
//            {
//                Interval = TimeSpan.FromSeconds(3)
//            };

//            refreshUsersTimer.Tick += async (s, e) =>
//            {
//                if (_chatClient.IsConnected())
//                {
//                    await _chatClient.GetAllClientsAsync();
//                }
//            };
//            refreshUsersTimer.Start();
//        }

//        private void FileButton_Click(object sender, RoutedEventArgs e)
//        {
//            OpenFileDialog dlg = new OpenFileDialog()
//            {
//                Filter = "Images|*.png;*.jpg;*.jpeg|All Files|*.*",
//                Title = "Выберите файл"
//            };

//            if (dlg.ShowDialog() == true)
//            {
//                string avatarBase64 = null;
//                if (CurrentUser?.AvatarBytes != null && CurrentUser.AvatarBytes.Length > 0)
//                {
//                    avatarBase64 = Convert.ToBase64String(CurrentUser.AvatarBytes);
//                }

//                Messages.Add(new MessageModel
//                {
//                    Username = CurrentUser?.Username ?? _userName,
//                    Message = "📎 Файл",
//                    FilePath = dlg.FileName,
//                    IsMine = true,
//                    Sender = CurrentUser,
//                    Time = DateTime.Now.ToShortTimeString(),
//                    SenderAvatar = avatarBase64
//                });
//            }
//        }

//        private void EmojiButton_Click(object sender, RoutedEventArgs e)
//        {
//            MessageTextBox.Text += " 😊";
//            MessageTextBox.Focus();
//            MessageTextBox.CaretIndex = MessageTextBox.Text.Length;
//        }

//        private void MessageTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//        {
//            if (CurrentUser != null)
//            {
//                TypingText.Text = $"{CurrentUser.Username} печатает...";
//                typingTimer.Stop();
//                typingTimer.Start();
//            }
//        }

//        public void ApplyTheme(string theme)
//        {
//            switch (theme)
//            {
//                case "Purple":
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2B0A3D"));
//                    break;
//                case "NeonBlue":
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#001B2E"));
//                    break;
//                case "SpaceBlack":
//                    Background = System.Windows.Media.Brushes.Black;
//                    break;
//                default:
//                    Background = new System.Windows.Media.SolidColorBrush(
//                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0A0A1A"));
//                    break;
//            }
//        }

//        public void SyncUser(UserModel updated)
//        {
//            var user = Users.FirstOrDefault(u => u.Id == updated.Id);
//            if (user == null) return;

//            user.Username = updated.Username;
//            user.Status = updated.Status;
//            user.About = updated.About;
//            user.AvatarPath = updated.AvatarPath;
//            user.Theme = updated.Theme;
//            user.LastSeen = DateTime.Now;
//            user.IsOnline = true;
//            user.AvatarBytes = updated.AvatarBytes;

//            UsersList?.Items.Refresh();

//            if (updated.Id == CurrentUser?.Id)
//            {
//                CurrentUser = user;
//                ApplyTheme(user.Theme);
//                RefreshMessagesAvatars();
//            }
//        }

//        public void RefreshMessagesAvatars()
//        {
//            foreach (var msg in Messages)
//            {
//                if (msg.Sender?.Id == CurrentUser?.Id)
//                {
//                    msg.SenderAvatar = CurrentUser?.AvatarBytes != null ?
//                        Convert.ToBase64String(CurrentUser.AvatarBytes) : null;
//                }
//            }
//            MessagesList?.Items.Refresh();
//        }

//        private void ProfileButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (CurrentUser != null)
//                {
//                    var profileWindow = new ProfileWindow(CurrentUser, _chatClient);
//                    profileWindow.Owner = this;
//                    profileWindow.ShowDialog();

//                    UsersList?.Items.Refresh();
//                    MessagesList?.Items.Refresh();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при открытии профиля: {ex.Message}",
//                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            try
//            {
//                if (UsersList.SelectedItem is UserModel u)
//                {
//                    var profileWindow = new ProfileWindow(u, _chatClient);
//                    profileWindow.Owner = this;
//                    profileWindow.ShowDialog();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void ExitButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                refreshUsersTimer?.Stop(); // Останавливаем таймер
//                _chatClient?.Disconnect();
//            }
//            catch { }
//            Application.Current.Shutdown();
//        }

//        protected override void OnClosed(EventArgs e)
//        {
//            try
//            {
//                refreshUsersTimer?.Stop();
//                _chatClient?.Disconnect();
//            }
//            catch { }
//            base.OnClosed(e);
//        }
//    }
//}


//        private void ProcessPacket(Packet packet)
//        {
//            try
//            {
//                switch (packet.Type)
//                {
//                    case PacketType.MessageReceived:
//                        var messageResponse = JsonSerializer.Deserialize<MessageResponse>(packet.Data.GetRawText());
//                        if (messageResponse != null)
//                        {
//                            AddMessageToUI(messageResponse);
//                        }
//                        break;

//                    case PacketType.MessageAdded:
//                        var confirmResponse = JsonSerializer.Deserialize<BaseResponse>(packet.Data.GetRawText());
//                        if (confirmResponse?.Success == true)
//                        {
//                            System.Diagnostics.Debug.WriteLine("Message sent successfully");
//                        }
//                        break;

//                    case PacketType.MessageHistoryReceived:
//                        var messages = JsonSerializer.Deserialize<List<MessageResponse>>(packet.Data.GetRawText());
//                        if (messages != null)
//                        {
//                            LoadMessageHistory(messages);
//                        }
//                        break;

//                    case PacketType.ClientStatusChanged:
//                        try
//                        {
//                            var statusResponse = JsonSerializer.Deserialize<ClientStatusResponse>(packet.Data.GetRawText());
//                            if (statusResponse != null)
//                            {
//                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
//                            }
//                        }
//                        catch { }
//                        break;

//                    case PacketType.ClientList:
//                        try
//                        {
//                            var clients = JsonSerializer.Deserialize<List<ClientResponse>>(packet.Data.GetRawText());
//                            if (clients != null)
//                            {
//                                LoadClients(clients);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientList error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientUpdated:

//                        MessageBox.Show("Происходит");
//                        try
//                        {

//                            var updatedClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            MessageBox.Show(
//    updatedClient?.Avatar == null
//        ? "Avatar NULL"
//        : $"Avatar bytes = {updatedClient.Avatar.Length}"
//);
//                            if (updatedClient != null)
//                            {
//                                UpdateClientInUI(updatedClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientUpdated error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientRegistered:
//                        try
//                        {
//                            var newClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (newClient != null)
//                            {
//                                AddClientToUI(newClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientRegistered error: {ex.Message}");
//                        }
//                        break;

//                    case PacketType.ClientLogged:
//                        try
//                        {
//                            var loggedClient = JsonSerializer.Deserialize<ClientResponse>(packet.Data.GetRawText());
//                            if (loggedClient != null)
//                            {
//                                UpdateClientInUI(loggedClient);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            System.Diagnostics.Debug.WriteLine($"ClientLogged error: {ex.Message}");
//                        }
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
//            }
//        }
