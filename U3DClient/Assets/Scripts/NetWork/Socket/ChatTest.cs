using ChatApp;
using UnityEngine;
using System.Collections;

public class ChatTest : MonoBehaviour
{
    public string serverIp="127.0.0.1";
    public int serverPort=9090;

    public int userId = 740;
    public string  chatText = "this is a name";

    public string name="Kele";
    public int age=55;
    

	// Use this for initialization
	void Start ()
	{
	    ProtoMapper.Initilize();
	    ProtoPacketQueueManager.instance.AddHandleListener<ChatApp.UserResp>(OnChatResp);
	    EncodeAndDecodeTest();

	}

    private void OnChatResp(object o)
    {
        ChatApp.UserResp resp = o as ChatApp.UserResp;
        Debug.Log("resp"+resp.chat);
    }

  
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUILayout.Button("Connect"))
        {
            SocketController.instance.Connnect(serverIp, serverPort);
        }
        GUILayout.Label("User id");
        userId =int.Parse( GUILayout.TextField(userId.ToString()));
        GUILayout.Label("name to send");
        chatText = GUILayout.TextField(chatText);
        if (GUILayout.Button("Send"))
        {
            SendGetInfo();
        }


        if (GUILayout.Button("Get player Info"))
        {
            SendGetInfo();
        }

    }

    void EncodeAndDecodeTest()
    {
        Chat_C2S chat = new Chat_C2S();
        chat.userId = userId;
        chat.text = chatText;
        Debug.Log("origin data " + chat.ToString());

        Chat_C2S chatNew = new Chat_C2S();
        chatNew.Decode(chat.Encode().ToArray());
        Debug.Log("new data " + chatNew.ToString());


    }

   

    void SendGetInfo()
    {
        ChatApp.UserReq req=new UserReq();
        req.id = 12;
       CommandController.Send(req);
    }

    void OnChatResp(ProtoBase_S2C data)
    {
        Chat_S2C chat = (Chat_S2C) data;
        Debug.Log("chat:  userId  " + chat.userId +"   chat name " + chat.text);

    }


    void OnGetPlayerInfoResp(ProtoBase_S2C data)
    {
        GetUserInfo_S2C info = (GetUserInfo_S2C)data;
        Debug.Log("get player info :  userId  " + info.userId + "    name " + info.name+" age "+info.age);

    }

}
