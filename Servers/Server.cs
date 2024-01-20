using EchoProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EchoServer.DAO;

namespace EchoServer
{
    public class Server
    {
        private Socket socket;
        //世界消息对每个客户端发送
        private List<Client> clientList = new List<Client>();

        //房间消息在房间内发送
        private List<Room> roomList = new List<Room>();
        private ControllerManager controllerManager;//项目中只有一个，在服务器中初始化
        public Server(int port)
        {
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);
            StartAccept();
        }
        void StartAccept()
        {
            socket.BeginAccept(AcceptCallback,null);
        }

        void AcceptCallback(IAsyncResult iar) 
        {
            Socket client = socket.EndAccept(iar);
            clientList.Add(new Client(client,this));
            StartAccept();
        }

        public void HandleRequest(MainPark park,Client client)
        {
            controllerManager.HandleRequest(park,client);
        }


        public void RemoveClient(Client client)
        {
            clientList.Remove(client);
        }

        public MainPark CreateRoom(Client client, MainPark park)
        {
            //这里不再使用CreateRoom消息，而是改用Room消息的第一个
            try
            {
                Room room = new Room(client, park.RoomPark[0]);
                roomList.Add(room);
                //Console.WriteLine(room.GetRoomInfo.RoomName + "," + room.GetRoomInfo.PlayerNum);
                foreach(PlayerPark p in room.GetPlayerInfo())
                {
                    park.PlayerPark.Add(p);
                }
                park.ReturnCode = ReturnCode.Success; 
            }
            catch(Exception e)
            {
                Console.WriteLine("房间创造失败"+e.ToString());
                park.ReturnCode = ReturnCode.Fail; 
            }
            return park;
        }

        public MainPark SearchRoom()
        {
            MainPark park = new MainPark();
            park.ActionCode = ActionCode.SearchRoom;
            try
            {
                if(roomList.Count == 0)
                {
                    park.ReturnCode= ReturnCode.NoRoom;
                    return park;
                }
                //理论上清空房间列表不会造成影响
                park.RoomPark.Clear();
                foreach (Room room in roomList)
                {
                    park.RoomPark.Add(room.GetRoomInfo);
                    //Console.WriteLine(room.GetRoomInfo.RoomName + "," + room.GetRoomInfo.PlayerNum);
                }
                park.ReturnCode = ReturnCode.Success;
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
                park.ReturnCode= ReturnCode.Fail;
            }
            return park;
        }
        /// <summary>
        /// 这里只设置了成功的返回，应该还有失败的返回，之后需要修改
        /// </summary>
        /// <param name="client"></param>
        /// <param name="park"></param>
        /// <returns></returns>
        public MainPark JoinRoom(Client client, MainPark park)
        {
            foreach(Room room in roomList) 
            {
                if (room.GetRoomInfo.RoomName.Equals(park.RoomPark[0].RoomName))
                {
                    //没有设置房间状态，全部可以加入
                    //以后再改吧
                    room.Join(client);
                    park.RoomPark.Add(room.GetRoomInfo);
                    foreach(PlayerPark player in room.GetPlayerInfo())
                    {
                        park.PlayerPark.Add(player);
                    }
                    park.ReturnCode = ReturnCode.Success;
                    return park;
                }
            }
            return park;
        }

        public MainPark ExitRoom(Client client, MainPark park)
        {
            if(client.GetRoom==null)
            {
                park.ReturnCode = ReturnCode.Fail;
                return park;
            }
            client.GetRoom.Exit(client);
            park.ReturnCode = ReturnCode.Success;
            return park;
        }

        ///// <summary>
        ///// 从房间列表移除房间
        ///// </summary>
        ///// <param name="client"></param>
        ///// <param name="park"></param>
        //public void RomoveRoom(Room room)
        //{
        //    roomList.Remove(room)
        //}
    }
}
