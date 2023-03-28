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
        static BlockingCollection<byte[]> drawData;

        const int connectPort = 6666;
        const int drawPort = 5555;
        static void Main(string[] args)
        {
            users = new List<IPEndPoint>();
            drawData = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
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
                    Console.WriteLine("Liczba zalogowanych uzytkownikow: "+users.Count);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private static void ReceiveData()
        {
            while (true)
            {
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = drawDataServer.Receive(ref clientEndPoint);

                    //determine what kind of data is this

                    byte userId = Convert.ToByte(GetUserId(clientEndPoint));

                    byte[] storeData = new byte[receivedBytes.Length + 1];
                    storeData[0] = userId;
                    Buffer.BlockCopy(receivedBytes, 0, storeData, 1, receivedBytes.Length);
                    drawData.Add(storeData);

                    /*byte[] x = new byte[8];
                    byte[] y = new byte[8];
                    Array.Copy(receivedBytes, 0, x, 0, 8);
                    Array.Copy(receivedBytes, 8, y, 0, 8);
                    Console.WriteLine("Received " + BitConverter.ToDouble(x)+" "+BitConverter.ToDouble(y) + " from user " + userId);*/
                    //Console.WriteLine(receivedBytes.Length.ToString() +" "+ userId.GetType().ToString());
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private static int GetUserId(IPEndPoint user)
        {
            foreach(IPEndPoint u in users)
            {
                if (u.Equals(user))
                    return users.IndexOf(u);
            }
            return -1;
        }
    }
}
