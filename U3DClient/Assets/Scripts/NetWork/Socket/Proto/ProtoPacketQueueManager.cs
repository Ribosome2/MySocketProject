using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacketHandle
{
    public Type packetType;
    public ProtoPacketQueueManager.OnHanldePacketData handleFuntion;
}

/// <summary>
/// 消息包队列处理类，每帧检测是有消息未处理
/// </summary>
public class ProtoPacketQueueManager : MonoBehaviour
{

    private static ProtoPacketQueueManager mInstance;

    public static ProtoPacketQueueManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject("protoPacketQueue").AddComponent<ProtoPacketQueueManager>();
            }
            return mInstance;
        }
    }

    private static   List<object> mListProtoPacket = new List<object>();

   

    public delegate void OnHanldePacketData(object data);

    public List<PacketHandle> mListPacketDataHandle = new List<PacketHandle>();





    /// <summary>
    /// 在Update处理所有的协议，避免多线程可能引起的不能在协议回调的时候操作UI或者其他的只能在主线程做的事情
    /// </summary>
    void Update()
    {
        while (mListProtoPacket.Count > 0)
        {
            object protoData = mListProtoPacket[0];
            HandlePacket(protoData);
            mListProtoPacket.RemoveAt(0);
        }
    }


    public static void AddPacket(object packet)
    {
        mListProtoPacket.Add(packet);
    }


    /// <summary>
    /// 处理协议包
    /// </summary>
    /// <param name="resp"></param>
    void HandlePacket( object resp)
    {
        //触发所有侦听了这个协议的函数
        for (int i = 0; i < mListPacketDataHandle.Count; i++)
        {
            PacketHandle handle = mListPacketDataHandle[i];
            if (handle.packetType == resp.GetType())
            {
                if (handle.handleFuntion != null)
                {
                    handle.handleFuntion(resp);
                }
            }
        }
    }

    public void AddHandleListener(Type packetType,OnHanldePacketData handleFunction)
    {
        PacketHandle handle = new PacketHandle();
        handle.packetType = packetType;
        handle.handleFuntion = handleFunction;
        mListPacketDataHandle.Add(handle);
    }

    public void AddHandleListener<T>( OnHanldePacketData handleFunction)
    {
        AddHandleListener(typeof(T),handleFunction);
    }

    public void RemoveHandleListener(OnHanldePacketData handleFunction)
    {
        for (int i = 0; i < mListPacketDataHandle.Count; i++)
        {
            PacketHandle handle = mListPacketDataHandle[i];
          
            if (handle.handleFuntion ==handleFunction)
            {
                mListPacketDataHandle.Remove(handle);
            }
        }
    }
    
}
