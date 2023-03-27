using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace PaintServer
{
    class Program
    {
        static Task connectTask;
        static Task receiveDataTask;
        static Task sendDataTask;

        static UdpClient connectClient;

        static List<IPEndPoint> users;

        const int connectPort = 6666;
        const int drawPort = 5555;
        static void Main(string[] args)
        {
            users = new List<IPEndPoint>();
            connectClient = new UdpClient(connectPort);
            connectTask = Task.Factory.StartNew(() => ListenConnections());

            Console.WriteLine("Connection server running on port: " + ((IPEndPoint)connectClient.Client.LocalEndPoint).Port);

            Task.WaitAll(connectTask);
        }

        private static void ListenConnections()
        {
            while (true)
            {
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Byte[] receiveBytes = connectClient.Receive(ref clientEndPoint);
                    string message = Encoding.ASCII.GetString(receiveBytes);
                    if(message == "connect")
                    {
                        Console.WriteLine(clientEndPoint.ToString() + " connected");
                        users.Add(clientEndPoint);
                        byte[] reply = BitConverter.GetBytes(((IPEndPoint)connectClient.Client.LocalEndPoint).Port);
                        connectClient.Send(reply, reply.Length, clientEndPoint);
                    }
                    if(message == "disconnect")
                    {
                        Console.WriteLine(clientEndPoint.ToString() + " disconnected");
                        users.Remove(clientEndPoint);
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }
    }
}
