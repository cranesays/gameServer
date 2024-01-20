using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using EchoProtocol;

namespace EchoServer
{
    public class Message
    {
        private byte[] buffer = new byte[1024];
        private int startIndex;
        public byte[] Buffer { get { return buffer; } }
        public int StartIndex { get { return startIndex; } }
        public int RemainSize { get { return buffer.Length - startIndex; } }
        public void ReadBuff(int size, Action<MainPark> HandleRequest)
        {
            startIndex += size;
            if (startIndex <= 4) return; //判断每条消息是否小于或等于包头长度
            int count = BitConverter.ToInt32(buffer, 0);//解析包头，包头的内容为包体的长度
            //循环用来解决粘包问题？
            while (true)
            {
                if (startIndex >= (count + 4))
                {
                    MainPark park = (MainPark)MainPark.Descriptor.Parser.ParseFrom(buffer, 4, count);
                    HandleRequest(park);
                    //被复制的数组，复制开始索引，接收数据的数组，复制到目标的索引，需要复制的长度
                    Array.Copy(buffer, count + 4, buffer, 0, startIndex - count - 4);
                    startIndex -= (count + 4);
                }
                else
                {
                    break;//考虑换位continue
                }
            }
        }
        public static byte[] ParkData(MainPark park)
        {
            byte[] data = park.ToByteArray();
            byte[] head = BitConverter.GetBytes(data.Length);
            return head.Concat(data).ToArray();
        }
    }
}
