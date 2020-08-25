using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UploadUriToServer
{
    public class ResourceFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisedPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static string HashPropertyName = "Hash";
        public static string UriPropertyName = "Uri";

        string _tableName;
        public string TableName
        {
            get => _tableName;
            set
            {
                if (_tableName != value)
                {
                    _tableName = value;
                    RaisedPropertyChanged();
                }
            }
        }

        string _idPropertyName;
        public string IdPropertyName
        {
            get => _idPropertyName;
            set
            {
                if (_idPropertyName != value)
                {
                    _idPropertyName = value;
                    RaisedPropertyChanged();
                }
            }
        }

        int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisedPropertyChanged();
                }
            }
        }

        string _uri;
        public string Uri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    RaisedPropertyChanged();
                }
            }
        }

        bool _localFileExists;
        public bool LocalFileExists
        {
            get => _localFileExists;
            set
            {
                if (_localFileExists != value)
                {
                    _localFileExists = value;
                    RaisedPropertyChanged();
                }
            }
        }

        string _hash;
        public string Hash
        {
            get => _hash;
            set
            {
                if (_hash != value)
                {
                    _hash = value;
                    RaisedPropertyChanged();
                }
            }
        }

        bool _copied;
        public bool Copied
        {
            get => _copied;
            set
            {
                if (_copied != value)
                {
                    _copied = value;
                    RaisedPropertyChanged();
                }
            }
        }

        bool _alreadyInBase;
        public bool AlreadyInBase
        {
            get => _alreadyInBase;
            set
            {
                if (_alreadyInBase != value)
                {
                    _alreadyInBase = value;
                    RaisedPropertyChanged();
                }
            }
        }
    }
}
