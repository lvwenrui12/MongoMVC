﻿using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoMVC.Models
{
    public class Tag
    {

        public string Name { get; set; }

        /// <summary>
        /// 寄存器
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }



        public int Value { get; set; }

        /// <summary>
        /// 记录tag的接收信息
        /// </summary>
        public List<Respose> Resposes { get; set; } = new List<Respose>();

    }
}