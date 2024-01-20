using EchoProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer.Controller
{
    class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }

        public MainPark UpState(Server server,Client client, MainPark park)
        {
            client.GetRoom.Broadcast(client, park);
            return null;
        }
        public MainPark UpItem(Server server, Client client, MainPark park)
        {
            client.GetRoom.UpItem(client, park);
            return null;
        }
    }
}
