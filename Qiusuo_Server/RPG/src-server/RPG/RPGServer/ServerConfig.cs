using System;
using System.IO;
using System.Xml;
using ExitGames.Logging;

namespace RPGServer
{
    public enum ServerType
    {
        PvpServer = 0,
        PveServer = 1
    }

    public class ServerConfig
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        //base
        public string ServerName { get; protected set; }

        public int ServerIndex { get; protected set; }
        public string PublicIp { get; protected set; }
        public int GamingTcpPort { get; protected set; }
        public int GamingUdpPort { get; protected set; }
        public ServerType MyServerType { get; protected set; }

        /// <summary>
        /// TODO:姚茂新先true
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public bool Init(string configPath)
        {
            return true;
            Log.Warn("InitServerConfig: " + configPath);
            MyServerType = ServerType.PvpServer;

            try
            {
                //read base
                var configFile = new FileInfo(configPath);
                FileStream configStream = configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configStream);
                XmlNode _rootNode = xmlDoc.SelectSingleNode("setup");
                //base
                var baseNode = _rootNode["base"];
                ServerIndex = int.Parse(baseNode["ServerIndex"].InnerText);
                var pubipNode = baseNode["PublicIp"];
                if (pubipNode != null)
                    PublicIp = pubipNode.InnerText;
                else
                    PublicIp = "127.0.0.1";

                var name = baseNode["ServerName"];
                if (name != null)
                    ServerName = name.InnerText;
                else
                    ServerName = "noset_" + PublicIp;

                GamingTcpPort = int.Parse(baseNode["GamingTcpPort"].InnerText);
                GamingUdpPort = int.Parse(baseNode["GamingUdpPort"].InnerText);
                var nodeServerType = baseNode["ServerType"];
                if (nodeServerType != null)
                    MyServerType = (ServerType)Enum.Parse(typeof(ServerType), nodeServerType.InnerText);
                configStream.Close();
                return true;
            }
            catch (IOException exception)
            {
                Log.Error("Failed to open XML SkillMainConfig file [" + configPath + "]", exception);
            }

            return false;
        }
    }
}