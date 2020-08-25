using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestConnectionTool.Models
{
    public class DataBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool isDummy { get; private set; } = false;

        private string _ServerName = null;
        public string ServerName
        {
            get { return _ServerName; }
            set
            {
                if (_ServerName != value)
                {
                    _ServerName = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(FullInstanceName));
                }
            }
        }

        private string _InstanceName = null;
        public string InstanceName
        {
            get { return _InstanceName; }
            set
            {
                if (_InstanceName != value)
                {
                    _InstanceName = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(FullInstanceName));
                }
            }
        }

        private string _UserSA = null;
        public string UserSA
        {
            get { return _UserSA; }
            set
            {
                if (_UserSA != value)
                {
                    _UserSA = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _CryptedPasswordSA = null;
        public string CryptedPasswordSA
        {
            get { return _CryptedPasswordSA; }
            set
            {
                if (_CryptedPasswordSA != value)
                {
                    _CryptedPasswordSA = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Version _Version = null;
        public Version Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Version _KL2_DataBaseVersion = null;
        public Version KL2_DataBaseVersion
        {
            get { return _KL2_DataBaseVersion; }
            set
            {
                if (_KL2_DataBaseVersion != value)
                {
                    _KL2_DataBaseVersion = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string FullInstanceName =>
            isDummy ? ToString() : $"{this}{(KL2_DataBaseVersion == null ? string.Empty : $" (v{KL2_DataBaseVersion})")}";

        public DataBase(bool dummy = false)
        {
            isDummy = dummy;
        }

        public override string ToString() =>
            isDummy
            ? "<Manual>"
            : $@"{ServerName}\{InstanceName}";
    }
}
