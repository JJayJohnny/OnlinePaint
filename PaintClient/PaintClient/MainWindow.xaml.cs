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
        Task drawTask;
        BlockingCollection<Tuple<Point, SolidColorBrush>> pointQueue;
        bool drawGap = true;
        bool connectedToServer = false;
        UdpClient udpClient;
        int drawServerPort;

        public MainWindow()
        {
            InitializeComponent();
            pointQueue = new BlockingCollection<Tuple<Point, SolidColorBrush>>(new ConcurrentQueue<Tuple<Point, SolidColorBrush>>());
            udpClient = new UdpClient();

            drawTask = Task.Factory.StartNew(() => DrawTask());
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
                pointQueue.Add(new Tuple<Point, SolidColorBrush>(point, (SolidColorBrush)colorPicker.Background));
            }
        }

        public void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            drawGap = true;
        }

        public void DrawLine(Tuple<Point, SolidColorBrush> point)
        {
                Line line = new Line()
                {
                    X1 = point.Item1.X-1,
                    Y1 = point.Item1.Y-1,
                    X2 = point.Item1.X,
                    Y2 = point.Item1.Y,
                    Stroke = point.Item2,
                    StrokeThickness = 4,
                };
                paintCanvas.Children.Add(line);
        }

        private void DrawTask()
        {
            
            Tuple<Point, SolidColorBrush> lastPoint = new Tuple<Point, SolidColorBrush>(new Point(), null);
            while (true) {
                try
                {
                    var point = pointQueue.Take();
                    drawGap = !drawGap;
                    if (drawGap)
                    {                      
                        Dispatcher.Invoke(() =>
                        {
                            Line line = new Line()
                            {
                                X1 = point.Item1.X,
                                Y1 = point.Item1.Y,
                                X2 = lastPoint.Item1.X,
                                Y2 = lastPoint.Item1.Y,
                                Stroke = point.Item2,
                                StrokeThickness = 4,
                            };
                            paintCanvas.Children.Add(line);
                        });
                        pointQueue.Add(point);
                    }
                    else
                        lastPoint = point;
                }
                catch (InvalidOperationException)
                {

                }
            }
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
    }
}
