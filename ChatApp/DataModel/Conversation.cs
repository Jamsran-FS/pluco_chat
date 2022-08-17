using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChatApp.DataModel
{
    public class Conversation : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public DateTime StartDate { get; set; }
        private ObservableCollection<User> Participants_ { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<User> Participants
        {
            get { return Participants_; }
            set
            {
                Participants_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("Participants")); }
            }
        }
        private ObservableCollection<Message> Messages_ { get; set; } = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get { return Messages_; }
            set
            {
                Messages_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("Messages")); }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, e);
        }
    }
}
