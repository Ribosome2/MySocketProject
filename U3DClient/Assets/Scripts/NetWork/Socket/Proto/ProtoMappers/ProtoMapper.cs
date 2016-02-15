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

    /// <summary>
    /// 协议类型和回调委托列表
    /// </summary>
    public static Dictionary<Type, List<Action>> mProtoCallBackDict = new Dictionary<Type, List<Action>>();

    public static void Initilize()
    {
        AddProtoToMap(10000, typeof (Chat_S2C));
        AddProtoToMap(10001, typeof(Chat_S2C));
    }

    /// <summary>
    /// 在映射里面添加协议号对应的解析类
    /// </summary>
    /// <param name="protoCode"></param>
    /// <param name="decoder"></param>
    static void AddProtoToMap(int protoCode, Type decoderType)
    {

        //if (decoderType.BaseType != typeof(ProtoBase_S2C))
        //{
        //    Debug.LogError(decoderType + "is not a valid decoder");
        //}

        if (mProtoDictionary.ContainsKey(protoCode))
        {
            Debug.LogError("Proto " + protoCode + " already map to decoder " + mProtoDictionary[protoCode]);
        }
        else
        {
            mProtoDictionary.Add(protoCode,decoderType);
        }
    }


    /// <summary>
    /// 添加指定的类型协议 回调处理函数，同个类型的同一个函数只能注册一次事件，防止重复
    /// </summary>
    /// <param name="protoType"></param>
    /// <param name="callBackFunc"></param>
    public void AddTypeCallBack(Type protoType, Action callBackFunc)
    {
        if(!mProtoCallBackDict.ContainsKey(protoType)) //还没有这个类的映射就添加
        {
            mProtoCallBackDict.Add(protoType, new List<Action>());

            //第一次添加的时候要顺便初始化协议号对应的类
            BindingFlags bindingFlag = BindingFlags.Static;
            FieldInfo codeField = protoType.GetField("protoCode", bindingFlag);
            if (codeField == null)
            {
                Debug.LogError("类" + protoType.Name + "没有协议号或者协议号属性错误");
                return;
            }
            else
            {
                int protoCode =(int) codeField.GetValue(null);
                AddProtoToMap(protoCode, protoType);
            }
           
        }
        List<Action> callBackList = mProtoCallBackDict[protoType];
        
        if (callBackList.Contains(callBackFunc)) //同一个函数不可以多次注册事件，
        {
            Debug.LogError("ProtoType " + protoType.Name + " try to register duplicate callBack " + callBackFunc);
        }
        else
        {
            callBackList.Add(callBackFunc);//但同一个协议类型可以有多个回调
        }
    }


    public static ProtoBase_S2C GetProtoDecoder(int protoCode)
    {
        if (!mProtoDictionary.ContainsKey(protoCode))
        {
            Debug.LogError("Proto " + protoCode + " doesn't have any decoder ");
            return null;
        }
        else
        {
            Type decoderType=mProtoDictionary[protoCode];
            
            ProtoBase_S2C decoder = null;
            decoder = decoderType.Assembly.CreateInstance(decoderType.Name) as ProtoBase_S2C;
            
            return  decoder;
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

}
