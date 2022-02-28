using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace _2222222
{
    public partial class frmClient : Form
    {
        public frmClient()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        Thread threadClient = null;
        Socket sockClient = null;
        string recClientIpAndPort = "";
        private void btnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txtIp.Text.Trim());
            IPEndPoint endPoint=new IPEndPoint (ip,int.Parse(txtPort.Text.Trim()));
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ShowMsg("Connecting to server...");
                sockClient.Connect(endPoint);
                
            }
            catch (SocketException skep)
            {
                MessageBox.Show(skep.Message);
                return;
                //this.Close();
            }
            ShowMsg("Connected successfully!");
            threadClient = new Thread(RecMsg);
            threadClient.IsBackground = true;
            threadClient.Start();

        }

        void RecMsg()
        {
            while (true)
            {
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                int length = -1;
                try
                {
                    length = sockClient.Receive(arrMsgRec);
                }
                catch (SocketException errorOccurred)
                {
                    ShowMsg("Error；" + errorOccurred.Message);
                    return;
                }
                catch (Exception errorOccurred)
                {
                    ShowMsg("Error："+errorOccurred.Message);
                    return;
                }

                if (arrMsgRec[0] == 0)
                {
                    string msgRecStr = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                    ShowMsg(msgRecStr);
                }


                if (arrMsgRec[0] == 1)
                {
                    string msgRecStr = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                    textBoxDistance.Text = msgRecStr;
                }

                if (arrMsgRec[0] == 2)
                {
                    string msgRecStr = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                    ShowMsg(msgRecStr);
                    recClientIpAndPort = msgRecStr;
                }


            }
        }
        void ShowMsg(string str)
        {
            txtMsg.AppendText(str + "\r\n");
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            string point1 = x1.Text.Trim() + "," +  y1.Text.Trim() + "," + z1.Text.Trim();
            string point2 = x2.Text.Trim() + "," + y2.Text.Trim() + "," + z2.Text.Trim();
            string strMsg = "The first point is : " + point1 + "\r\n" 
                            + "The second point is : " +  point2;
            string sendMsg = point1 + ";" + point2 + ";" + recClientIpAndPort;
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
            arrSendMsg[0] = 0;
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            sockClient.Send(arrSendMsg);
          
            ShowMsg(strMsg);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void txtIp_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void x1_TextChanged(object sender, EventArgs e)
        {

        }

        private void x1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        

        private void y1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void z1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void x2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void y2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void z2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && e.KeyChar != '.')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
