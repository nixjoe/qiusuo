using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Common.Protocol;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;
using ProtoBuf;
using ProtoBuf.Meta;
using RPGServer.Manager;
using LogManager = ExitGames.Logging.LogManager;

namespace RPGServer
{
    public class RPGApplication : ApplicationBase
    {
        #region 字段与属性

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// GameServerApplication实例
        /// </summary>
        public new static RPGApplication Instance { get; private set; }

        /// <summary>
        /// 服务器名字,应用名称
        /// </summary>
        public string ServerName;

        /// <summary>
        /// 服务器启动id，多用于标识启动的唯一实例
        /// </summary>
        public Guid ServerGuid;

        /// <summary>
        /// 服务器唯一标识符
        /// </summary>
        public static readonly int ServerUID = 1;

        #endregion 字段与属性

        #region 构造函数

        /// <summary>
        /// 构造函数,线程管理,时间初始化
        /// </summary>
        public RPGApplication()
        {
            Instance = this;
            //TODO:姚茂新 线程管理
            Time.InitTime();
        }

        #endregion 构造函数

        #region 重载Photon函数

        #region Setup

        protected override void Setup()
        {
            ServerName = ApplicationName;
            ServerGuid = Guid.NewGuid();

            SetupLog();
            Log.InfoFormat("服务器运行中:ServerName==>{0} ServerUID==>{1}  ServerGuid==>{2}", ServerName, ServerUID, ServerGuid);

            if (!SetSkillData())
            {
                Log.Error("技能数据初始化错误...");
                return;
            }
            StartThread();
            PrepareProtocol();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Log.ErrorFormat("UnHandledException, sender:{0}, args:{1}", sender, args);
            Log.Info("ManagedThreadId ==> " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        ///     配置日志文件
        /// </summary>
        private void SetupLog()
        {
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(Path.Combine(ApplicationRootPath, "bin_Win64"), "log"); //设置日志目录
            var path = Path.Combine(BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            XmlConfigurator.ConfigureAndWatch(file);
            Log.Info("日志配置文件设置成功");
        }

        /// <summary>
        /// 初始化技能s数据
        /// </summary>
        /// <returns></returns>
        private bool SetSkillData()
        {
            Log.Info("初始化技能数据...");
            long time = DateTime.Now.Ticks;
            if (!BaseDataMgr.Instance.InitBaseConfigData())
            {
                return false;
            }

            Log.Info("技能数据初始化完成 time:" + TimeSpan.FromTicks(DateTime.Now.Ticks - time));
            return true;
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        private void StartThread()
        {
            //TODO:姚茂新启动线程
            Log.Info("启动线程");
        }

        /// <summary>
        /// 预热协议
        /// </summary>
        private void PrepareProtocol()
        {
            Log.Info("预热协议");
            return;
            Stopwatch sw = new Stopwatch(); sw.Start();
            // 预热所有协议
            var model = RuntimeTypeModel.Default;
            model.Add(typeof(object), true);

            Assembly t = Assembly.Load("MobaProtocol");//装载程序集
            foreach (Type type in t.GetTypes())
            {
                if (type.Namespace != null && (type.Namespace.Contains("MobaProtocol.Data")))
                {
                    if (type.GetCustomAttributes(typeof(ProtoContractAttribute), false).Length > 0)
                    {
                        model.Add(type, true);
                        Log.Info("Add type: " + type);
                    }
                }
            }

            model.AllowParseableTypes = true;
            model.AutoAddMissingTypes = true;
            model.CompileInPlace();
            Log.Info("prepare serialize used " + sw.ElapsedMilliseconds); sw.Restart();
        }

        #endregion Setup

        #region CreatePeer

        /// <summary>
        /// IPhotonApplication.OnInit之后调用
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new RPGPeer(initRequest);
        }

        #endregion CreatePeer

        #region OnStopRequested

        protected override void OnStopRequested()
        {
            Log.InfoFormat("ServerName==>{0} ServerUID==>{1}  ServerGuid==>{2}", ServerName, ServerUID, ServerGuid);
            // TODO:姚茂新 用户状态这里是否有通告Lobby
            //            Log.InfoFormat("OnStopRequested: ServerUID={0}", ServerUID);
            //            m_gameWorld.SafeStopGame();
            //
            //            GameThreadMgr.Instance.StopThreads();
            //
            //            if (PvpLobbyConnectionList != null && PvpLobbyConnectionList.Count != 0)
            //            {
            //                foreach (var value in PvpLobbyConnectionList.Values)
            //                {
            //                    value.Dispose();
            //                }
            //            }

            base.OnStopRequested();
        }

        #endregion OnStopRequested

        #region TearDown

        protected override void TearDown()
        {
            Log.InfoFormat("服务器关闭:ServerName==>{0} ServerUID==>{1}  ServerGuid==>{2}", ServerName, ServerUID, ServerGuid);
        }

        #endregion TearDown

        #endregion 重载Photon函数

        #region 广播消息

        public void BroadClienMessage<TPeer>(IEnumerable<TPeer> peers, PvpCode code, params object[] args) where TPeer : PeerBase
        {
            Dictionary<byte, object> param = comm.BuildParams(args);
            if (param == null)
                param = new Dictionary<byte, object>();
            var data = new EventData() { Code = (byte)code, Parameters = param };
            BroadCastEvent(data, peers, new SendParameters());//TODO:姚茂新按协议广播?
        }

        public void SendAllLobbyMessage(PvpCode code, params object[] args)
        {
            //            if (PvpLobbyConnectionList == null || PvpLobbyConnectionList.Count == 0)
            //            {
            //                Log.ErrorFormat("PvpLobbyConnectionList 为空 code {0} args {1}", code, args);
            //                return;
            //            }
            //            foreach (var tmpConnection in PvpLobbyConnectionList.Values)
            //            {
            //                var result = tmpConnection.GetPeer().SendOperationRequest((byte)code, args);
            //                if (result != SendResult.Ok)
            //                {
            //                    Log.ErrorFormat("[send][lobby]SendAllLobbyMessage failed, code:{0}, result:{1}",
            //                        code, result);
            //                }
            //            }
        }

        public SendResult SendLobbyMessage(string lobbykey, PvpCode code, params object[] args)
        {
            //            if (string.IsNullOrEmpty(lobbykey) || lobbykey.Equals(""))
            //            {
            //                Log.ErrorFormat("lobbykey 为空 code {0}", code);
            //                return SendResult.Failed;
            //            }
            //
            //            PvpLobbyConnection tmpConnection = null;
            //            if (PvpLobbyConnectionList.TryGetValue(lobbykey, out tmpConnection))
            //            {
            //                var result = tmpConnection.GetPeer().SendOperationRequest((byte)code, args);
            //                if (result != SendResult.Ok)
            //                {
            //                    Log.ErrorFormat("[send][lobby] failed, code:{0}, result:{1} serverkey {2}",
            //                        code, result, lobbykey);
            //                }
            //                Log.InfoFormat("[send][lobby] success, code:{0}, result:{1} serverkey {2}",
            //                        code, result, lobbykey);
            //                return result;
            //            }
            return SendResult.Disconnected;
        }

        #endregion 广播消息
    }
}