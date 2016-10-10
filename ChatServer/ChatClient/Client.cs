using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets; 
    



namespace ChatClient
{
    public class Client
    {
        TcpClient tcpClient;

        public Client()
        {
            tcpClient = new TcpClient(); 
        }

        public void StartChat()
        {
            tcpClient.Connect("10.2.20.33", 54321);
            if(tcpClient.Connected)
            {
                Console.WriteLine("You are connected.");
            }
            Thread sendThread = new Thread(SendMessage);
            Thread recieveThread = new Thread(RecieveMessage);
            recieveThread.Start(tcpClient);
            sendThread.Start(tcpClient);
          
        }

        public static void SendMessage(object argument)
        {
            TcpClient client = (TcpClient)argument;
            StreamWriter writer = new StreamWriter(client.GetStream());
            try
            {
                string message = String.Empty.ToLower();
                while (!message.Equals("exit"))
                {
                    message = Console.ReadLine();
                    writer.WriteLine(message);
                    writer.Flush();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void RecieveMessage(object argument)
        {
            TcpClient client = (TcpClient)argument;
            StreamReader reader = new StreamReader(client.GetStream());
            try
            {
                while (true)
                {
                    string serverString = reader.ReadLine();
                    Console.WriteLine(serverString);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

       
    }
}
