using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingClientProgram
{
    [Serializable]
    class ChatPacket
    {
        public string Username { get; set; }
        public string Message { get; set; }
      //  public string UserColor { get; set; }
    }

    [Serializable]
    public class UserConnectionPacket
    {
        public string Username { get; set; }
        public string UserGuid { get; set; }
        public string[] Users { get; set; }
        public bool IsJoining { get; set; }
    }
}
