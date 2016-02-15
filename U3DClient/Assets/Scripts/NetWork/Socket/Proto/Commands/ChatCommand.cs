using UnityEngine;
using System.Collections;

public class ChatReq
{
    public static int protoCode=10000;
    public ChatApp.UserReq data=new ChatApp.UserReq();
}


public class ChatResp
{
    public static  int protoCode=10000;
    public ChatApp.UserResp data = new ChatApp.UserResp();
}
