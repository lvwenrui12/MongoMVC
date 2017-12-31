using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitsubishi.FX.Common
{
    public class ByteHelper
    {

        public static byte[] MergerArray(byte[] First, byte[] Second)
        {
            byte[] result = new byte[First.Length + Second.Length];
            First.CopyTo(result, 0);
            Second.CopyTo(result, First.Length);
            return result;
        }

        public static string ByteToString(byte[] bytes)

        {

            StringBuilder strBuilder = new StringBuilder();

            foreach (byte bt in bytes)

            {

                strBuilder.AppendFormat("{0:X2} ", bt);

            }

            return strBuilder.ToString();

        }

        public static byte[] StringToByte(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);

        }

        /// <summary>
        /// 将结果的ASCII转换成相对的二进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AsciiToBin(byte[] data)
        {

            string s = Encoding.ASCII.GetString(data);
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in s)
            {
                int aaa = Convert.ToInt32(item.ToString(), 16);
                string k = Convert.ToString(aaa, 2).PadLeft(4, '0');
                strBuilder.Append(k);
            }
            return strBuilder.ToString();

        }



    }
}
