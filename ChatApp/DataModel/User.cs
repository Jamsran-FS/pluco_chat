using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ChatApp.DataModel
{
    public class User : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public SolidColorBrush Color { get; set; }
        private bool IsActive_ { get; set; }
        public bool IsActive
        {
            get { return IsActive_; }
            set
            {
                IsActive_ = value;
                if (PropertyChanged != null) { OnPropertyChanged(new PropertyChangedEventArgs("IsActive")); }
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
