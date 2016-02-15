using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 请求协议基类
/// </summary>
public class  ProtoBufBaseReq
{
    public Type data; //这里要限制一个泛型的话要怎么写

}

/// <summary>
/// 服务器返回协议基类
/// </summary>
public abstract class ProtoBufBaseResp
{
    
}
