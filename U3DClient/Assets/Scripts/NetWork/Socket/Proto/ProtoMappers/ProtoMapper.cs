#define USE_PROTOLBUF   //定义是否用protobuff解析协议
using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 协议映射工具类，用来指定什么协议由什么类来进行解析
/// </summary>
public partial class ProtoMapper
{
    private static Dictionary<int, Type> mProtoDictionary = new Dictionary<int, Type>();
    static Dictionary<Type,int> protoTypeDict=new Dictionary<Type, int>();

  

    public static void Initilize()
    {
        AddProtoToMap<ChatApp.UserReq>(0);
        AddProtoToMap<ChatApp.UserResp>(1);
    }

    /// <summary>
    /// 在映射里面添加协议号对应的解析类
    /// </summary>
    /// <param name="protoCode"></param>
    /// <param name="decoder"></param>
    static void AddProtoToMap<T>(int protoCode)
    {
        if (mProtoDictionary.ContainsKey(protoCode))
        {
            Debug.LogError("Proto " + protoCode + " already map to decoder " + mProtoDictionary[protoCode]);
        }
        else
        {
            mProtoDictionary.Add(protoCode,typeof(T));
            protoTypeDict.Add(typeof(T),protoCode);
        }
    }


   
     
    
    public static object GetProtoDecoder(int protoCode)
    {
        if (!mProtoDictionary.ContainsKey(protoCode))
        {
            Debug.LogError("Proto " + protoCode + " doesn't have any decoder ");
            return null;
        }
        else
        {
            Type decoderType=mProtoDictionary[protoCode];
            object targetObj = decoderType.Assembly.CreateInstance(decoderType.FullName);
            return  targetObj;
        }
    }

    /// <summary>
    /// 获取解析指定协议
    /// </summary>
    /// <param name="protoCode"></param>
    /// <returns></returns>
    public static Type GetProtobufDecodeType(int protoCode)
    {
        if (!mProtoDictionary.ContainsKey(protoCode))
        {
            Debug.LogError("Proto " + protoCode + " doesn't have any decoder ");
            return null;
        }
        else
        {
            Type decoderType=mProtoDictionary[protoCode];
            return  decoderType;
        }
    }

    /// <summary>
    /// 获取解析指定类型的协议号
    /// </summary>
    /// <param name="protoCode"></param>
    /// <returns></returns>
    public static int GetProtoCode(Type type)
    {
        int protoCode = 0;
        if (protoTypeDict.TryGetValue(type, out protoCode)==false)
        {
            Debug.LogError("没有注册类型" + type);
        }
        return protoCode;
    }


}
