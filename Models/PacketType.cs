namespace WpfApp1.Models
{
    public enum PacketType
    {
        Registration,
        Login,
        SendMessage,
        GetAllMessages,
        MessageReceived,
        MessageAdded,
        MessageHistoryReceived,
        ClientStatusChanged,
        GetAllClients,
        ClientLogged,
        ClientRegistered,
        UpdateClient,
        ClientUpdated,
        ClientList,
        ClientDeleted,
        DeleteMessage,
        MessageDeleted
    }
}