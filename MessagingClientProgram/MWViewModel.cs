using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;


namespace MessagingClientProgram
{
    class MWViewModel
    {
       public CancellationTokenSource source = new CancellationTokenSource();
   
        public ClientWebSocket WebSocket = new ClientWebSocket();

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

        private string _port = "8000";
        public string Port
        {
            get { return _port; }
            set { OnPropertyChanged(ref _port, value); }
        }


        private string _message;
        private WebSocket ws;

        public string Message
        {
            get { return _message; }
            set { OnPropertyChanged(ref _message, value); }
        }

        private ClientWebSocket client;
        Uri host;

        public void Connect()
        {
            try
            {
                host = new Uri(_address);
                CancellationToken token = source.Token;
                client.ConnectAsync(host, token);
                byte[] Message = Encoding.ASCII.GetBytes(_username + " : Has Connected");
                ArraySegment<byte> Message2 = new ArraySegment<byte>(Message);
                client.SendAsync(Message2, 0, true, token);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

   

    public void Send()
        {
            CancellationToken token = source.Token;
            byte[] Message = Encoding.ASCII.GetBytes(_username + " : " + _message);
            ArraySegment<byte> Message2 = new ArraySegment<byte>(Message);
            client.SendAsync(Message2, 0, true, token);
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
