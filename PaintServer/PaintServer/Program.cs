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

        static UdpClient connectServer;
        static UdpClient drawDataServer;

        static List<IPEndPoint> users;

        const int connectPort = 6666;
        const int drawPort = 5555;
        static void Main(string[] args)
        {
            users = new List<IPEndPoint>();
            connectServer = new UdpClient(connectPort);
            drawDataServer = new UdpClient(drawPort);

            connectTask = Task.Factory.StartNew(() => ListenConnections());
            receiveDataTask = Task.Factory.StartNew(() => ReceiveData());

            Console.WriteLine("Connection server running on port: " + ((IPEndPoint)connectServer.Client.LocalEndPoint).Port);

            Task.WaitAll(connectTask, receiveDataTask);
        }

        private static void ListenConnections()
        {
            while (true)
            {
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Byte[] receiveBytes = connectServer.Receive(ref clientEndPoint);
                    string message = Encoding.ASCII.GetString(receiveBytes);
                    if(message == "connect")
                    {
                        Console.WriteLine(clientEndPoint.ToString() + " connected");
                        users.Add(clientEndPoint);
                        byte[] reply = BitConverter.GetBytes(((IPEndPoint)drawDataServer.Client.LocalEndPoint).Port);
                        connectServer.Send(reply, reply.Length, clientEndPoint);
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

        private static void ReceiveData()
        {

        }
    }
}
