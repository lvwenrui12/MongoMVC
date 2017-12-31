using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MongoMVC.Models
{
    public class PLC
    {

        public string  name { get; set; }
    
        /// <summary>
        /// IP地址
        /// </summary>
        public IPAddress ip { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int iPortNumber { get; set; }

        /// <summary>
        /// 串口
        /// </summary>
        public int serialPort { get; set; }


        public List<Tag> tags { get; set; }
    }
}