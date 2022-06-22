using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Coldairarrow.DotNettySocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebNettyServer
{
    public partial class Form1 : Form
    {
        WsSocket ws;
        int rows = 0;
        JObject CONN;
        JObject DISFLOOR;
        JObject NOright;
        JObject WELCOME;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ws = new WsSocket();
            ws.PartEvent += OnStep;
            CONN = new JObject(
                new JProperty("command","INIT DISPLAY"),
                new JProperty("message",new JObject(new JProperty("videoAddr","rtsp://192.168.0.143/av0_1")))
            );
            DISFLOOR = new JObject(
                new JProperty("command", "CONTENT DISPLAY"),
                new JProperty("message", new JObject(new JProperty("accessResult", "PERMISSION:DISPATCHING"),
                    new JProperty("accessDesc","F"),
                    new JProperty("extras",new JObject(new JProperty("floorName","7楼")))
                ))
            );
            NOright = new JObject(
                new JProperty("command", "CONTENT DISPLAY"),
                new JProperty("message", new JObject(new JProperty("accessResult", "PERMISSION:DENIED"),
                    new JProperty("accessDesc", "无通行权限"),
                    new JProperty("extras", new JObject(new JProperty("floorName", "7楼")))
                ))
            );
            WELCOME = new JObject(
                new JProperty("command", "CONTENT DISPLAY"),
                new JProperty("message", new JObject(new JProperty("accessResult", "PERMISSION:ACCESS"),
                    new JProperty("accessDesc", "欢迎光临"),
                    new JProperty("extras", new JObject(new JProperty("floorName", "7楼")))
                 ))
            );
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Interval = 10000;
            //timer.Elapsed += SendMessage;
            //timer.Enabled = true;
            //timer.Start();
        }
        private void SendMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            Random Rd = new Random();
            int flg = Rd.Next(1,4);
            string str = string.Empty;
            switch (flg)
            {
                case 1:
                    str = JsonConvert.SerializeObject(DISFLOOR);
                    break;
                case 2:
                    str = JsonConvert.SerializeObject(NOright);
                    break;
                case 3:
                    str = JsonConvert.SerializeObject(WELCOME);
                    break;
            }
            var listconn = ws.theServer.GetAllConnections();
            foreach (var item in listconn)
            {
                item.Send(str);
            }
        }
        public  void OnStep(Object sender, BusMessageArgs message)
        {            
            this.Invoke(new Action(() =>
            {
                if (rows > 200)
                {
                    rows = 0;
                    AcceptMsg.Text = string.Empty;
                }
                rows++;
                AcceptMsg.Text += "\r\n" + message.ComMessage;
                if (message.ComMessage.IndexOf("成功上线")>-1)
                {
                    var listconn = ws.theServer.GetAllConnections();
                    foreach (var item in listconn)
                    {
                        Thread.Sleep(50);
                        item.Send(JsonConvert.SerializeObject(CONN));
                    }
                }
            }));

        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            //ws = new WsSocket();
            //ws.PartEvent += OnStep;
            ws._ListWsmodel.Clear();
            ws.Ws_close();
            ws = new WsSocket();
            ws.PartEvent += OnStep;      }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            ws.Ws_close();
        }

        private async void SendBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SendMsg.Text.ToString()))
            {
                var listconn = ws.theServer.GetAllConnections();
                foreach (var item in listconn)
                {
                    await item.Send(SendMsg.Text.ToString());
                }
            }
        }
    }
}
