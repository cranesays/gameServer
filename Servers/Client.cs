using EchoProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EchoServer.DAO;
using Org.BouncyCastle.Bcpg;
using MySql.Data.MySqlClient;

namespace EchoServer
{
    public class Client
    {
        private string connstr = "database=game;data source=localhost;user=root;password=root;pooling=false;charset=utf8;port=3306";
        
        public Socket socket;
        private Message message;
        private UserData userData;
        private Server server;
        private MySqlConnection mysqlCon;

        public UserData GetUserData { get { return userData; } }
        public MySqlConnection GetMySqlConnection { get { return mysqlCon; } }
        public Room ?GetRoom { get;set; }
        public string ?UserName { get; set; }


        public Client(Socket socket, Server server)
        {
            userData= new UserData();
            message = new Message();
            try
            {
                mysqlCon = new MySqlConnection(connstr);
                mysqlCon.Open();
                Console.WriteLine("DB connected success");

                //打开相应数据库，放在这里应该可以
                string sql = "USE game";
                MySqlCommand comd = new MySqlCommand(sql, mysqlCon);
                comd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库连接失败" + ex.Message);
            }

            this.socket = socket;
            this.server = server;
            StartReceive();
        }

        public void StartReceive()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex, message.RemainSize, SocketFlags.None, ReceiveCallback, null);
        }
        public void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                if (socket == null || socket.Connected == false) return;
                int size = socket.EndReceive(iar);

                if (size == 0) return;

                message.ReadBuff(size,HandleRequest);
                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine("NetworkClient Receive Fail" + ex.ToString());
            }
        }
        public void Send(MainPark park)
        {
            socket.Send(Message.ParkData(park));
        }

        void HandleRequest(MainPark park)
        {
            server.HandleRequest(park, this);
        }

        private void Close()
        {
            if(GetRoom != null)
            {
                GetRoom.Exit(this);
            }
            server.RemoveClient(this);
            socket.Close();
            mysqlCon.Close();
        }
    }
}
