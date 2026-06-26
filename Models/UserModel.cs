using System;

namespace SpaceTcpChat.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }  
        public string Username { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public string Theme { get; set; } = "Dark";
        public string AvatarPath { get; set; } = string.Empty;
        public byte[] AvatarBytes { get; set; }
        public string About { get; set; } = string.Empty;
    }
}