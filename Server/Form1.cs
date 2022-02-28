using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;  // For IP，IPAddress, IPEndPoint，Port
using System.Threading;
using System.IO;

namespace _11111
{
    public partial class frm_server : Form
    {
        public frm_server()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        // clients' connections
        Thread threadWatch = null; 
        Socket socketWatch = null;

        // saving new Socket and Thread
        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();

        string toSendClinet = "";
        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(txtIp.Text.Trim());  // getting IP address
                // creating object for ip and port
                IPEndPoint endPoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
                try
                {
                    // bind ip and port
                    socketWatch.Bind(endPoint);
                }
                catch (SocketException errorOccurred)
                {
                    MessageBox.Show("Error："+errorOccurred.Message);
                    return;
                }
                // set clents up to 6 for the requirement
                socketWatch.Listen(6);
                // threads
                threadWatch = new Thread(WatchConnecting);
                threadWatch.IsBackground = true;
                threadWatch.Start();
                ShowMsg("Server has started successfully！");
        }

        void WatchConnecting()
        {
            while (true)
            {
                // Accept() method is used to obtain the socket which a client's connection is detected.
                Socket sokConnection = socketWatch.Accept(); 
                // adding client's IP into the list box
                lbOnline.Items.Add(sokConnection.RemoteEndPoint.ToString());
                // adding socket contents connected to client's profile
                dict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                ShowMsg("Client connected successfully！");
                // sending the client's ip and port back to the client
                // due to the last problem of always sending the result for last client's app
                string sendMsg = sokConnection.RemoteEndPoint.ToString();
                // convert into byte
                byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(sendMsg);
                // creating new byte
                byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                // sending result
                arrSendMsg[0] = 2;
                Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                ShowMsg("Replying the client: " + sendMsg);
                
                Thread thr = new Thread(RecMsg);
                thr.IsBackground = true;
                thr.Start(sokConnection);
                // adding new threads
                dictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);
                sokConnection.Send(arrSendMsg);
            }
        }

        void RecMsg(object sokConnectionparn)
        {
                Socket sokClient = sokConnectionparn as Socket;
                while (true)
                {
                    // define a 2M cache because got overload last time and the program is the bottleneck
                    byte[] contentRec = new byte[1024 * 1024 * 2];
                    // save content into the variable
                    int length = -1;
                    try
                    {
                        length = sokClient.Receive(contentRec);  // the real length of receiving data
                    }
                    catch (SocketException errorHappened)
                    {
                        ShowMsg("Error：" + errorHappened.Message);
                        // remove everything
                        dict.Remove(sokClient.RemoteEndPoint.ToString());
                        dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                        lbOnline.Items.Remove(sokClient.RemoteEndPoint.ToString());
                        break;
                    }
                // if data received
                if (contentRec[0] == 0)
                {
                    if (contentRec[1] == 0 && contentRec[2] == 0 && contentRec[3] == 0 && contentRec[4] == 0) 
                    {
                        return;
                    }
                    //convert to string
                    string msgRecStr = System.Text.Encoding.UTF8.GetString(contentRec, 1, length - 1);

                    ShowMsg("The Two 3D points: " + msgRecStr);

                    // sending
                    string sendMsg = getCalculte(msgRecStr).ToString();
                    // convert into byte
                    byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(sendMsg);
                    // creating new byte
                    byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                    // sending result
                    arrSendMsg[0] = 1; 
                    Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                    // solving error by adding this, need search more
                    dict[toSendClinet].Send(arrSendMsg);

                    ShowMsg("Distance: " + sendMsg);
                }


            }     
        }
        // for calculating distance
        double getCalculte(string str)
        {
            int numericValue;
            // get the two points
            string[] rows = str.Split(new char[] { ';' });
            string[] strPoint1 = rows[0].Split(new char[] { ',' });
            string[] strPoint2 = rows[1].Split(new char[] { ',' });
            // getting the specific client's ip and port number
            toSendClinet = rows[2];
            double[] point1 = { 0, 0, 0 };
            double[] point2 = { 0, 0, 0 };

            // the first point
            point1[0] = double.Parse(strPoint1[0]);
            point1[1] = double.Parse(strPoint1[1]);
            point1[2] = double.Parse(strPoint1[2]);
            // the second point
            point2[0] = double.Parse(strPoint2[0]);
            point2[1] = double.Parse(strPoint2[1]);
            point2[2] = double.Parse(strPoint2[2]);
            // the formula
            double value = Math.Pow(Math.Abs(point1[0] - point2[0]), 2.0)
                + Math.Pow(Math.Abs(point1[1] - point2[1]), 2.0)
                + Math.Pow(Math.Abs(point1[2] - point2[2]), 2.0);

            return Math.Round(Math.Sqrt(value), 2);
        }
        void ShowMsg(string str)
        {
            txtMsg.AppendText(str + "\r\n");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msgRecStr = "Server" + "\r\n" + "   -->" + txtMsgSend.Text.Trim() + "\r\n";
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(msgRecStr); 
            byte[] arrSendMsg=new byte[arrMsg.Length+1];
            arrSendMsg[0] = 0; 
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            string clientInList = "";
            clientInList = lbOnline.Text.Trim();
            if (string.IsNullOrEmpty(clientInList))   // if none of clients selected, will show warning
            {
                MessageBox.Show("Please select the client.");
            }
            else
            {
                dict[clientInList].Send(arrSendMsg);
                ShowMsg(msgRecStr);
                txtMsgSend.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socketWatch.Close();
            threadWatch.Abort();
            this.Dispose();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
