using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoMVC.Models
{
    public class Respose
    {
        /// <summary>
        /// 
        /// </summary>
        public string  Ip { get; set; }

        /// <summary>
        /// 寄存器
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public int  Address { get; set; }

        /// <summary>
        /// 返回的时间
        /// </summary>
        public string ResposeTime { get; set; } = DateTime.Now.ToString();
    }
}