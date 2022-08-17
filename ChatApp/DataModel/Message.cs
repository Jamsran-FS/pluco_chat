using System;
using System.Collections.Generic;

namespace ChatApp.DataModel
{
    public class Message
    {
        public string ID { get; set; }
        public User User { get; set; }
        public string DateTime { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageSource { get; set; }
        public string ReplyMessageSource { get; set; }
        public List<int> Emojis { get; set; } = new List<int>();
    }
}
