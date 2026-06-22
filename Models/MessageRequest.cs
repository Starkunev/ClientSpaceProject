using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class MessageRequest
    {
        public int FromClientId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
