using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ChatWin.xaml
    /// </summary>
    public partial class ChatWin : Window
    {
        private readonly ClassForServerWork _serverClient;
        private readonly int _userId;
        private readonly string _userName;
        private  bool _isOnline;
        public ChatWin(ClassForServerWork serverClient, int userId, string userName)
        {
            InitializeComponent();
            _serverClient = serverClient;
            _userId = userId;
            _userName = userName;
            _isOnline = true;
            Loaded += ChatWin_Loaded;
        }
        private void ChatWin_Loaded(object sender, RoutedEventArgs e)
        {
            lblStatus.Content =
                $"{_userName} {(_isOnline ? "Онлайн" : "Оффлайн")}";
        }

        protected override void OnClosed(EventArgs e)
        {
            _serverClient.Dispose();
            base.OnClosed(e);
        }
    }
}
