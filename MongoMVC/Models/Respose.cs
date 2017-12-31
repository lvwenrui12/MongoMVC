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
        public string  ip { get; set; }

        /// <summary>
        /// 寄存器
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public int  address { get; set; }

        /// <summary>
        /// 返回的时间
        /// </summary>
        public string resposeTime { get; set; } = DateTime.Now.ToString();
    }
}