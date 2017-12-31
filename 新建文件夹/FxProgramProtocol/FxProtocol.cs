using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Mitsubishi.FX.Parameters;
using Mitsubishi.FX.Common;
using Mitsubishi.FX.FxProgramProtocol.Common;
using System.Diagnostics;

namespace Mitsubishi.FX.FxProgramProtocol
{
    public class FxProtocol
    {
        public delegate void SendEventHandler(byte[] buffer);

        public delegate void ReviceEventHandler(byte[] buffer);

        /// <summary>
        /// PLC连接智联宝串口号
        /// </summary>
       

        /// <summary>
        /// 发送事件
        /// </summary>
        public event SendEventHandler SendEvent;
        /// <summary>
        /// 接收事件
        /// </summary>
        public event ReviceEventHandler ReviceEvent;

        private FxRingBuffer _RingBuffer;
        private const int MAX_RETRY_READ_COUNT = 5;

        /// <summary>
        /// 块大小
        /// </summary>
        private const int BlockSize = 0x0010;

        public ICellDataType DataType { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        private string HostName { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        private int PortNumber { get; set; }

        private TcpClient Client;


        private NetworkStream Stream { get; set; }



        #region 构造函数
        public FxProtocol(string HostName, int port)
        {

            this.HostName = HostName;
            this.PortNumber = port;
            _RingBuffer = new FxRingBuffer();
        }
        #endregion

        #region 验证ip正确
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>s
        private bool IsCorrectIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-

5]|[01]?\d\d?)$");
        }
        #endregion



        #region Tcp连接
        /// <summary>
        /// Tcp连接
        /// </summary>  
        public void Connect()
        {
            this.Client = new TcpClient();

            if (!this.Client.Connected)
            {
                this.Client.Connect(this.HostName, this.PortNumber);
                this.Stream = this.Client.GetStream();
            }
        }
        protected void Disconnect()
        {

            if (this.Client.Connected)
            {
                this.Client.Close();
            }
        }

        /// <summary>
        /// 驱动与设备是否连接标志
        /// </summary>
        public bool Connected
        {
            get
            {
                if (Client != null)
                {
                    return Client.Connected;
                }
                return false;
            }
        }

        /// <summary>
        /// 智联宝
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            Connect();

            return 0;
        }
        #endregion



        #region 添加美的协议后， tcp 发送 命令，获取返回值
        /// <summary>
        /// 默认是 8bit格式发送
        /// </summary>
        /// <param name="iCommand"></param>
        /// <returns></returns>
        protected FxCommandResponse Send(byte[] iCommand,int SerialPort)
        {
            return Send(iCommand, UInt8DataType.Default, SerialPort);
        }

        protected FxCommandResponse Send(byte[] iCommand, ICellDataType responseDataType,int SerialPort)

        {
            byte[] resultBuff = null;
            byte[] resultByte = new byte[] { };

            FxCommandResponse result = new FxCommandResponse(ResultCodeConst.rcNotSettting,

null, responseDataType);
            int reReadTimes = 0;                                            // 重读次数
            string hh = ByteHelper.ByteToString(iCommand);

            //添加美的协议
            iCommand = MeiCloudCommand.GetMeiCloudSendData(iCommand, SerialPort);
            NetworkStream networkStream = this.Stream;
            string ll = ByteHelper.ByteToString(iCommand);
            networkStream.Write(iCommand, 0, iCommand.Length);
            networkStream.Flush();
            this.SendEvent?.Invoke(iCommand);
            _RingBuffer.Clear();
            while (reReadTimes < MAX_RETRY_READ_COUNT)
            {

                using (var memoryStream = new MemoryStream())
                {
                    var buff = new byte[256];
                    do
                    {
                        int count;
                        try
                        {
                             count = networkStream.Read(buff, 0, buff.Length);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                      
                        if (count == 0)
                        {
                            throw new Exception("PLC被断开");
                        }
                        memoryStream.Write(buff, 0, count);
                    } while (networkStream.DataAvailable);


                    this.ReviceEvent?.Invoke(memoryStream.ToArray());
                    byte[] receiceData = memoryStream.ToArray();
                    string kk = ByteHelper.ByteToString(receiceData);
                    //将结果去掉美的协议,以及端口号
                    if (receiceData.Length > 6)
                    {
                        resultByte = receiceData.Skip(6).Take(receiceData.Length -

7).ToArray();

                        _RingBuffer.Append(resultByte, 0, resultByte.Length);
                        //string str1 =ByteHelper.ByteToString(result);
                        resultBuff = _RingBuffer.PickPackage();//验证报文完整性

                        if (resultByte != null && resultByte.Length > 0)
                        {
                            break;
                        }
                    }

                }

                reReadTimes++;
            }

            // 处理收到的数据
            if (resultBuff == null)
            {
                result.ResultCode = ResultCodeConst.rcFailt;
                Debug.Print("发送命令超时候仍没有收到FX PLC的合法响应.");
            }
            else if (resultBuff.Length == 1)
            {
                result.ResultCode = (ResultCodeConst)resultBuff[0];
            }
            else
            {
                result.ResultCode = ResultCodeConst.rcSuccess;

                List<int> valuelist;
                FxCommandHelper.ParseSmart(resultBuff, 0, responseDataType, out valuelist);

                result.SetResponseValue(valuelist);
                result.SetRawData(resultBuff);

            }

            return result;

        }


        #endregion

        #region  写
        /// <summary>
        /// 适用于X，Y，M寄存器,置位与复位
        /// </summary>
        /// <param name="tagParameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteBit(TagParameter tagParameter, int value,int SerialPort)
        {
            string cmd;
            FxCommandConst v;
            if (value == 0)
            {
                v = FxCommandConst.FxCmdForceOn;
            }
            else
            {
                v = FxCommandConst.FxCmdForceOff;
            }

            string tagAddress = tagParameter.FxAddressType.ToString() +

tagParameter.Address.ToString();
            cmd = FxCommandHelper.Make(v, new FxAddress(tagAddress,

FxAddressLayoutType.AddressLayoutByte));

            byte[] buff = ASCIIEncoding.ASCII.GetBytes(cmd);

            FxCommandResponse result = Send(buff, SerialPort);

            if (result.ResultCode == ResultCodeConst.rcSuccess)
            {
                return true;
            }
            return false;

        }


        public FxCommandResponse Write<T>(TagParameter tagParameter, List<uint> writeValues,int SerialPort)

where T : ICellDataType, new()
        {

            string cmd;
            T t = new T();
            string tagAddressStr = tagParameter.FxAddressType.ToString() +

tagParameter.Address.ToString();

            cmd = FxCommandHelper.Make<T>(FxCommandConst.FxCmdWrite, new FxAddress

(tagAddressStr, ControllerTypeConst.ctPLC_Fx), writeValues);

            byte[] buff = ASCIIEncoding.ASCII.GetBytes(cmd);
            FxCommandResponse result = Send(buff, SerialPort);

            return result;
        }
        public FxCommandResponse Write(TagParameter tagParameter, List<uint> writeValues,int SerialPort)
        {

            if (tagParameter.cellDataType == "UInt8DataType")
            {
                return Write<UInt8DataType>(tagParameter, writeValues, SerialPort);
            }
            else if (tagParameter.cellDataType == "UInt16DataType")
            {
                return Write<UInt16DataType>(tagParameter, writeValues, SerialPort);
            }
            else if (tagParameter.cellDataType == "UInt32DataType")
            {
                return Write<UInt32DataType>(tagParameter, writeValues, SerialPort);
            }

            return null;

        }
        #endregion

        #region 读

        #region 读 8bit*Count
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="tagParameter"></param>
        /// <param name="responseDataType">8,16,32</param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public FxCommandResponse Read(TagParameter tagParameter,int SerialPort, int Count = 1)
        {
            DecieDeDataType(tagParameter);

            ICellDataType responseDataType = tagParameter.CellDataType;

            string tagAddressStr = tagParameter.FxAddressType.ToString() +

tagParameter.Address.ToString();

            string cmd = FxCommandHelper.Make(FxCommandConst.FxCmdRead, new FxAddress

(tagAddressStr, ControllerTypeConst.ctPLC_Fx), Count * responseDataType.DataItemSize);

            byte[] buff = ASCIIEncoding.ASCII.GetBytes(cmd);
            FxCommandResponse result = Send(buff, responseDataType, SerialPort);

            return result;
        }
        #endregion

        #region bit读
        public bool ReadBit(TagParameter tagParameter,int SerialPort, out int value)
        {

            tagParameter.CellDataType = UInt8DataType.Default;
            FxCommandResponse ResponseResult = Read(tagParameter, SerialPort);

            if (ResponseResult.ResponseValue != null && ResponseResult.ResponseValue.Count >

0)
            {
                List<int> values = ResponseResult.ResponseValue;
                int result = values.FirstOrDefault();
                string str = Convert.ToString(result, 2).PadLeft(8, '0');
                int k = tagParameter.Address % 8;
                string resultStr = str[str.Length - 1 - k].ToString();
                value = int.Parse(resultStr);
                return true;

            }
            value = 0;
            return false;

        }
        #endregion

        #region 进制判断

        private void DecieDeDataType(TagParameter parameter)
        {

            if (parameter.cellDataType == "UInt32DataType")
            {
                parameter.CellDataType = UInt32DataType.Default;
            }
            else if (parameter.cellDataType == "UInt16DataType")
            {
                parameter.CellDataType = UInt16DataType.Default;
            }
            else if (parameter.cellDataType == "UInt8DataType")
            {
                parameter.CellDataType = UInt8DataType.Default;
            }
        }


    #endregion

    #endregion


}
}
