using Microsoft.Win32;
using SpaceTcpChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfApp1.Models;

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
        private readonly string _userLogin;
        private readonly string _userPassword;

        public class Base64ByteArrayConverter : JsonConverter<byte[]>
        {
            public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string base64 = reader.GetString();
                    if (string.IsNullOrEmpty(base64))
                        return null;
                    try
                    {
                        return Convert.FromBase64String(base64);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }

            public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
            {
                if (value == null)
                    writer.WriteNullValue();
                else
                    writer.WriteStringValue(Convert.ToBase64String(value));
            }
        }
        public MainWindow(ChatClient chatClient, Guid userId, string userName, string login = null, string password = null)
        {
            InitializeComponent();
            DataContext = this;

            _chatClient = chatClient;
            _userPassword = password ?? string.Empty;
            _userId = userId;
            _userName = userName;
            _userLogin = login ?? userName;

            UsersList.ItemsSource = Users;
            MessagesList.ItemsSource = Messages;

            MessageTextBox.PreviewKeyDown += MessageTextBox_PreviewKeyDown;

            SetupTimers();
            LoadUser();

            StartReceivingMessages();

            Loaded += async (s, e) =>
            {
                MessageTextBox.Focus();

                
                if (_chatClient.IsConnected())
                {
                    await Task.Delay(300); 
                    await _chatClient.GetAllClientsAsync();
                    await _chatClient.GetAllMessages();
                }
            };

            Console.WriteLine($"MainWindow создан: UserId={_userId}, UserName={_userName}, UserLogin={_userLogin}");
            
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
                            System.Diagnostics.Debug.WriteLine($"=== Получен MessageReceived ===");
                            System.Diagnostics.Debug.WriteLine($"Raw Data: {packet.Data.GetRawText()}");

                            var messageResponse = packet.Data.Deserialize<MessageResponse>();
                            if (messageResponse != null)
                            {
                               
                                if (!Messages.Any(m => m.Id == messageResponse.Id))
                                {
                                    AddMessageToUI(messageResponse);
                                }
                            }
                            break;
                        }
                    

                    case PacketType.MessageAdded:
                        {
                            System.Diagnostics.Debug.WriteLine($"=== Получен MessageAdded ===");
                            System.Diagnostics.Debug.WriteLine($"Raw Data: {packet.Data.GetRawText()}");

                            try
                            {
                                var json = packet.Data.GetRawText();
                                var doc = JsonDocument.Parse(json);
                                var root = doc.RootElement;

                                
                                if (root.TryGetProperty("baseResponse", out JsonElement baseResp))
                                {
                                    if (baseResp.TryGetProperty("Success", out JsonElement success) && success.GetBoolean())
                                    {
                                        
                                        if (root.TryGetProperty("messageResponse", out JsonElement msgResp))
                                        {
                                            if (msgResp.TryGetProperty("Id", out JsonElement msgId))
                                            {
                                                var realId = msgId.GetGuid();
                                                System.Diagnostics.Debug.WriteLine($"Реальный Id сообщения: {realId}");

                                              
                                                var myMessage = Messages.LastOrDefault(m => m.IsMine);
                                                if (myMessage != null)
                                                {
                                                    myMessage.Id = realId;
                                                    MessagesList?.Items.Refresh();
                                                    System.Diagnostics.Debug.WriteLine($"Id обновлен: {realId}");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                              
                                        var errorMsg = baseResp.TryGetProperty("Message", out JsonElement msg)
                                            ? msg.GetString()
                                            : "Неизвестная ошибка";

                                       
                                        var myMessage = Messages.LastOrDefault(m => m.IsMine);
                                        if (myMessage != null)
                                        {
                                            Messages.Remove(myMessage);
                                        }

                                        MessageBox.Show($"Ошибка отправки: {errorMsg}", "Ошибка",
                                            MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                                else
                                {
                                    
                                    if (root.TryGetProperty("Success", out JsonElement success) && success.GetBoolean())
                                    {
                                        if (root.TryGetProperty("MessageId", out JsonElement msgId))
                                        {
                                            var realId = msgId.GetGuid();
                                            var myMessage = Messages.LastOrDefault(m => m.IsMine);
                                            if (myMessage != null)
                                            {
                                                myMessage.Id = realId;
                                                MessagesList?.Items.Refresh();
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"MessageAdded error: {ex.Message}");
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

                    case PacketType.ClientStatusChanged:
                        {
                            var statusResponse = packet.Data.Deserialize<ClientStatusResponse>();
                            if (statusResponse != null)
                            {
                                UpdateUserStatus(statusResponse.ClientId, statusResponse.IsOnline);
                            }
                            break;
                        }

                    case PacketType.ClientList:
                        {
                            var clients = packet.Data.Deserialize<List<ClientResponse>>();
                            if (clients != null)
                                LoadClients(clients);
                            break;
                        }

                    case PacketType.ClientRegistered:
                        {
                            var newClient = packet.Data.Deserialize<ClientResponse>();
                            if (newClient != null)
                                AddClientToUI(newClient);
                            break;
                        }

                    case PacketType.MessageDeleted:
                        {
                            System.Diagnostics.Debug.WriteLine($"=== Получен MessageDeleted ===");
                            System.Diagnostics.Debug.WriteLine($"Raw Data: {packet.Data.GetRawText()}");

                            try
                            {
                                var json = packet.Data.GetRawText();

                                
                                var guidJson = json.Trim('"');
                                if (Guid.TryParse(guidJson, out Guid messageId))
                                {
                                    System.Diagnostics.Debug.WriteLine($"Удаление сообщения: {messageId}");

                                    Dispatcher.Invoke(() =>
                                    {
                                        var messageToRemove = Messages.FirstOrDefault(m => m.Id == messageId);
                                        if (messageToRemove != null)
                                        {
                                            Messages.Remove(messageToRemove);
                                            System.Diagnostics.Debug.WriteLine($"Сообщение удалено из UI");
                                        }
                                    });
                                }
                                else
                                {
                                    
                                    var errorResponse = JsonSerializer.Deserialize<BaseResponse>(json);
                                    if (errorResponse != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {errorResponse.Message}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"MessageDeleted error: {ex.Message}");
                            }
                            break;
                        }
                    
                  


                    case PacketType.ClientLogged:
                        {
                            System.Diagnostics.Debug.WriteLine($"=== Получен ClientLogged ===");
                            System.Diagnostics.Debug.WriteLine($"Raw Data: {packet.Data.GetRawText()}");

                            var json = packet.Data.GetRawText();

                            if (json.Contains("ClientResponse"))
                            {
                                 var doc = JsonDocument.Parse(json);
                                var root = doc.RootElement;

                                if (root.TryGetProperty("ClientResponse", out JsonElement clientElement))
                                {
                                    System.Diagnostics.Debug.WriteLine($"ClientResponse JSON: {clientElement.GetRawText()}");

                                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                    options.Converters.Add(new Base64ByteArrayConverter());

                                    var loggedClient = JsonSerializer.Deserialize<ClientResponse>(
                                        clientElement.GetRawText(), options);

                                    if (loggedClient != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"ClientLogged: Id={loggedClient.Id}, Name={loggedClient.Name}");
                                        System.Diagnostics.Debug.WriteLine($"CurrentUserId={_userId}, Совпадает: {loggedClient.Id == _userId}");
                                        System.Diagnostics.Debug.WriteLine($"Avatar null: {loggedClient.Avatar == null}, Length: {loggedClient.Avatar?.Length ?? 0}");

                                       
                                        if (loggedClient.Id == _userId && loggedClient.Avatar != null && loggedClient.Avatar.Length > 0)
                                        {
                                            CurrentUser.AvatarBytes = loggedClient.Avatar;
                                            System.Diagnostics.Debug.WriteLine($"✅ Аватар текущего пользователя обновлен: {loggedClient.Avatar.Length} байт");
                                        }

                                        UpdateClientInUI(loggedClient);
                                    }
                                }
                            }
                            break;
                        }
                    case PacketType.ClientUpdated:
                        {
                            System.Diagnostics.Debug.WriteLine($"=== Получен ClientUpdated пакет ===");
                            System.Diagnostics.Debug.WriteLine($"Raw Data: {packet.Data.GetRawText()}");

                            try
                            {
                                var json = packet.Data.GetRawText();

                            
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };
                                options.Converters.Add(new Base64ByteArrayConverter());

                           
                                if (json.Contains("clientProfileResponse"))
                                {
                                    var doc = JsonDocument.Parse(json);
                                    var root = doc.RootElement;

                                    if (root.TryGetProperty("clientProfileResponse", out JsonElement clientProfile))
                                    {
                                        var updatedClient = JsonSerializer.Deserialize<ClientResponse>(
                                            clientProfile.GetRawText(),
                                            options
                                        );

                                        if (updatedClient != null && updatedClient.Id != Guid.Empty)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"ClientUpdated: Id={updatedClient.Id}, Name={updatedClient.Name}");
                                            System.Diagnostics.Debug.WriteLine($"Avatar null: {updatedClient.Avatar == null}, Length: {updatedClient.Avatar?.Length ?? 0}");

                                            UpdateClientInUI(updatedClient);
                                            RefreshMessagesAvatars();
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine("ERROR: Не удалось десериализовать clientProfileResponse");
                                        }
                                    }
                                }
                                else
                                {
                                   
                                    var updatedClient = JsonSerializer.Deserialize<ClientResponse>(json, options);
                                    if (updatedClient != null && updatedClient.Id != Guid.Empty)
                                    {
                                        UpdateClientInUI(updatedClient);
                                        RefreshMessagesAvatars();
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("ERROR: Не удалось десериализовать ClientUpdated");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"ClientUpdated deserialize error: {ex.Message}");
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProcessPacket error: {ex.Message}");
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

               
                if (client.Id == _userId)
                {
                    CurrentUser.AvatarBytes = client.Avatar;
                    CurrentUser.Username = client.Name;
                    System.Diagnostics.Debug.WriteLine($"✅ Аватар текущего пользователя из ClientList: {(client.Avatar?.Length ?? 0)} байт");
                }
            }

           
            if (!Users.Any(u => u.Id == _userId))
            {
                Users.Add(CurrentUser);
            }

            UsersList?.Items.Refresh();
        }


       

        private void AddClientToUI(ClientResponse client)
        {
           
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
            System.Diagnostics.Debug.WriteLine($"=== UpdateClientInUI ===");
            System.Diagnostics.Debug.WriteLine($"Client Id: {client.Id}, Name: {client.Name}");
            System.Diagnostics.Debug.WriteLine($"Avatar null: {client.Avatar == null}, Length: {client.Avatar?.Length ?? 0}");

            var user = Users.FirstOrDefault(u => u.Id == client.Id);
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"Найден пользователь в UI: {user.Username}");

                user.Username = client.Name;

                if (client.Avatar != null && client.Avatar.Length > 0)
                {
                    user.AvatarBytes = client.Avatar;
                    System.Diagnostics.Debug.WriteLine($"Аватар обновлен: {client.Avatar.Length} байт");
                }

               
                if (client.Id == _userId)
                {
                    CurrentUser.AvatarBytes = user.AvatarBytes;
                    CurrentUser.Username = user.Username;
                    System.Diagnostics.Debug.WriteLine($"✅ CurrentUser обновлен");
                }

                UsersList?.Items.Refresh();
                RefreshMessagesAvatars();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Пользователь с Id={client.Id} НЕ НАЙДЕН в списке UI");
            }
        }
        

        private void AddMessageToUI(MessageResponse message)
        {
    
            if (message.FromClientId == _userId)
            {
              
                var existingMsg = Messages.LastOrDefault(m => m.IsMine);
                if (existingMsg != null)
                {
                  
                    existingMsg.Id = message.Id;
                    MessagesList?.Items.Refresh();
                    System.Diagnostics.Debug.WriteLine($"Обновлен ID своего сообщения: {message.Id}");
                }
                else
                {
                    
                    AddNewMessage(message);
                }
                return; 
            }

           
            AddNewMessage(message);
        }

        
        private void AddNewMessage(MessageResponse message)
        {
            var sender = Users.FirstOrDefault(u => u.Id == message.FromClientId);

            if (sender == null)
            {
                sender = new UserModel
                {
                    Id = message.FromClientId,
                    Username = message.SenderName
                };
            }

            var messageModel = new MessageModel
            {
                Id = message.Id,
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

        private async void DeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MessageModel message)
            {
                var result = MessageBox.Show("Удалить сообщение?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        
                        await _chatClient.DeleteMessageAsync(message.Id);

                       
                        Messages.Remove(message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
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

           
            var tempId = Guid.NewGuid();
            var myMessage = new MessageModel
            {
                Id = tempId,
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
              
                Messages.Remove(myMessage);
                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                
                if (user.Id != _userId)
                {
                   
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

      

        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (UsersList.SelectedItem is UserModel u)
                {
            
                    var profileWindow = new ProfileWindow(u);
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

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentUser != null)
                {
                  
                    System.Diagnostics.Debug.WriteLine($"Открытие профиля: Login={_userLogin}, UserName={_userName}");

                    var profileWindow = new ProfileWindow(CurrentUser, _chatClient, _userLogin,_userPassword);
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
