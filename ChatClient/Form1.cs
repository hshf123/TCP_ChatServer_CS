using ServerCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public static Form1 Form;
        public Form1()
        {
            InitializeComponent();
            Form = this;
        }

        // 채팅창
        public void WriteMessage(string userName, string chat)
        {
            if(rtb_Message.InvokeRequired)
            {
                rtb_Message.Invoke(new MethodInvoker(delegate ()
                {
                    if (tb_myName.Text == userName)
                    {
                        rtb_Message.SelectionAlignment = HorizontalAlignment.Right;
                        string c = $"{chat}";

                        rtb_Message.AppendText(c + "\n");
                        rtb_Message.Focus();
                        rtb_Message.ScrollToCaret();

                        rtb_chatBox.Focus();
                    }
                    else
                    {
                        rtb_Message.SelectionAlignment = HorizontalAlignment.Left;
                        string c = $"{userName} > {chat}";
                        rtb_Message.AppendText(c + "\n");
                        rtb_Message.Focus();
                        rtb_Message.ScrollToCaret();

                        rtb_chatBox.Focus();
                    }
                }));
            }
            else
            {
                if (tb_myName.Text == userName)
                {
                    rtb_Message.SelectionAlignment = HorizontalAlignment.Right;
                    string c = $"{chat}";

                    rtb_Message.AppendText(c + "\n");
                    rtb_Message.Focus();
                    rtb_Message.ScrollToCaret();

                    rtb_chatBox.Focus();
                }
                else
                {
                    rtb_Message.SelectionAlignment = HorizontalAlignment.Left;
                    string c = $"{userName} > {chat}";
                    rtb_Message.AppendText(c + "\n");
                    rtb_Message.Focus();
                    rtb_Message.ScrollToCaret();

                    rtb_chatBox.Focus();
                }
            }
        }

        public void Connect()
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 194);

            Connector connector = new Connector();
            connector.Connect(endPoint, SessionManager.Instance.Generate, 1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connect();
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string userName = $"{tb_myName.Text}";
            string chat = $"{rtb_chatBox.Text}";
            C_Chat packet = new C_Chat();
            packet.userName = userName;
            packet.chat = chat;
            SessionManager.Instance.SendForEach(packet);
        }

        
    }
}
