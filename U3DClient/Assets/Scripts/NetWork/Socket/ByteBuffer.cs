using System;
using ProtoBuf.Meta;
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
    /// 现在就用 1  2  4  这几个字节 
    /// </summary>
    public const int Header_Length= 2;

    public const int ProtoCodeLength = 2;
    public  Byte[] protobuffBytes=new byte[1024*1024];

   

    /// <summary>
    /// 从协议头获取的协议实际内容长度，这里要注意，一定是先接收正确的协议头这个长度才是正确的长度
    /// </summary>
    public int BodyLength
    {
        get
        {
            if (Header_Length == 1)
            {
                return protobuffBytes[0];
            }else if (Header_Length == 2)
            {
                return   BitConverter.ToInt16(protobuffBytes, 0);
            }else if (Header_Length == 4)
            {
                return BitConverter.ToInt32(protobuffBytes, 0);
            }
            else
            {
                Debug.LogError("不支持的头长度"+Header_Length);
                return 0;
            }
        }
    }
    public int ProtoCode
    {
        get
        {
            if (ProtoCodeLength == 1)
            {
                return protobuffBytes[Header_Length];
            }
            else if (ProtoCodeLength == 2)
            {
                return BitConverter.ToInt16(protobuffBytes, Header_Length);
            }
            else if (ProtoCodeLength == 4)
            {
                return BitConverter.ToInt32(protobuffBytes, Header_Length);
            }
            else
            {
                Debug.LogError("不支持的协议号 头长度" + ProtoCodeLength);
                return 0;
            }
        }
    }


    /// <summary>
    /// 在刚好接收完一个完整的协议的时候调用，构造一个完整的协议包
    /// </summary>
    public void CreatePacketFromBuffer()
    {
        try
        {
            int bodyLen = BodyLength;
           
            int protoCode = ProtoCode;
            object decoder = ProtoMapper.GetProtoDecoder(protoCode);
            if (decoder != null)
            {
                byte[] dataBytes = new byte[bodyLen - ProtoCodeLength]; //协议体等于总的协议体长度减去协议号字节
                Array.Copy(protobuffBytes, Header_Length+ProtoCodeLength, dataBytes, 0, dataBytes.Length);
                Debug.Log("Content :"+SocketController.ByteArrayToStr(dataBytes));
                System.IO.MemoryStream stream_output = new System.IO.MemoryStream(dataBytes);

                RuntimeTypeModel.Default.Deserialize(stream_output, decoder, decoder.GetType());
                
                ProtoPacketQueueManager.AddPacket(decoder);
                //用队列在帧更新的时候处理协议，是因为有可能是在多线程下通信，而有些操作是只能在主线程执行
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("协议包创建出错："+ex.Message+ex.StackTrace);
        }
       

    }
    
    

}
