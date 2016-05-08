# MySocketProject
在Unity里面使用Socket通信的框架，
里面有实现一套自定义的decode-encode的功能,但是缺点是要自己写协议代码，
而且没有protobuf压缩数据等功能

后面也实现了一套用protobuf进行协议的序列化功能，基本就可以跟各种语言的服务器通信了

使用说明：
./ProtoGen文件夹下面放协议的定义源文件，
运行 ProtoGenerator.bat 生成相应的C#协议文件

协议结构是 len+protocode+content 
具体用法可以查看聊天实例
