using System;

namespace SpaceTcpChat.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }

        public bool IsMine { get; set; }

        public UserModel Sender { get; set; }

        
        public byte[] SenderAvatarBytes { get; set; }
        public bool IsFile { get; set; }
        public bool IsImage { get; set; }
        public string FilePath { get; set; }
    }
}