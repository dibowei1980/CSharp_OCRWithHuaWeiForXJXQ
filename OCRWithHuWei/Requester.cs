using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCRWithHuWei
{
    using System.Net;
    public static class Requester
    {
        static Requester()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        }
    }
}
