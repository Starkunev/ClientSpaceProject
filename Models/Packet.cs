using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.Text.Json;

namespace WpfApp1.Models
{
    public class Packet
    {
        public PacketType Type { get; set; }
        public JsonElement Data { get; set; }
    }
}
