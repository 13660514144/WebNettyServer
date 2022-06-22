using Coldairarrow.DotNettySocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace WebNettyServer
{
    public class WsSocket
    {
        public  event EventHandler<BusMessageArgs> PartEvent;
        public  IWebSocketServer theServer;
        public Wsmodel _Wsmodel;
        public List<Wsmodel> _ListWsmodel;
        public WsSocket()
        {
            WsSocket_Init();
        }
        public async void WsSocket_Init()
        {
            _ListWsmodel = new List<Wsmodel>();
            await WebSocket();           
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Interval = 2000;
            //timer.Elapsed += RunQueue;
            //timer.Enabled = true;
            //timer.Start();
        }
        public async void Ws_close()
        {
            theServer.Close();
            InFunction($"服务close");
        }
        public async Task WebSocket()
        {
            try
            {
                int Port = 8888;
          
                theServer = await SocketBuilderFactory.GetWebSocketServerBuilder(Port)
                   
                    .OnConnectionClose((server, connection) =>
                    {
                        //_ListWsmodel.RemoveAll(x => x.ConnectId == connection.ConnectionId);
                        //_logger.LogInformation($"连接关闭,连接名[{connection.ConnectionId}],当前连接数:{server.GetConnectionCount()}");
                        InFunction($"连接关闭,连接名[{connection.ConnectionId}],当前连接数:{server.GetConnectionCount()}");
                        _ListWsmodel.Remove(_ListWsmodel.Where(p => p.ConnectId == connection.ConnectionId).FirstOrDefault());
                    })
                    .OnException(ex =>
                    {
                        //_logger.LogInformation($"服务端异常:{ex.Message}");
                        InFunction($"服务端异常:{ex.Message}");
                    })
                    .OnNewConnection((server, connection) =>
                    {
                        connection.ConnectionName = $"名字{connection.ConnectionId}";
                        _Wsmodel = new Wsmodel();
                        _Wsmodel.ConnectId = connection.ConnectionId;
                        _Wsmodel.Connection = connection;
                        _ListWsmodel.Add(_Wsmodel);
                        InFunction($"成功上线:{connection.ConnectionId}");
                    })
                    .OnRecieve((server, connection, msg) =>
                    {
                        try
                        {
                            /*JObject Obj = JObject.Parse(msg);
                            switch ((string)Obj["Mothed"])
                            {
                                case "ReturnClientModel":
                                    _ListWsmodel.RemoveAll(x => x.ConnectId == connection.ConnectionId);
                                    Wsmodel Ws = (Wsmodel)JsonConvert.DeserializeObject(Obj["Data"].ToString(), typeof(Wsmodel));
                                    Ws.ConnectId = connection.ConnectionId;
                                    _ListWsmodel.Add(Ws);

                                    //_logger.LogInformation($"ReturnClientModel:{JsonConvert.SerializeObject(_ListWsmodel)}");
                                    break;
                                case "RequestData":
                                    break;
                            }*/
                            InFunction($"客户端 id：{connection.ConnectionId}发送消息={msg}");
                        }
                        catch (Exception ex)
                        {
                           // _logger.LogInformation($"ReturnClientModel:{JsonConvert.SerializeObject(_ListWsmodel)} msg={msg}");
                        }
                        //_logger.LogInformation($"服务端 OnRecieve:数据{msg}");
                        //connection.Send($"服务端 OnRecieve:数据{msg}  client={connection.ConnectionId}");
                        SeverTOClient(connection,$"收到数据：{msg}");
                    })
                    .OnSend((server, connection, msg) =>
                    {
                        //Console.WriteLine($"向连接名[{connection.ConnectionName}]发送数据:{msg}");
                        InFunction($"向连接名[{connection.ConnectionName}]发送数据:{msg}");
                    })
                    .OnServerStarted(server =>
                    {
                        //_logger.LogInformation("WS-Socket服务启动");
                        InFunction($"服务启动成功  WS-Socket服务启动");

                    }).BuildAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogInformation($"WS-Socket start err :{ex.Message}");
                InFunction($"WS-Socket start err :{ex.Message}");
            }
        }
        public async void InFunction(string comMessage)
        {
            var messageArg = new BusMessageArgs(comMessage);
            //PartEvent?.Invoke(null, messageArg);
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                PartEvent?.Invoke(null, messageArg);
            }
            ));
            //触发事件，执行所有注册过的函数
        }
        /// <summary>
        public async void SeverTOClient(IWebSocketConnection Client, string Msg)
        {
            await Client.Send(Msg);
        }

        public class Wsmodel
        {
            public string ConnectId { get; set; }
            public string ClientIp { get; set; }
            public string ClientPort { get; set; }
            public string DeviceType { get; set; }
            public IWebSocketConnection Connection { get; set; }
        }
    }
}
