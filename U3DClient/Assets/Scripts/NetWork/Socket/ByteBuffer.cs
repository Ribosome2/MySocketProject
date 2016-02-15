using System;
using UnityEngine;
using System.Collections;

public class ByteBuffer
{
    private static ByteBuffer mInstance;
    public static ByteBuffer Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new ByteBuffer();
            }
            return mInstance;
        }
    }
    
    /// <summary>
    /// 协议头的长度，这几个字节我们用来标记每个协议的总字节长度
    /// </summary>
    public readonly int Header_Length= 4;
    public  Byte[] protobuffBytes=new byte[1024*1024];

   

    /// <summary>
    /// 从协议头获取的协议实际内容长度，这里要注意，一定是先接收正确的协议头这个长度才是正确的长度
    /// </summary>
    public int BodyLength
    {
        get
        {
            byte[] lenthBytes=new byte[4];
            Array.Copy(protobuffBytes, lenthBytes, lenthBytes.Length);
            int bodyLength= BitConverter.ToInt32(lenthBytes, 0);
            return bodyLength;
        }
    }



    /// <summary>
    /// 在刚好接收完一个完整的协议的时候调用，构造一个完整的协议包
    /// </summary>
    public void CreatePacketFromBuffer()
    {
        byte[] packetBytes = new byte[BodyLength];
        //从缓存里获取属于这个协议的内容
        Array.Copy(protobuffBytes,Header_Length, packetBytes,0, packetBytes.Length);
        byte[] protoCodeBytes = new byte[4];
        Array.Copy(packetBytes, 0, protoCodeBytes, 0, protoCodeBytes.Length); //协议的前面四位为协议号
        int protoCode = BitConverter.ToInt32(protoCodeBytes, 0);

        ProtoBase_S2C decoder = ProtoMapper.GetProtoDecoder(protoCode);
        if (decoder!= null)
        {
            decoder.Decode(packetBytes);
            ProtoPacketQueueManager.instance.AddPacket(decoder);
            //用队列在帧更新的时候处理协议，是因为有可能是在多线程下通信，而有些操作是只能在主线程执行
        }

    }
    /// <summary>
    /// 在刚好接收完一个完整的协议的时候调用，构造一个完整的协议包(这个函数用protoBuf来序列化数据）
    /// </summary>
    public void CreatePacketFromBuffer2()
    {
        byte[] packetBytes = new byte[BodyLength];
        //从缓存里获取属于这个协议的内容
        Array.Copy(protobuffBytes, Header_Length, packetBytes, 0, packetBytes.Length);
        byte[] protoCodeBytes = new byte[4];
        Array.Copy(packetBytes, 0, protoCodeBytes, 0, protoCodeBytes.Length); //协议的前面四位为协议号
        int protoCode = BitConverter.ToInt32(protoCodeBytes, 0);

        Type decoder = ProtoMapper.GetProtobufDecodeType(protoCode);
        if (decoder != null)
        {
            System.IO.MemoryStream stream_output = new System.IO.MemoryStream(packetBytes);

            //？？？？？？？？？？？？？？ 这里这个泛型的语法怎么写？ 还没搞明白
            //decoder = ProtoBuf.Serializer.Deserialize<decoder.GetType()>(stream_output);
           // ProtoPacketQueueManager.instance.AddPacket(decoder);
            //用队列在帧更新的时候处理协议，是因为有可能是在多线程下通信，而有些操作是只能在主线程执行
        }

    }
    

}
