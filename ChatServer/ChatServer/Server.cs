using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace ChatServer
{
    public class Server
    {
        TcpListener chatServer;
        public static Dictionary<TcpClient, string> users = new Dictionary<TcpClient, string>();
        private static Queue<string> messageQueue;
        public readonly string ChatName;
        IPAddress localIp = IPAddress.Parse("10.2.20.33");
        public ILoggable logger;
        

        public Server(string chatName, ILoggable logger)
        {
            messageQueue = new Queue<string>();
            ChatName = chatName;
            this.logger = logger;
        }

        public void TurnServerOn()
        {
            try
            {
                chatServer = new TcpListener(localIp, 54321);
                chatServer.Start();
                Console.WriteLine("Server online...");
                while (true)
                {
                    Console.WriteLine("Waiting for client connections...");
                    TcpClient client = chatServer.AcceptTcpClient();
                    Console.WriteLine("New client connected...");
                    Thread chatThread = new Thread(ProcessClientMessages);
                    chatThread.Start(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                string message = e.ToString();
                logger.LogExceptions(message);
                
            }
        }

        private static void ProcessClientMessages(object argument)
        {
            
            TcpClient client = (TcpClient)argument;
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());
            try
            {
                AddNewUser(reader, writer, client);
                string message = String.Empty.ToLower();
                while (message != "exit")
                {
                    if (messageQueue.Count == 0)
                    { 
                        message = reader.ReadLine();
                        AddMessageToQueue(message, client);
                        BroadcastMessageToAll();
                    }
                    else
                    {
                        BroadcastMessageToAll();
                    }
                }
            }
            catch (IOException)
            {
                CheckForDisconnect();
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        public static string GetUserName(StreamWriter writer, StreamReader reader)
        {
            writer.WriteLine("Enter username: ");
            writer.Flush();
            return reader.ReadLine();
        }

        public static void AddNewUser(StreamReader reader, StreamWriter writer, object user)
        {
            TcpClient client = (TcpClient)user;
            string userName = GetUserName(writer, reader);
            if (users.ContainsValue(userName))
            {
                writer.WriteLine("That username is taken.");
                writer.Flush();
                AddNewUser(reader, writer, user);
            }
            else
            {
                users.Add(client, userName);
                string joinMessage = String.Format("{0} has joined the chat", userName);
                messageQueue.Enqueue(joinMessage);
               // logger.LogUserActivity(joinMessage);
                writer.Write("{0}: ", userName);
                writer.Flush();
            }
        }

        public static void BroadcastMessageToAll()
        {
            try
            {
                string messageToSend = GetMessage();
                foreach (TcpClient client in users.Keys)
                {
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    writer.WriteLine(messageToSend);
                    writer.Flush();
                   // logger.LogMessages(messageToSend);
                }
            }
            catch(IOException)
            {

            }
             
            
        }

        public static string GetMessage()
        {
            return messageQueue.Dequeue();
        }

        public static void AddMessageToQueue(string message, TcpClient client)
        {
            string userName = users[client];
            string messageToSend = FormatMessage(message, userName);
            messageQueue.Enqueue(messageToSend);
        }

        public static string FormatMessage(string message, string userName)
        {
            return String.Format("{0}: {1}", userName, message);
        }

        public static void CheckForDisconnect()
        {
            try
            {
                foreach (TcpClient client in users.Keys)
                {
                    if (!client.Connected)
                    {
                        string disconnectMessage = String.Format("{0} has left the chat.", users[client]);
                        Console.WriteLine(disconnectMessage);
                        messageQueue.Enqueue(disconnectMessage);
                        //logger.LogUserActivity(disconnectMessage);
                        users.Remove(client);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("A user left");
            }
       }

    }
}
    