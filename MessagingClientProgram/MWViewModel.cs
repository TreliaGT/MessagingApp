﻿using System;
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
    class MWViewModel : INotifyPropertyChanged
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


        private bool status;
        public bool Status
        {
            get{return status;}
            set { status = value; NotifyPropertyChanged();}
        }

        // keep track of UI Thread
        private SynchronizationContext context;

        // assign to by other classes when this class is updated
        public event PropertyChangedEventHandler PropertyChanged;

        // Used to stop a Thread when the client loses connection to the server.
        private bool isCapturingMessages;
    

        private TcpClient clientSocket;
        private NetworkStream serverStream;

        // A thread used to capture messages from the server.
        private Thread captureThread;

        // The maximum size of the buffer allowed.
        private int bufferSize = 10025;

        public MWViewModel()
        {
            clientSocket = null;
            serverStream = default(NetworkStream);
            isCapturingMessages = false;
            context = SynchronizationContext.Current;
        }


        /// <summary>
        /// Connect to a server
        /// </summary>
        public void Connect()
        {
        
            // Close any already existing connections
            if (IsConnected())
            {
                Disconnect();
            }
        
            // Create a new connection to the server with the given username.
            Status = false;

            if (clientSocket == null) clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect(_address, Convert.ToInt32(_port));
            }
            catch (SocketException)
            {
                Disconnect();
                return;
            }
            serverStream = clientSocket.GetStream();
            
            // Send the username to the server
            byte[] outStream = Encoding.ASCII.GetBytes(_username + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

            Status = true;
            // Create a Thread to monitor retrieval of new messages
            isCapturingMessages = true;
            captureThread = new Thread(CaptureMessages);
            captureThread.Start();
        }


        public void Send(string message)
        {
            if (IsConnected())
            {
                byte[] outStream = Encoding.ASCII.GetBytes(message + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }

        public void Disconnect()
        {
            // Only disconnect when there is no connection
            if (!IsConnected())
                return;
            
            // this will stop the captureThread
            isCapturingMessages = false;

            serverStream.Close();
            serverStream = default(NetworkStream);
  
            clientSocket.Close();
                clientSocket = null;

            Status = false;
        }

        /// <summary>
        /// Ensure that there is a connection.
        /// </summary>
        /// <returns>true if there is a connection.</returns>
        public bool IsConnected()
        {
            return (clientSocket != null && clientSocket.Connected);
        }

        /// <summary>
        /// While isCaptureingMessages is true, communicate to the server.
        /// Upon waiting for messages
        /// </summary>
        private void CaptureMessages()
        {
            while (isCapturingMessages)
            {
                serverStream = clientSocket.GetStream();
                clientSocket.ReceiveBufferSize = bufferSize;
                
                // Convert data recieved from the NetworkStream to a byte array.
                byte[] inStream = new byte[bufferSize];
                try
                {
                    serverStream.Read(inStream, 0, bufferSize);
                }
                catch (Exception)
                {
                    // If the client cannot get data from the NetworkStream, close connection.
                    Disconnect();
                    break;
                }
                
            // Get a message from the server
            var inData = Encoding.ASCII.GetString(inStream);
                
            // Check if the message recieved from the server is a command (!<Command>!)
            int firstIndex = inData.IndexOf('!');
                int lastIndex = inData.LastIndexOf('!');
                
            if (firstIndex == 0
                && lastIndex > 0)
                {
                var message = inData.Substring(1, lastIndex - 1);
                    
                if (message.Equals("Username in use"))
                    {
                        // If the command of the server is "Username in use" disconnect
                
                        Disconnect();
                        break;
                    }
                }
                //"\0"
            int indexIDATA = inData.IndexOf('\0');
            inData = inData.Substring(0 , indexIDATA);
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
        
        private void RaisePropertyChanged(object param)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((string)param));
        }
    }
}
