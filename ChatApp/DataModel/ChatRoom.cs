using System;
using System.Collections.Generic;

namespace ChatApp.DataModel
{
    public class ChatRoom
    {
        public int ID { get; set; }
        public string RoomName { get; set; }
        public User Admin { get; set; }
        public int MemberCount { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<User> Members { get; set; } = new List<User>();
    }
}
