using ChatApp.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Http;
using Newtonsoft.Json;
using ChatApp.Classes;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using SocketIOClient;
using System.Configuration;
using System.Linq;
using System.IO;

namespace ChatApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SocketIO SocketIO { get; } = new SocketIO(Functions.ServerURL);
        public List<LanguageCode> LanguageCodes { get; set; } = new List<LanguageCode>();
        public User SelectedUser { get; set; }
        public ChatRoom SelectedChatRoom { get; set; }
        private ObservableCollection<User> Users_ { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get { return Users_; }
            set
            {
                Users_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("Users")); }
            }
        }
        private ObservableCollection<ChatRoom> ChatRooms_ { get; set; } = new ObservableCollection<ChatRoom>();
        public ObservableCollection<ChatRoom> ChatRooms
        {
            get { return ChatRooms_; }
            set
            {
                ChatRooms_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("ChatRooms")); }
            }
        }
        public Dictionary<string, Conversation> Conversations { get; set; } = new Dictionary<string, Conversation>();
        private ObservableCollection<Message> Conversation_ { get; set; } = new ObservableCollection<Message>();
        public ObservableCollection<Message> Conversation
        {
            get { return Conversation_; }
            set
            {
                Conversation_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("Conversation")); }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, e);
        }

        public MainWindow()
        {
            InitializeComponent();

            Title = $"{ Title } - { ConfigurationManager.AppSettings["email"] }";
            Functions.GenerateColorList();
            Functions.HttpClient.DefaultRequestHeaders.Add("authorization", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1tcyIsImlhdCI6MTY1NDQwNzMzOX0.9UGy-vHWu9uHCm48xCKV02pz0nHZ870rwlr5cilm4qU");

            GetUsers();
            GetChatRooms();
            GenerateConversation();
            GetTranslationLanguage();
            _ = ListenServerResponseAsync();

            ConversationItemsControl.ItemsSource = null;
            ConversationItemsControl.ItemsSource = Conversation;
        }

        private void GetTranslationLanguage()
        {
            using (StreamReader r = new StreamReader("Languages.json"))
            {
                string json = r.ReadToEnd();
                LanguageCodes = JsonConvert.DeserializeObject<List<LanguageCode>>(json);
            }

            foreach (LanguageCode languageCode in LanguageCodes)
            {
                CbTargetLanguages.Items.Add(languageCode.Language);
            }
            string selectedLanguage = LanguageCodes.Find(x => x.Code == ConfigurationManager.AppSettings["targetLanguage"]).Language;
            CbTargetLanguages.SelectedItem = selectedLanguage;
            LblTargetLanguage.Text = $"Detect language->{ selectedLanguage }";
        }

        private void GenerateConversation()
        {
            /*foreach (var user in Users)
            {
                Conversation conversation = new Conversation();
                conversation.Participants.Add(user);
                for (int i = 0; i < user.ID; i++)
                {
                    conversation.Messages.Add(new Message()
                    {
                        User = user,
                        DateTime = DateTime.Now,
                        MessageType = MessageType.Text,
                        MessageSource = "on Monday, after the weekend in Europe - the first thing I find out is if this name is taken yes, have a nice weekend and good mood"
                    });
                }
                Conversations.Add(user.UserName, conversation);
            }*/
        }

        private async void GetUsers()
        {
            // Logged in user
            Users.Add(new User()
            {
                ID = ConfigurationManager.AppSettings["userID"].ToString(),
                UserName = ConfigurationManager.AppSettings["username"].ToString(),
                Email = ConfigurationManager.AppSettings["email"].ToString(),
                FirstName = ConfigurationManager.AppSettings["firstname"].ToString(),
                LastName = ConfigurationManager.AppSettings["lastname"].ToString(),
                FullName = $"{ ConfigurationManager.AppSettings["firstname"] } { ConfigurationManager.AppSettings["lastname"] }",
                IsActive = true,
                Color = Functions.ColorList[0]
            });

            // All other users
            var reqBody = new Dictionary<string, string>
            {
                { "userId", Users.First().ID }
            };
            var response = await Functions.HttpClient.PostAsync($"{ Functions.ServerURL }chat", new FormUrlEncodedContent(reqBody));
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            int colorIndex = 1;
            foreach (var user in json.Property("otherusers").Values())
            {
                var userJson = JObject.Parse(user.ToString());
                string firstname = userJson.Property("Firstname").Value.ToString();
                string lastname = userJson.Property("Lastname").Value.ToString();
                Users.Add(new User()
                {
                    ID = userJson.Property("_id").Value.ToString(),
                    UserName = userJson.Property("UserName").Value.ToString(),
                    Email = userJson.Property("Email").Value.ToString(),
                    FirstName = firstname,
                    LastName = lastname,
                    FullName = $"{ firstname } { lastname }",
                    IsActive = bool.Parse(userJson.Property("IsActive").Value.ToString()),
                    Color = Functions.ColorList[colorIndex]
                });
                colorIndex++;
            }

            UserItemsControl.ItemsSource = null;
            UserItemsControl.ItemsSource = Users;

            (Functions.FindItemControl(UserItemsControl, "BtnUser", Users.FirstOrDefault()) as Button).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        }
        private void GetChatRooms()
        {
            /*ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-1", MemberCount = 36 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-2", MemberCount = 24 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-3", MemberCount = 10 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-4", MemberCount = 4 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-5", MemberCount = 18 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-6", MemberCount = 55 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-7", MemberCount = 9 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-8", MemberCount = 12 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-9", MemberCount = 3 });
            ChatRooms.Add(new ChatRoom() { RoomName = "ChatRoom-10", MemberCount = 32 });*/
            ChatRoomItemsControl.ItemsSource = null;
            ChatRoomItemsControl.ItemsSource = ChatRooms;
        }

        private void BtnChatRoom_Click(object sender, RoutedEventArgs e)
        {
            SelectedChatRoom = (sender as Button).Tag as ChatRoom;
            LblChatTitle.Text = SelectedChatRoom.RoomName;
            LblChatType.Text = "Group chat";
            LblIsActive.Visibility = Visibility.Collapsed;
            (BtnRoomMembers.Content as TextBlock).Text = $"{ SelectedChatRoom.MemberCount } members";
            BtnRoomMembers.Visibility = Visibility.Visible;
        }

        private async void BtnUser_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser = (sender as Button).Tag as User;
            LblChatTitle.Text = SelectedUser.FullName;
            LblChatType.Text = "Direct message";
            LblIsActive.Visibility = Visibility.Visible;
            BtnRoomMembers.Visibility = Visibility.Collapsed;

            var reqBody = new Dictionary<string, string>
            {
                { "fromUserId", ConfigurationManager.AppSettings["userID"].ToString() },
                { "toUserId", SelectedUser.ID }
            };
            var response = await Functions.HttpClient.PostAsync($"{ Functions.ServerURL }chat/getconversation", new FormUrlEncodedContent(reqBody));
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);

            if (json.Property("success").Value.ToString().ToLower().Equals("true"))
            {
                string loggedInUserId = ConfigurationManager.AppSettings["userID"].ToString();
                Conversation = new ObservableCollection<Message>();
                foreach (var message in json.Property("conversation").Values())
                {
                    var messageJson = JObject.Parse(message.ToString());

                    if ((!loggedInUserId.Equals(SelectedUser.ID) && !messageJson.Property("fromUserId").Value.ToString().Equals(messageJson.Property("toUserId").Value.ToString())) || (loggedInUserId.Equals(SelectedUser.ID) && messageJson.Property("fromUserId").Value.ToString().Equals(messageJson.Property("toUserId").Value.ToString())))
                    {
                        Conversation.Add(new Message()
                        {
                            ID = messageJson.Property("_id").Value.ToString(),
                            User = Users.ToList().Find(x => x.ID == messageJson.Property("fromUserId").Value.ToString()),
                            DateTime = messageJson.Property("sendDateTime").Value.ToString(),
                            MessageType = MessageType.Text,
                            MessageSource = messageJson.Property("message").Value.ToString()
                        });
                    }
                }
            }

            ConversationItemsControl.ItemsSource = null;
            ConversationItemsControl.ItemsSource = Conversation;

            if (SelectedUser.IsActive)
            {
                LblIsActive.Text = "online";
                LblIsActive.Foreground = new SolidColorBrush(Color.FromRgb(43, 172, 118));
            }
            else
            {
                LblIsActive.Text = "offline";
                LblIsActive.Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117));
            }
        }

        private void BtnChatOnly_Click(object sender, RoutedEventArgs e)
        {
            TextInputGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
            TextInputGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            LblTranslationLanguage.Visibility = Visibility.Collapsed;
        }

        private void BtnChatWithTranslate_Click(object sender, RoutedEventArgs e)
        {
            TextInputGrid.ColumnDefinitions[0].Width = new GridLength(48, GridUnitType.Star);
            TextInputGrid.ColumnDefinitions[1].Width = new GridLength(52, GridUnitType.Star);
            LblTranslationLanguage.Visibility = Visibility.Visible;
        }

        private void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMention_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEmoji_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnTextFormating_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVoiceMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Translate()
        {
            var reqBody = new Dictionary<string, string>
            {
                { "text", Functions.StringFromRichTextBox(TbTranslationInput)},
                { "target", ConfigurationManager.AppSettings["targetLanguage"].ToString() }
            };
            var response = await Functions.HttpClient.PostAsync($"{ Functions.ServerURL }translate", new FormUrlEncodedContent(reqBody));
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            TbChatInput.AppendText(json.Property("result").Value[0].ToString());
        }

        private void BtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            Translate();
        }

        private void TbTranslationInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter: Translate(); break;
                default: break;
            }
        }

        public async Task ListenServerResponseAsync()
        {
            SocketIO.OnConnected += (sender, e) =>
            {
                Console.WriteLine("SocketIO connected...");
                _ = SocketIO.EmitAsync("login", ConfigurationManager.AppSettings["userID"]);
            };
            SocketIO.On("directMessage", response =>
            {
                string responseString = response.ToString().TrimStart('[').TrimEnd(']');
                JObject json = JObject.Parse(responseString);
                string msg = json.Property("result").Value.ToString();
                string fromUserId = json.Property("fromUserId").Value.ToString();
                _ = ConversationItemsControl.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Conversation.Add(new Message()
                    {
                        User = Users.ToList().Find(x => x.ID == fromUserId),
                        DateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"),
                        MessageType = MessageType.Text,
                        MessageSource = msg
                    });
                }));
                _ = ChatMessageScroller.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ChatMessageScroller.ScrollToEnd();
                }));
            });
            SocketIO.On("login", response =>
            {
                string responseString = response.ToString().TrimStart('[').TrimEnd(']');
                JObject json = JObject.Parse(responseString);
                string userID = json.Property("result").Value.ToString();

                if (!userID.Equals(ConfigurationManager.AppSettings["userID"]))
                {
                    _ = UserItemsControl.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Users.ToList().Find(x => x.ID == userID).IsActive = true;
                    }));

                    if (SelectedUser.ID.Equals(userID))
                    {
                        _ = LblIsActive.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            LblIsActive.Text = "online";
                            LblIsActive.Foreground = new SolidColorBrush(Color.FromRgb(43, 172, 118));
                        }));
                    }
                }
            });
            SocketIO.On("logout", response =>
            {
                string responseString = response.ToString().TrimStart('[').TrimEnd(']');
                JObject json = JObject.Parse(responseString);
                string userID = json.Property("result").Value.ToString();
                if (!userID.Equals(ConfigurationManager.AppSettings["userID"]))
                {
                    _ = UserItemsControl.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Users.ToList().Find(x => x.ID == userID).IsActive = false;
                    }));

                    if (SelectedUser.ID.Equals(userID))
                    {
                        _ = LblIsActive.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            LblIsActive.Text = "offline";
                            LblIsActive.Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117));
                        }));
                    }
                }
            });
            await SocketIO.ConnectAsync();
        }

        private void SendMessage()
        {
            string message = Functions.StringFromRichTextBox(TbChatInput);
            SocketIO.EmitAsync("directMessage", new object[] { message, SelectedUser.ID, DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") });
            TbChatInput.Document.Blocks.Clear();
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void TbChatInput_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TbChatInput_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter: SendMessage(); break;
                default: break;
            }
        }

        private void BtnTranslationLanguage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStartVideoCall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStartVoiceCall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnWorkspace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUserSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMemberRequest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCreateChatRoom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void LanguageSelectionPopup_LostFocus(object sender, RoutedEventArgs e)
        {
            //LanguageSelectionPopup.IsOpen = false;
        }

        private void BtnSelectLanguage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point a = e.GetPosition(this);
            LanguageSelectionPopup.HorizontalOffset = a.X + 20;
            LanguageSelectionPopup.VerticalOffset = a.Y + 20;
            LanguageSelectionPopup.IsOpen = true;
        }

        private void BtnChangeTranslationLang_Click(object sender, RoutedEventArgs e)
        {
            string selectedLanguage = CbTargetLanguages.SelectedItem.ToString();
            Functions.UpdateSetting("targetLanguage", LanguageCodes.Find(x => x.Language == selectedLanguage).Code);
            LblTargetLanguage.Text = $"Detect language->{ selectedLanguage }";
            LanguageSelectionPopup.IsOpen = false;
        }
    }
}
