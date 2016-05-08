﻿using System.Net;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
public class SocketController
{
    static SocketController mInstance;

    public static SocketController instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new SocketController();
            }
            return mInstance;
        }
    }


    private const int Send_Time_Out = 3;
    private const int RevTimeOut = 3;
    private const int Header_Length = 4;
    private Socket mSocket = null;
    

    /// <summary>
    /// 尝试连接服务器
    /// </summary>
    /// <param name="address"></param>
    /// <param name="remoteport"></param>
    /// <returns></returns>
    public bool Connnect(string address, int remoteport)
    {
        Debug.Log("try to connect to " + address + " port number "+remoteport);
        if (mSocket != null && mSocket.Connected)
        {
            return true;
        }
        IPHostEntry hostEntry = Dns.GetHostEntry(address);
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(ip, remoteport);

                //创建Socket
                mSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mSocket.BeginConnect(ipe, new System.AsyncCallback(OnConnection), mSocket);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        return true;
    }


    /// <summary>
    /// 连接结果回调
    /// </summary>
    /// <param name="ar"></param>
    private void OnConnection(IAsyncResult ar)
    {
        Debug.Log("Connection callback ");
        //获取服务器Sockect
        Socket serverSocket = ar.AsyncState as Socket;
        try
        {
            //和服务器连接，如果连接失败，这里将会抛出异常
            mSocket.EndConnect(ar);
            mSocket.SendTimeout = Send_Time_Out;
            mSocket.ReceiveTimeout = RevTimeOut;
        
            mSocket.BeginReceive(ByteBuffer.Instance.protobuffBytes, 0, ByteBuffer.Header_Length,
                SocketFlags.None, new System.AsyncCallback(OnReceiveHeader), serverSocket);
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof (SocketException))
            {
                SocketException sockEx = ex as SocketException;
                if (sockEx.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    Debug.LogError("bei拒绝连接");
                }
                Debug.LogError(string.Format("Sockect exception errro code{0} message{1}"+sockEx.SocketErrorCode,ex.Message));
            }
            else
            {
                Debug.LogError(string.Format(" exception  message{0}" +  ex.Message));
            }
        }
    }


    /// <summary>
    /// 接收协议头成功
    /// </summary>
    /// <param name="ar"></param>
    private void OnReceiveHeader(IAsyncResult ar)
    {
        Debug.Log("Start receive header");
        Socket serverSocket = (Socket)ar.AsyncState;
        try
        {
            int read = mSocket.EndReceive(ar);
            if (read < 1)
            {
                OnDisconnect();
            }
            else
            {
                //到这里我们已经得到了协议头，已经可以知道协议长度了，开始接收正式的协议内容
                //这里我们从协议头之后开始接收协议头指示的协议体长度
                mSocket.BeginReceive(ByteBuffer.Instance.protobuffBytes, ByteBuffer.Header_Length, ByteBuffer.Instance.BodyLength,
                SocketFlags.None, new System.AsyncCallback(OnReceiveBody), serverSocket);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Socket expction " + ex.Message);
        }
        
    }


    /// <summary>
    /// 接收协议体内容成功
    /// </summary>
    /// <param name="ar"></param>
    private void OnReceiveBody(IAsyncResult ar)
    {
        Debug.Log("Start receive body");
        Socket serverSocket = (Socket)ar.AsyncState;
        try
        {
            int read = mSocket.EndReceive(ar);
            if (read < 1)
            {
                OnDisconnect();
            }
            else
            {
                //接收协议体完成，立刻从缓存创建一个完整的协议包
                ByteBuffer.Instance.CreatePacketFromBuffer();


                //接收完之后继续接收下一个协议头，循环：  协议头---->协议体---->协议头
                mSocket.BeginReceive(ByteBuffer.Instance.protobuffBytes, 0,ByteBuffer.Header_Length,
                    SocketFlags.None, new System.AsyncCallback(OnReceiveHeader), serverSocket);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Socket expction " + ex.Message+" stackTrace : "+ Environment.StackTrace);
        }
        
    }

    /// <summary>
    /// 失去连接
    /// </summary>
    void OnDisconnect()
    {
        UnityEngine.Debug.LogError ("Connection lost");
    }



    public  void SendBytes(byte[] finalBytes)
    {
        PrintBytes(finalBytes);
        int totalBytes = finalBytes.Length;
        int sendBytes = 0;
        while (sendBytes < totalBytes)
        {
            int sendCount = mSocket.Send(finalBytes, sendBytes, totalBytes - sendBytes, SocketFlags.None);
            sendBytes += sendCount;
        }
    }

   public static  string  ByteArrayToStr(byte[] byteArray)
    {
        string str = "Byte count:  " + byteArray.Length+"   content: ";

        for (int i = 0; i < byteArray.Length; i++)
        {
            str += byteArray[i]+" - ";
        }
        return str;
    }

   public static  void PrintBytes(Byte[] byteArray)
    {
        string str = "Byte count:  " + byteArray.Length+"  content : ";
        
        for (int i = 0; i < byteArray.Length; i++)
        {
            str += byteArray[i] + " - ";
        }
        Debug.Log("Byte details:  " + str);
    }
}
