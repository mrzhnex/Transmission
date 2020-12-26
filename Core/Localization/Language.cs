using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Core.Localization
{
    [Serializable]
    public class Language : INotifyPropertyChanged
    {
        [XmlIgnore]
        public static Language Default { get; private set; } = new Language("ru", "русский");
        public string Culture { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        internal Language(string Culture, string Name)
        {
            this.Culture = Culture;
            this.Name = Name;
        }
        public Language() { }

        public void CompareToNew(Language language)
        {
            Culture = language.Culture;
            Name = language.Name;
            Connect = language.Connect;
            Server = language.Server;
            Password = language.Password;
        }

        #region Upper strings
        public string Connect
        {
            get { return connect; }
            set
            {
                connect = value;
                OnPropertyChanged(nameof(Connect));
            }
        }
        public string Server
        {
            get { return server; }
            set
            {
                server = value;
                OnPropertyChanged(nameof(Server));
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        #endregion

        #region Lower strings
        private string connect = "подключиться";
        private string server = "сервер";
        private string password = "пароль";
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}