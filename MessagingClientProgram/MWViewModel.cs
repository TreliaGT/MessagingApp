using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace MessagingClientProgram
{
    class MWViewModel //: INotifyPropertyChanged
    {
      
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; NotifyPropertyChanged(); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged(); }
        }


        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged(); }
        }

        public List<string> messages;

        // Used to keep track of the UI Thread
        private SynchronizationContext context;

        // Can be assigned to by other classes when the this class updates.
        public PropertyChangedEventHandler PropertyChanged;​
    // Used to stop a Thread when the client loses connection to the server.
    private bool isCapturingMessages;
​
    private string status;
        /// <summary>
        /// Can be used to keep track of what the Client is doing.
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        private TcpClient clientSocket;
        private NetworkStream serverStream;
​
    // A thread specifically used to capture messages from the server.
    private Thread captureThread;
​
    // The maximum size of the buffer allowed.
    private int bufferSize = 10025;

        public MWViewModel()
        {
            clientSocket = null;
            serverStream = default;
            isCapturingMessages = false;
​
        context = SynchronizationContext.Current;
        }
​


        /// <summary>
        /// Connect to a server using a given address, port, and name.
        /// </summary>
        public void Connect()
        {
        // Might want to do some error trapping here.
        // Regex code: "^\\w+$"
        // And if it fails to validate, either update the status or throw an Exception
​
        // Close any already existing connections
        if (IsConnected())
            {
                Disconnect();
            }
​
        // Create a new connection to the server with the given username.
        Status = string.Format("Establishing a connection ....", _address, _port);
         
            if (clientSocket == null) clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect(_address, Convert.ToInt32(_port));
            }
            catch (SocketException)
            {
                // TODO: Update the Status, and run the Disconnect method
                return;
            }
            serverStream = clientSocket.GetStream();
​
        // Send the username to the server
        byte[] outStream = Encoding.ASCII.GetBytes(_username + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
​
        // TODO: Update the status notifying the Client of the new connection.
​
        // Create a Thread to monitor retrieval of new messages
        isCapturingMessages = true;
            captureThread = new Thread(CaptureMessages);
            captureThread.Start();
        }


        public void Send()
        {
            if (IsConnected())
            {
                byte[] outStream = Encoding.ASCII.GetBytes(_message + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }

        public void Disconnect()
        {
            // Only disconnect when there is no connection
            if (!IsConnected())
                return;
​
        // Set isCapturingMessages to false, this will stop the captureThread
        isCapturingMessages = false;
​
        // Close the stream connected to the server
        serverStream.Close();
            serverStream = default;
​
        // Close the socket connected to the client
        clientSocket.Close();
            clientSocket = null;
​
        // TODO: if the setStatus is set to true, update the status.
        }

        /// <summary>
        /// Ensure that there is an existing connection.
        /// </summary>
        /// <returns>true if there is a connection.</returns>
        public bool IsConnected()
        {
            return (clientSocket != null && clientSocket.Connected);
        }

        /// <summary>
        /// While isCaptureingMessages is true, constantly communicate to the server waiting to recieve messages.
        /// </summary>
        private void CaptureMessages()
        {
            while (isCapturingMessages)
            {
                // Establish a new NetworkStream.
                serverStream = clientSocket.GetStream();
                clientSocket.ReceiveBufferSize = bufferSize;
​
            // Convert data recieved from the NetworkStream to a byte array.
            byte[] inStream = new byte[bufferSize];
                try
                {
                    serverStream.Read(inStream, 0, bufferSize);
                }
                catch (Exception)
                {
                    // If the client cannot get data from the NetworkStream, close the connection.
                    Disconnect();
                    break;
                }
​
            // Get a message from the server
            var inData = Encoding.ASCII.GetString(inStream);
​
            // Check if the message recieved from the server is a command (!<Command>!)
            int firstIndex = inData.IndexOf('!');
                int lastIndex = inData.LastIndexOf('!');
​
            if (firstIndex == 0
                && lastIndex > 0)
                {
                    var message = inData.Substring(1, lastIndex - 1);
​
                if (message.Equals("Username in use"))
                    {
                        // If the command of the server is "Username in use" disconnect
                        // TODO: Update the status and run Disconnect().
                        break;
                    }
                }
​
            Message = inData;
            }
        }



        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (SynchronizationContext.Current == context)
            {
                RaisePropertyChanged(propertyName);
            }
            else
            {
                context.Send(RaisePropertyChanged, propertyName);
            }
        }
​
    private void RaisePropertyChanged(object param)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((string)param));
        }
    }
}
