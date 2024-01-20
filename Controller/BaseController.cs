using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;

namespace EchoServer
{
    abstract class BaseController
    {
        protected RequestCode requestCode;

        public RequestCode GetRequestCode { get { return requestCode; } }
    }
}
