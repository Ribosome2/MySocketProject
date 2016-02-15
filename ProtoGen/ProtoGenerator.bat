set OutPath=..\U3DClient\Assets\Scripts\Protocols\
set ServerPath=..\GameServer\GameServer\Packet\proto\
set IntPath=proto\

call :Gen chatapp.proto chatapp.cs
pause
:Gen
protogen -i:%IntPath%%1 -o:%OutPath%%2
protogen -i:%IntPath%%1 -o:%ServerPath%%2
@goto :EOF

