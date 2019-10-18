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
using System.Collections.ObjectModel;

namespace MessagingClientProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MWViewModel mum;
        private ObservableCollection<string> messageCollection;
  
        public MainWindow()
        {
            InitializeComponent();

            mum = new MWViewModel();
            mum.PropertyChanged += MWVM_PropertyChanged;

            messageCollection = new ObservableCollection<string>();
            MessageLV.ItemsSource = messageCollection;
        }

        /// <summary>
        /// addes messages to the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MWVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Message")
            {
                messageCollection.Add(mum.Message);
            }
            if(e.PropertyName == "Status")
            {
                status.IsChecked = mum.Status;
            }
        }

        /// <summary>
        /// sends a message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send_Click(object sender, RoutedEventArgs e)
        {
              mum.Send(MessageTxt.Text);
            MessageTxt.Text = "";
        }

        /// <summary>
        /// sends message using the enter key on the keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mum.Send(MessageTxt.Text);
                MessageTxt.Text = "";
            }
        }

        /// <summary>
        /// connects to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            mum.Port = PortTXT.Text;
            mum.Address = ServerTXT.Text;
            mum.Username = UsernameTXT.Text;
            mum.Connect();
        }

        /// <summary>
        /// disconnects from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            mum.Disconnect();
        }
    }
}
