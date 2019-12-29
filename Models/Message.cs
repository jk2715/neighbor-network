using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neighbor_Network.Models
{
    public class Message
    {
        public int MsgID { get; set; }

        public int TID { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Coordinates { get; set; }

    }
}
