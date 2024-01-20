using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;
using EchoServer.Controller;

namespace EchoServer
{
    public class ControllerManager
    {
        private Dictionary<RequestCode,BaseController> controllerDict = new Dictionary<RequestCode,BaseController>();
        private Server server;
        public ControllerManager(Server server) 
        {
            this.server = server;
            UserController userController= new UserController();
            controllerDict.Add(userController.GetRequestCode, userController);
            RoomController roomController = new RoomController();
            controllerDict.Add(roomController.GetRequestCode, roomController);
            GameController gameController = new GameController();
            controllerDict.Add(gameController.GetRequestCode, gameController);
        }
        /// <summary>
        /// 处理请求//使用了反射
        /// </summary>
        public void HandleRequest(MainPark park,Client client)
        {
            if (controllerDict.TryGetValue(park.RequestCode, out BaseController? controller))
            {
                string methodname = park.ActionCode.ToString();
                MethodInfo ?method = controller.GetType().GetMethod(methodname);
                if(method == null) 
                {
                    Console.WriteLine("没有找到指定的事件处理" + park.ActionCode.ToString());
                    return;
                }
                object[] obj = new object[] { server, client, park };
                object? ret = method.Invoke(controller,obj);
                if(ret != null) 
                {
                    client.Send(ret as MainPark);
                }
            }
            else 
            {
                Console.WriteLine("没有找到对应的controller处理");
            }
        }
    }
}
