START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml

XCOPY /Y GenPackets.cs "../../ChatClient/Packet/"
XCOPY /Y GenPackets.cs "../../Server/Packet/"

XCOPY /Y ClientPacketManager.cs "../../ChatClient/Packet/"
XCOPY /Y ServerPacketManager.cs "../../Server/Packet/"