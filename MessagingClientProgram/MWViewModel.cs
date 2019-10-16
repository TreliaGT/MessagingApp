using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessagingClientProgram
{
    class MWViewModel : INotifyPropertyChanged
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        private string _username;
        public string Username
        {
            get { return _username; }
            set { OnPropertyChanged(ref _username, value); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { OnPropertyChanged(ref _address, value); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { OnPropertyChanged(ref _port, value); }
        }


        private string _message;
        public string Message
        {
            get { return _message; }
            set { OnPropertyChanged(ref _message, value); }
        }

        public List<string> messages;
       

        public void Connect()
        {
            messages.Add("Connecting to Chat Server ..");
          
            clientSocket.Connect(_address, Convert.ToInt32(_port));

            byte[] outStream = Encoding.ASCII.GetBytes(_username + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            //Thread ctThread = new Thread();
           // ctThread.Start();
        }


        public void Send()
        {
       
        }

        public void Disconnect()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool OnPropertyChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
