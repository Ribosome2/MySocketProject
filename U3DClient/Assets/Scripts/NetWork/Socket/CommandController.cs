using System;
using System.Collections.Generic;
using ProtoBuf.Meta;
using UnityEngine;
using System.Collections;

public class CommandController  {

    public static void Send(object o)
    {
        int code = ProtoMapper.GetProtoCode(o.GetType());
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        RuntimeTypeModel.Default.Serialize(stream, o); //先序列出协议体
        byte[] contentBytes = stream.ToArray();
        List<byte> byteList=new List<byte>();
        byteList.AddRange(contentBytes);

        //添加协议号
        byte[] bs = protoCodeToBytes(code);
        byteList.InsertRange(0,bs);
        

        //添加协议长度信息
        byte[] lenBytes = protoLengthToBytes(byteList.Count);
         Debug.Log("总长度"+lenBytes.Length);
        byteList.InsertRange(0,lenBytes);
       
        SocketController.instance.SendBytes(byteList.ToArray());

    }

    static byte[] protoCodeToBytes(int protoCode)
    {
        if (ByteBuffer.ProtoCodeLength==1)
        {
            return new byte[] {(byte)protoCode };
        }else if (ByteBuffer.ProtoCodeLength==2)
        {
            return BitConverter.GetBytes((Int16)protoCode);
        }else if (ByteBuffer.ProtoCodeLength == 4)
        {
            return BitConverter.GetBytes(protoCode);
        }
        else
        {
            Debug.LogError("不支持协议号长度");
            return null;
        }
    }

    static byte[] protoLengthToBytes(int length)
    {
        if (ByteBuffer.Header_Length== 1)
        {
            return new byte[] { (byte)length };
        }
        else if (ByteBuffer.Header_Length == 2)
        {
            return BitConverter.GetBytes((Int16)length);
        }
        else if (ByteBuffer.Header_Length == 4)
        {
            return BitConverter.GetBytes(length);
        }
        else
        {
            Debug.LogError("不支持协议号长度");
            return null;
        }
    }

}
