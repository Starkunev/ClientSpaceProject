using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class ClientStatusResponse
    {
        public Guid ClientId { get; set; }
        public bool IsOnline { get; set; }
    }
}