using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPackets;

        static string packetNames;
        static ushort packetId;

        static void Main(string[] args)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (XmlReader xml = XmlReader.Create("PDL.xml", xmlReaderSettings))
            {
                xml.MoveToContent();

                while (xml.Read())
                {
                    if (xml.Depth == 1 && xml.NodeType == XmlNodeType.Element)
                        ParsePacket(xml);
                }
            }
            string fileText = string.Format(PacketFormat.fileFormat, packetNames, genPackets);
            File.WriteAllText("GenPackets.cs", fileText);
        }

        public static void ParsePacket(XmlReader xml)
        {
            if (xml.NodeType != XmlNodeType.Element)
                return;

            if (xml.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = xml["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> tuple = ParseMembers(xml);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, tuple.Item1, tuple.Item2, tuple.Item3);
            packetNames += string.Format(PacketFormat.fileEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";
        }

        public static Tuple<string, string, string> ParseMembers(XmlReader xml)
        {
            string packetName = xml["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = xml.Depth + 1;
            while (xml.Read())
            {
                if (xml.Depth != depth)
                    break;

                string memberName = xml["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = xml.Name.ToLower();
                switch (memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> tuple = ParseList(xml);
                        memberCode += tuple.Item1;
                        readCode += tuple.Item2;
                        writeCode += tuple.Item3;
                        break;
                    default:
                        break;
                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        private static Tuple<string, string, string> ParseList(XmlReader xml)
        {
            string listName = xml["name"];
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> tuple = ParseMembers(xml);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                tuple.Item1,
                tuple.Item3,
                tuple.Item2);
            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string FirstCharToUpper(string listName)
        {
            return Char.ToUpper(listName[0]) + listName.Substring(1);
        }

        public static string FirstCharToLower(string listName)
        {
            return Char.ToLower(listName[0]) + listName.Substring(1);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

    }
}
