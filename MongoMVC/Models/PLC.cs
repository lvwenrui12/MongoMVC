﻿using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MongoMVC.Models
{
    public class PLC: Entity
    {

        public string  Name { get; set; }
    
        /// <summary>
        /// IP地址
        /// </summary>
        public IPAddress Ip { get; set; }

        /// <summary>
        /// 协议
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int IPortNumber { get; set; }

        /// <summary>
        /// 串口
        /// </summary>
        public int SerialPort { get; set; }


        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}