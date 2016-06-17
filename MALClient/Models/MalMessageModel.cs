using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{
    public class MalMessageModel
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
    }
}
