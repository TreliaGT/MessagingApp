using MahApps.Metro.Controls;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;

namespace MessagingClientProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(MessageTxt.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            readData = "Connecting to Chat Server ..";
            msg();
            clientSocket.Connect("P-B223-176148" ,8888);

            byte[] outStream = Encoding.ASCII.GetBytes(UsernameTXT.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        /*   private void getMessage()
           {
               while (true)
               {
                   serverStream = clientSocket.GetStream();
                   int buffSize = 0;
                   byte[] inStream = new byte[10025];
                   buffSize = clientSocket.ReceiveBufferSize;
                   serverStream.Read(inStream, 0, buffSize);
                   string returndata = Encoding.ASCII.GetString(inStream);
                   readData = " " + returndata;
                   msg();
               }
           }*/

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[300];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                msg();
            }
        }

      /*  private void msg(bool waitUntilReturn = false)
        {
            if (control.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(msg));
            }
            else
            {
                messagesData.Add(UsernameTXT.Text + ":: " + readData);
                MessageLV.Items.Add(messagesData);
            }
          
            Action append = () => MessageLV.Items.Add(UsernameTXT.Text + ":: " + readData);
            if (MessageLV.CheckAccess())
            {
                append();
            }
            else if (waitUntilReturn)
            {
                MessageLV.Dispatcher.Invoke(append);
            }
            else
            {
                MessageLV.Dispatcher.BeginInvoke(append);
            }
        }*/

  

        private void msg()
        {
            MessageTxt.Dispatcher.Invoke(
                new UpdateTextCallback(updateText),
                new object[] { " >> " + readData }
                );
        }

        private void updateText(string message)
        {
            MessageTxt.Text = MessageTxt.Text + Environment.NewLine + message;
        }

        public delegate void UpdateTextCallback(string message);
    }
}
