using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
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
       

        public string Message
        {
            get { return _message; }
            set { OnPropertyChanged(ref _message, value); }
        }

        public List<string> MessagesList;



        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public void Connect()
        {
            try
            {
                MessagesList.Add("Connecting...");
                clientSocket.Connect(_address, Convert.ToInt32(_port));

                byte[] outStream = Encoding.ASCII.GetBytes(_username + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void getMessage()
        {
            while (true)
            {
                /* serverStream = clientSocket.GetStream();
                 int buffSize = 0;
                 byte[] inStream = new byte[30000];
                 buffSize = clientSocket.ReceiveBufferSize;
                 serverStream.Read(inStream, 0, buffSize);
                 string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                 MessagesList.Add(returndata);*/
                ReceiveData(clientSocket);

            }
        }

        public void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[102400];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                MessagesList.Add(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }



        public void Send()
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
