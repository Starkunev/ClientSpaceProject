using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
    }
}
