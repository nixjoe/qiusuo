using System;
using System.Collections.Generic;
using Common.Protocol;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using RPGServer.Peer;

namespace RPGServer
{
    /// <summary>
    /// peer基类,集成公有
    /// </summary>
    public class RPGPeer : ClientPeer
    {
        #region 字段与属性

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// UDP驱动,短连接
        /// </summary>
        private readonly UdpDriver _udpDriver;

        /// <summary>
        /// 保存所有的客户端
        /// </summary>
        private static readonly Dictionary<string, RPGPeer> PeerDict = new Dictionary<string, RPGPeer>();

        /// <summary>
        /// 实际数量
        /// </summary>
        public static int RealCount;

        /// <summary>
        /// 全局唯一标识符（分配）
        /// </summary>
        public Guid PeerGuid { get; protected set; }

        /// <summary>
        /// 发送的参数
        /// </summary>
        public SendParameters SendParams;

        /// <summary>
        /// 时间同步参数
        /// </summary>
        public SendParameters timeSyncParams;

        /// <summary>
        /// 错误的请求
        /// </summary>
        public int WrongRequestNum = 0;

        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 获取或设置udp协议的通道id 1。
        /// </summary>
        private const int ChannelId1 = 5;

        /// <summary>
        /// 获取或设置udp协议的通道id 2。
        /// </summary>
        private const int ChannelId2 = 6;

        #endregion 字段与属性

        /// <summary>
        /// 客户端连接过来进行操作
        /// </summary>
        /// <param name="initRequest"></param>
        public RPGPeer(InitRequest initRequest) : base(initRequest)
        {
            PeerGuid = Guid.NewGuid();
            _udpDriver = new UdpDriver(this);
            RoomId = -1;
            lock (PeerDict)
            {
                PeerDict.Add(PeerGuid.ToString(), this);
            }
            SendParams.ChannelId = 0;//设置udp协议的通道id
            Log.InfoFormat("客户端连接:PeerGuid => {0},ConnectionId => {1},RemoteIP => {2}:{3}", PeerGuid, initRequest.ConnectionId, initRequest.RemoteIP, initRequest.RemotePort);
            PrintPeerState();
        }

        /// <summary>
        /// 输出所有的连接的,链接过来的,实际在连接的peer
        /// </summary>
        public static void PrintPeerState()
        {
            lock (PeerDict)
            {
                int connectedCount = 0;
                foreach (var peer in PeerDict.Values)
                {
                    if (peer != null && peer.Connected)
                    {
                        connectedCount++;
                    }
                }
                Log.InfoFormat("当前连接/连接总数: {0}/{1}", connectedCount, PeerDict.Count);
            }
        }

        /// <summary>
        /// 客户端断线处理
        /// </summary>
        /// <param name="reasonCode"></param>
        /// <param name="reasonDetail"></param>
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (!Enum.IsDefined(typeof(DisconnectReason), reasonCode))
                throw new ArgumentOutOfRangeException(nameof(reasonCode));
            Log.Info("peer disconnect, PeerGuid:" + PeerGuid + ", reasonCode:" + reasonCode);
            lock (PeerDict)
            {
                PeerDict.Remove(PeerGuid.ToString());
            }
            _udpDriver.OnDisconnect();
            Dispose();
        }

        /// <summary>
        /// 处理来自客户端的请求
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (Log.IsDebugEnabled)
                Log.DebugFormat("==> GameClientPeer :OnOperationRequest: pid={0}, op={1}", ConnectionId, (PvpCode)operationRequest.OperationCode);

            //处理不可靠操作代码
            if (sendParameters.Unreliable)
            {
                OnRecvTimeSync(operationRequest);
            }
            else
            {
                //TODO:处理可靠操作代码
            }
        }

        public void Update()
        {
            _udpDriver?.OnUpdate();
        }

        public void UdpDriverFlush()
        {
            _udpDriver?.Flush();
        }

        #region 发送数据接口

        public SendResult Send(byte code, int _channelId, int _length, byte[] _pkg)
        {
            _udpDriver.AddPkg(code, _channelId, _length, _pkg);
            return SendResult.Ok;
        }

        /// <summary>
        /// 发送数据，供类外调用接口
        /// </summary>
        /// <param name="response"></param>
        public SendResult SendResponse(OperationResponse response, ChannelType channelType = ChannelType.NormalRelivableUdp, int channelID = 0)
        {
            SendResult result;
            {
                SendParams.ChannelId = (byte)channelID;
                result = SendOperationResponse(response, SendParams);
            }

            if (result == SendResult.Ok)
                return result;
            Log.ErrorFormat("RPGPeer. Send {0} wrong, error={1}, guid:{2}, remoteIp:{3}, connectionId:{4}", response.OperationCode, result, PeerGuid, RemoteIP + ":" + RemotePort, ConnectionId);
            return result;
        }

        private void ChoseChannelID(ChannelType channelType)
        {
            if (channelType == ChannelType.NormalRelivableUdp)
            {
                SendParams.ChannelId = ChannelId1;
                SendParams.Unreliable = false;
            }
            else
            {
                SendParams.ChannelId = ChannelId2;
                SendParams.Unreliable = false;
            }
        }

        public bool SendMessageSteam(byte code, byte[] bt, ChannelType channelType = ChannelType.NormalRelivableUdp, int channelID = 0)
        {
            SendResult result;
            if ((int)channelType == (int)ChannelType.CustomRelivableUdp)
            {
                result = this.Send(code, channelID, bt.Length, bt);
            }
            else
            {
                Dictionary<byte, object> param = new Dictionary<byte, object>();
                param[0] = bt;
                OperationResponse response = new OperationResponse((byte)code, param);

                result = SendResponse(response, channelType, channelID);
            }
            if (result != SendResult.Ok)
            {
                Log.Error("@SendMessage error:" + code + "  result:" + result.ToString());
                return false;
            }
            return true;
        }

        private void OnRecvTimeSync(OperationRequest operationRequest)
        {
            timeSyncParams.Unreliable = true;
            Dictionary<byte, object> param = new Dictionary<byte, object>();
            param[0] = operationRequest.Parameters[0];
            param[1] = operationRequest.Parameters[1];
            param[2] = (long)(DateTime.Now.Ticks / (long)10000);
            OperationResponse response = new OperationResponse(3, param);
            SendOperationResponse(response, timeSyncParams);
        }

        #endregion 发送数据接口
    }
}