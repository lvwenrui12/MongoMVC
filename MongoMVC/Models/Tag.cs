using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoMVC.Models
{
    public class Tag
    {

        public string name { get; set; }

        /// <summary>
        /// 寄存器
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }



        public int   value { get; set; }

    }
}