using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;
using System.Reflection;

namespace EchoServer
{
    class UserController:BaseController
    {
        public UserController() 
        { 
            requestCode = RequestCode.User;

        }
        public MainPark Logon(Server server,Client client,MainPark park)
        {
            if(client.GetUserData.Logon(park,client.GetMySqlConnection))
            {
                park.ReturnCode= ReturnCode.Success;
            }
            else
            {
                park.ReturnCode = ReturnCode.Fail;
            }
            return park;
        }
        public MainPark Login(Server server,Client client, MainPark park)
        {
            if (client.GetUserData.Login(park, client.GetMySqlConnection))
            {
                park.ReturnCode = ReturnCode.Success;
                client.UserName = park.LoginPark.Username;
            }
            else
            {
                park.ReturnCode = ReturnCode.Fail;
            }
            return park;
        }
    }
}
