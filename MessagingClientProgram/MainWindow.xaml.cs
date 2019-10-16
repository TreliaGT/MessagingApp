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
using System.Collections.Specialized;

namespace MessagingClientProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
       /* System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;*/
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)MessageLV.Items).CollectionChanged
              += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            if (e.NewItems.Count > 0)
            {
                MessageLV.ScrollIntoView(MessageLV.Items[MessageLV.Items.Count - 1]);
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var context = (MWViewModel)DataContext;
             context.Send();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            var context = (MWViewModel)DataContext;
            context.Connect();
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            var context = (MWViewModel)DataContext;
            context.Disconnect();
        }


        /*   private void Send_Click(object sender, RoutedEventArgs e)
           {
               byte[] outStream = System.Text.Encoding.ASCII.GetBytes(MessageTxt.Text + "$");
               serverStream.Write(outStream, 0, outStream.Length);
               serverStream.Flush();
           }

           private void ConnectBtn_Click(object sender, RoutedEventArgs e)
           {

                 readData = "Connecting to Chat Server ..";
                  msg();
                  clientSocket.Connect("127.0.0.1", 8888);

                  byte[] outStream = Encoding.ASCII.GetBytes(UsernameTXT.Text + "$");
                  serverStream.Write(outStream, 0, outStream.Length);
                  serverStream.Flush();

                  Thread ctThread = new Thread(getMessage);
                  ctThread.Start();
           }

           public void ReceiveData(TcpClient client)
           {
               NetworkStream ns = client.GetStream();
               byte[] receivedBytes = new byte[1024];
               int byte_count;

               while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
               {
                   MessageLV.Items.Add(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
               }
           }

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



           private void msg()
           {
               MessageLV.Dispatcher.Invoke(
                   new UpdateTextCallback(updateText),
                   new object[] { " >> " + readData }
                   );
           }

           private void updateText(string message)
           {
               MessageTxt.Text = MessageTxt.Text + Environment.NewLine + message;
           }

           public delegate void UpdateTextCallback(string message);*/
    }
}
