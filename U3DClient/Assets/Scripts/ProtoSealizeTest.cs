using UnityEngine;
using System.Collections;

public class ProtoSealizeTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ChatApp.UserResp user = new ChatApp.UserResp();
        user.name = "用户";
        user.chat = "Hell,propobuf";

        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize<ChatApp.UserResp>(stream, user);

        //这个字节数组最终使用的时候应该是从网络或者文件得到的
        byte[] bytes = stream.ToArray();
        System.IO.MemoryStream stream_output = new System.IO.MemoryStream(bytes);
        ChatApp.UserResp userData = ProtoBuf.Serializer.Deserialize<ChatApp.UserResp>(stream_output);

        Debug.Log("解析后" + userData.name + " content" + userData.chat);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
