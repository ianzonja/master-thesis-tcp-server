using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Message
    {
        public string Text { get; set; }

        public string AuthorPlayfabId { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
    }
}
