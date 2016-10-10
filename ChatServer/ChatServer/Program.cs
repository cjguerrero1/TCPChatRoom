using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatLogger logger = new ChatLogger();
            Server server = new Server("ChatRoom", logger);
            server.TurnServerOn(logger);
            Console.ReadKey();

        }
    }
}
