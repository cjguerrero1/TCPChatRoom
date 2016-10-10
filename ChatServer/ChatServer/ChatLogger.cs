using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChatServer
{
    public class ChatLogger : ILoggable
    {
        StreamWriter writer = new StreamWriter("chatText.txt", true);
        public void LogExceptions(string message)
        {
            writer.WriteLine("Exception was thrown: {0}", message);
        }

        public void LogMessages(string message)
        {
            writer.WriteLine("Message sent: {0}", message);
        }

        public void LogUserActivity(string message)
        {
            writer.WriteLine(message);
        }
    }
}
