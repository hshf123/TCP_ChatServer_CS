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
        public Form1(string userName)
        {
            InitializeComponent();
            Form = this;
            tb_myName.Text = userName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connect();
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
        
        // 채팅창
        public void WriteMessage(string userName, string chat)
        {
            if(rtb_Message.InvokeRequired)
            {
                rtb_Message.Invoke(new MethodInvoker(delegate ()
                {
                    AppendText(userName, chat);
                }));
            }
            else
            {
                AppendText(userName, chat);
            }
        }
        private void AppendText(string userName, string chat)
        {
            if (tb_myName.Text == userName)
            {
                rtb_Message.SelectionAlignment = HorizontalAlignment.Right;
                string c = $"{chat}";

                rtb_Message.AppendText(c + "\n");
                rtb_Message.Focus();
                rtb_Message.ScrollToCaret();

                rtb_chatBox.Clear();
                rtb_chatBox.Focus();
            }
            else
            {
                rtb_Message.SelectionAlignment = HorizontalAlignment.Left;
                string c = $"{userName} > {chat}";
                rtb_Message.AppendText(c + "\n");
                rtb_Message.Focus();
                rtb_Message.ScrollToCaret();

                rtb_chatBox.Clear();
                rtb_chatBox.Focus();
            }
        }

        // 채팅 내용 보내기
        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (rtb_chatBox.Text == "")
            {
                rtb_chatBox.Clear();
                return;
            }

            string userName = $"{tb_myName.Text}";
            string chat = $"{rtb_chatBox.Text}";
            C_Chat packet = new C_Chat();
            packet.userName = userName;
            packet.chat = chat;
            SessionManager.Instance.SendForEach(packet);
            rtb_chatBox.Clear();
        }
        private void rtb_chatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btn_Send_Click(sender, e);
                return;
            }
        }

        // 유저 카운팅 하기
        public void UserCount(int count)
        {
            if (tb_userNames.InvokeRequired)
            {
                tb_userNames.Invoke(new MethodInvoker(delegate ()
                {
                    tb_userNames.Text = $"{count} 명 입장...";
                }));
            }
            else
            {
                tb_userNames.Text = $"{count} 명 입장...";
            }
        }
    }
}
