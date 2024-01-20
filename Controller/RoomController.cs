using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;

namespace EchoServer
{
    class RoomController : BaseController
    {
        public RoomController() 
        {
            requestCode = RequestCode.Room;
        }
        public MainPark CreateRoom(Server server,Client client,MainPark park)
        {
            return server.CreateRoom(client,park);
        }
        public MainPark SearchRoom(Server server,Client client,MainPark park) 
        {
            return server.SearchRoom(); 
        }
        public MainPark JoinRoom(Server server, Client client, MainPark park)
        {
            return server.JoinRoom(client,park);
        }
        public MainPark Exit(Server server, Client client, MainPark park)
        {
            return server.ExitRoom(client, park);
        }
        public MainPark GameStart(Server server, Client client, MainPark park)
        {
            return client.GetRoom.GameStart(client);
        }
    }
}
