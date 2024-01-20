using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;
using Google.Protobuf.Collections;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tls;

namespace EchoServer
{
    public class Room
    {
        private RoomPark roomInfo;
        public RoomPark GetRoomInfo
        {
            get
            {
                roomInfo.RoomName = roomInfo.RoomName;
                roomInfo.PlayerNum = clientList.Count;
                return roomInfo;
            }
        }
        /// <summary>
        /// 房间内的所有客户端，第一个客户端为房主
        /// </summary>
        private List<Client> clientList = new List<Client>();


        public Room(Client client, RoomPark park)
        {
            this.roomInfo = park;
            clientList.Add(client);
            client.GetRoom = this;
        }

        public RepeatedField<PlayerPark> GetPlayerInfo()
        {
            RepeatedField<PlayerPark> parks = new RepeatedField<PlayerPark>();
            foreach (Client client in clientList)
            {
                PlayerPark player = new PlayerPark();
                player.PlayerName = client.UserName;
                parks.Add(player);
            }
            return parks;
        }
        /// <summary>
        /// 重要！！！参数中的client表示不发给谁！如果要发给自己就传入空参数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="park"></param>
        public void Broadcast(Client ?client, MainPark park)
        {
            foreach (Client c in clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }
                c.Send(park);
            }
        }
        public void Join(Client client)
        {
            clientList.Add(client);
            client.GetRoom = this;
            //向其他客户端广播加入信息
            MainPark park = new MainPark();
            park.ActionCode = ActionCode.PlayerList;
            foreach (PlayerPark player in GetPlayerInfo())
            {
                park.PlayerPark.Add(player);
            }
            Broadcast(client, park);

        }

        /// <summary>
        /// 离开房间，一般用户离开和房主离开分别判断
        /// </summary>
        /// <param name="client"></param>
        public void Exit(Client client)
        {
            MainPark park = new MainPark();

            //房主离开
            if (client == clientList[0])
            {
                client.GetRoom = null;
                park.ActionCode = ActionCode.Exit;
                Broadcast(client, park);
            }

            //一般用户离开
            clientList.Remove(client);
            client.GetRoom = null;
            //向其他客户端广播退出信息
            park.ActionCode = ActionCode.PlayerList;
            foreach (PlayerPark player in GetPlayerInfo())
            {
                park.PlayerPark.Add(player);
            }
            Broadcast(client, park);
        }

        public MainPark GameStart(Client client)
        {
            MainPark park = new MainPark();
            //不是房主，返回值为失败，不进行广播
            if (client != clientList[0])
            {
                park.RequestCode = RequestCode.Room;
                park.ReturnCode = ReturnCode.Fail;
                park.ActionCode = ActionCode.GameStart;
                return park;
            }

            park.RequestCode = RequestCode.Room;
            park.ReturnCode = ReturnCode.Success;
            park.ActionCode= ActionCode.GameStart;
            foreach (PlayerPark player in GetPlayerInfo())
            {
                park.PlayerPark.Add(player);
            }
            //给不包括自己的服务端广播信息
            Broadcast(client, park);
            //返回给自身的信息
            return park;
        }

        public void UpItem(Client client,MainPark park)
        {

            //不是房主，返回值为失败，不进行广播
            if (client != clientList[0])park.PlayerPark[0].Authority="NUll";
            //是房主，拥有权限
            Broadcast(client, park);
        }
    }
}
