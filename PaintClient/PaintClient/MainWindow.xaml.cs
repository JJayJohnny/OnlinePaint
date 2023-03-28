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
using Winforms = System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace PaintClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Task receiveDataTask;
        
        bool connectedToServer = false;
        UdpClient udpClient;
        int drawServerPort;

        Point lastDrawnPoint;
        bool liftPen = true;

        public MainWindow()
        {
            InitializeComponent();
            udpClient = new UdpClient();

            lastDrawnPoint = new Point();
        }

        private void ChoseColor(object sender, RoutedEventArgs args)
        {
            Winforms.ColorDialog colorDialog = new Winforms.ColorDialog();
            if(colorDialog.ShowDialog() == Winforms.DialogResult.OK)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(
                    colorDialog.Color.R,
                    colorDialog.Color.G,
                    colorDialog.Color.B));
                colorPicker.Background = brush;
            }
        }

        public void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this.paintCanvas);
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DrawLine(new Tuple<Point, SolidColorBrush>(point, (SolidColorBrush)colorPicker.Background));

                //send data to server
                if (connectedToServer)
                {
                    try
                    {
                        string ip = ipTextBox.Text;
                        byte[] xByte = BitConverter.GetBytes(point.X);
                        byte[] yByte = BitConverter.GetBytes(point.Y);
                        byte[] message = new byte[xByte.Length + yByte.Length];
                        Buffer.BlockCopy(xByte, 0, message, 0, xByte.Length);
                        Buffer.BlockCopy(yByte, 0, message, xByte.Length, yByte.Length);
                        udpClient.Send(message, message.Length, ip, drawServerPort);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            liftPen = true;
        }
        public void CanvasMouseDown(object sender, MouseEventArgs e)
        {
            
        }

        public void DrawLine(Tuple<Point, SolidColorBrush> point)
        {
            Line line = new Line()
            {
                X1 = lastDrawnPoint.X,
                Y1 = lastDrawnPoint.Y,
                X2 = point.Item1.X,
                Y2 = point.Item1.Y,
                Stroke = point.Item2,
                StrokeThickness = 4,
            };
            if (liftPen)
            {
                line.X1 = line.X2 - 1;
                line.Y1 = line.Y2 - 1;
                liftPen = false;
            }
            paintCanvas.Children.Add(line);
            lastDrawnPoint = point.Item1;     
        }

        private void ConnectClick(object sender, RoutedEventArgs args)
        {
            try
            {
                string ip = ipTextBox.Text;
                int port = int.Parse(portTextBox.Text);

                //udpClient.Connect(ip, port: port);

                Byte[] sendBytes = Encoding.ASCII.GetBytes("connect");
                udpClient.Send(sendBytes, sendBytes.Length, ip, port);

                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient.Receive(ref remoteIpEndPoint);
                int returnData = BitConverter.ToInt32(receiveBytes);

                Debug.WriteLine("Odebrano: " + returnData.ToString());
                drawServerPort = returnData;

                connectedToServer = true;
                ManageConnectionChange();
                receiveDataTask = Task.Factory.StartNew(() => ReceiveData());
                //udpClient.Close();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void DisconnectClick(object sender, RoutedEventArgs args)
        {
            try
            {
                string ip = ipTextBox.Text;
                int port = int.Parse(portTextBox.Text);

                //udpClient.Connect(ip, port: port);
                Byte[] sendBytes = Encoding.ASCII.GetBytes("disconnect");
                udpClient.Send(sendBytes, sendBytes.Length, ip, port);

                drawServerPort = -1;
                connectedToServer = false;
                ManageConnectionChange();
                //udpClient.Close();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void ManageConnectionChange()
        {
            ipTextBox.IsEnabled = !connectedToServer;
            portTextBox.IsEnabled = !connectedToServer;
            connectButton.IsEnabled = !connectedToServer;

            disconnectButton.IsEnabled = connectedToServer;
        }

        private void ReceiveData()
        {
            while (connectedToServer)
            {
                try
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    byte[] message = udpClient.Receive(ref sender);
                paintCanvas.Dispatcher.Invoke(new Action(() =>
                {
                    byte[] x = new byte[8];
                    byte[] y = new byte[8];
                    Array.Copy(message, 1, x, 0, 8);
                    Array.Copy(message, 9, y, 0, 8);
                    Debug.WriteLine(BitConverter.ToDouble(x) + " " + BitConverter.ToDouble(y));
                    Point p = new Point(BitConverter.ToDouble(x), BitConverter.ToDouble(y));
                    SolidColorBrush b = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    DrawLine(new Tuple<Point, SolidColorBrush>(p, b));
                }));
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }
    }
}
