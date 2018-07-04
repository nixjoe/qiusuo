using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;

namespace RPGServer.Peer
{
    public class UdpDriver : UdpDriverBase
    {
        private object threadLock = new object();

        public UdpDriver(RPGPeer _peer) : base(_peer)
        {
        }

        public void AddPkg(byte code, int _channelId, int _length, byte[] _pkg)
        {
            lock (threadLock)
            {
                if (!pkgMatrix.ContainsKey(_channelId))
                {
                    pkgMatrix.Add(_channelId, new UdpPackage[maxPkgsPerChannel]);
                    channelArr.Add(_channelId, new ChannelInfo(_channelId));
                }
                long newSeqNo = channelArr[_channelId].AllocSeqNo();
                if (channelArr[_channelId].Full())
                {// if packages cache pool is full
                    ////Log.Error("UDP Driver: pkg pool is full...");
                    peer.Disconnect();
                    return;
                }
                ////Log.Error("====================AddPkg:" + code + " c: " + channelId + " Seqno:" + newSeqNo + " len:" + length + " fristS:" + channelArr[channelId].firstSeqno + " lastS:" + channelArr[channelId].lastSeqno);
                pkgMatrix[_channelId][SeqnoIdx(newSeqNo)] = new UdpPackage(code, _channelId, newSeqNo, _length, _pkg);
                //AlignSeqNo(newSeqNo, channelId);
            }
        }

        /*
         * send pkgs that have not be sent
         * be called by Application in the end of frame
         */

        public void Flush()
        {
            lock (threadLock)
            {
                int allSendLen = 0;
                sendIndex++;
                //apply not sent pkgs
                TryFlush();
                //apply lost pkgs
                allSendLen += TryResendLostPkg();

                allSendLen += PrepairTimeoutPkg(allSendLen);
                if (sendPkgs.Count > 0)
                    //Log.Error("==========================>>>>>>>>>>>>>>>>>>Flush End: Len:" + allSendLen + " pkgCnt:" + sendPkgs.Count);
                    //send pkgs finally
                    SendPkgs();

                //end....
                ClearCacheList();
            }
        }

        /*
         * pkgs arrived
         */

        public void OnRecv(OperationRequest operationRequest)
        {
            lock (threadLock)
            {
                try
                {
                    if (operationRequest.OperationCode == OperationCode_Ack)
                    {
                        OnAck(operationRequest.Parameters);
                    }
                    else
                    {
                        OnRecvMsg(operationRequest);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("UdpDriver OnRecv:" + ex.ToString());
                }
            }
        }

        public void OnDisconnect()
        {
            lock (threadLock)
            {
                foreach (var channel in channelArr.Values)
                {
                    if (!channel.Empty())
                    {
                        channel.Reset();
                    }
                }

                if (_calcedSeqNoSet != null)
                {
                    _calcedSeqNoSet.Clear();
                }
            }
        }

        public void OnUpdate()
        {
        }
    }

    public class UdpDriverBase
    {
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected Dictionary<int, UdpPackage[]> pkgMatrix;
        protected Dictionary<int, ChannelInfo> channelArr; // array for store channel info
        protected long sendIndex = 0;
        protected RPGPeer peer;
        private SendParameters sendParam = new SendParameters();

        static public int maxPkgsPerChannel = 500;
        protected const int maxPkgsOnceSend = 40;
        protected const int maxBytesOnceSend = 480;

        protected const byte OperationCode_Ack = 1;
        protected const byte OperationCode_Other = 2;
        protected int maxSeqnoRecved = 0;

        private int[] resendDelay = { 70 * 10000, 100 * 10000, 400 * 10000, 1000 * 10000, 3000 * 10000 };

        protected int maxTimeoutArrLen = 30;
        protected int maxTimeoutTimeDelay = 37;
        protected List<UdpPackage>[] timeoutArr;
        protected HashSet<int> _calcedSeqNoSet = null;

        public UdpDriverBase(RPGPeer _peer)
        {
            peer = _peer;
            pkgMatrix = new Dictionary<int, UdpPackage[]>();
            channelArr = new Dictionary<int, ChannelInfo>();
            sendParam.Unreliable = true;
            sendParam.Flush = true;
            ackParam = new Dictionary<byte, object>();
            ackResponse = new OperationResponse(OperationCode_Ack, ackParam);
            timeoutArr = new List<UdpPackage>[maxTimeoutArrLen];
            for (int i = 0; i < maxTimeoutArrLen; i++)
            {
                timeoutArr[i] = new List<UdpPackage>();
            }
            _calcedSeqNoSet = new HashSet<int>();
            for (int i = 0; i < 1000; ++i) //InitPool increase capacity of hashset
            {
                _calcedSeqNoSet.Add(i);
            }
            _calcedSeqNoSet.Clear();
        }

        protected int GetSendDelayTime(int resendCnt)
        {
            if (resendCnt > resendDelay.Length)
            {
                return resendDelay[resendDelay.Length - 1];
            }
            else
            {
                return resendDelay[resendCnt - 1];
            }
        }

        public static int SeqnoIdx(long _seqNo)
        {
            return (int)(_seqNo % (long)maxPkgsPerChannel);
        }

        protected void AlignSeqNo(long _newSeqNo, int _channelId)
        {
            //channelArr[channelId].AlignSeqNo(_newSeqNo);
        }

        protected List<UdpPackage> sendPkgs = new List<UdpPackage>();
        private List<UdpPackage> couldApplyPkgs = new List<UdpPackage>();

        protected int TryFlush()
        {
            int sendLen = 0;
            long nowTime = DateTime.Now.Ticks;
            sendPkgs.Clear();
            couldApplyPkgs.Clear();
            foreach (var channel in channelArr.Values)
            {
                if (channel.Empty()) continue; //当前channel为空的
                UdpPackage[] pkgs = pkgMatrix[channel.channelID];
                for (long seqIdx = channel.firstSeqno; seqIdx < channel.lastSeqno; seqIdx++)
                {
                    UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                    if (pkg == null) continue;
                    if (pkg.sendIndex == sendIndex) continue; //当前发送队列已经标记发送
                    if (pkg.sentCnt > 0) continue;
                    ////Log.Error("----------------------- TryFlush: " + pkg.channelID + " SeqNo:" + pkg.seqNo);
                    sendLen += pkg.length;
                    sendPkgs.Add(pkg);
                    pkg.sendIndex = sendIndex;
                }
            }
            //SendPkgs(sendPkgs, couldApplyPkgs);
            if (sendPkgs.Count > 0)
            {
                ////Log.Error("----------------------- TryFlush: " + sendPkgs.Count);
            }
            return sendLen;
        }

        /*
         * 发送一些还没收到ACK，但是该channel新的pkg缺收到了ACK，所以基本可以断定此类pkg 丢掉了,需要重发
         */

        protected int TryResendLostPkg()
        {
            int sendLen = 0;
            int oldCnt = sendPkgs.Count;
            foreach (var channel in channelArr.Values)
            {
                if (!channel.Empty())
                {
                    UdpPackage[] pkgs = pkgMatrix[channel.channelID];
                    for (long seqIdx = channel.firstSeqno; seqIdx < channel.lastSeqno; seqIdx++)
                    {
                        UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                        if (pkg.sendIndex == sendIndex) continue; //当前发送队列已经标记发送
                        if (pkg != null && pkg.bLost)
                        {
                            //Log.Error("----------------------- TryLost: " + pkg.channelID + " SeqNo:" + pkg.seqNo);
                            pkg.sendIndex = sendIndex;
                            sendPkgs.Add(pkg);
                            sendLen += pkg.length;
                        }
                    }
                }
                //SendPkgs(sendPkgsEx, null);
            }
            int moreCnt = sendPkgs.Count - oldCnt;
            if (moreCnt > 0)
            {
                ////Log.Error("----------------------- TryLost: " + moreCnt);
            }
            return sendLen;
        }

        /*
         * 尝试发送超时的
         */

        protected int TryResendTimeoutPkg(long factor1 = 0, bool useResendDelayTimeArr = true, bool forceResend = false)
        {
            long nowTime = DateTime.Now.Ticks;
            int oldCnt = sendPkgs.Count;
            int sendLen = 0;
            foreach (var channel in channelArr.Values)
            {
                if (!channel.Empty())
                {
                    UdpPackage[] pkgs = pkgMatrix[channel.channelID];
                    for (long seqIdx = channel.firstSeqno; seqIdx < channel.lastSeqno; seqIdx++)
                    {
                        UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                        if (pkg.sendIndex == sendIndex) continue; //当前发送队列已经标记发送
                        if (pkg != null && !pkg.isAckBack && pkg.sentCnt > 0)
                        {
                            long diffTime = pkg.sentTime + (peer.RoundTripTime + factor1) * 10000;
                            if (useResendDelayTimeArr)
                            {
                                diffTime += GetSendDelayTime(pkg.sentCnt);
                            }
                            if (diffTime < nowTime)
                            {
                                ////Log.Error("----------------------- TryTimeout: " + pkg.channelID + " SeqNo:" + pkg.seqNo + " DelayTime:" + ((nowTime - pkg.sentTime) / 10000));
                                pkg.sendIndex = sendIndex;
                                sendPkgs.Add(pkg);
                                sendLen += pkg.length;
                            }
                        }
                    }
                }
            }
            int moreCnt = sendPkgs.Count - oldCnt;
            if (moreCnt > 0)
            {
                ////Log.Error("----------------------- TryTimeout: " + moreCnt + " factor:" + factor1);
            }
            return sendLen;
        }

        protected int PrepairTimeoutPkg(int sendLen)
        {
            long nowTime = DateTime.Now.Ticks;
            foreach (var channel in channelArr.Values)
            {
                if (!channel.Empty())
                {
                    UdpPackage[] pkgs = pkgMatrix[channel.channelID];
                    for (long seqIdx = channel.firstSeqno; seqIdx < channel.lastSeqno; seqIdx++)
                    {
                        UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                        if (pkg.sendIndex == sendIndex) continue; //当前发送队列已经标记发送
                        if (pkg != null && !pkg.isAckBack && pkg.sentCnt > 0)
                        {
                            long diffTime = (nowTime - (pkg.sentTime + peer.RoundTripTime * 10000)) / 10000;
                            if (diffTime < 0) continue;
                            long idx = diffTime / maxTimeoutTimeDelay;
                            if (idx >= maxTimeoutArrLen)
                            {
                                idx = maxTimeoutArrLen - 1;
                            }

                            timeoutArr[idx].Add(pkg);

                            pkg.sendIndex = sendIndex;
                        }
                    }
                }
            }

            int selfSendLen = 0;
            int index = (maxTimeoutArrLen - 1);
            bool bStop = false;
            for (; index >= 2; index--)
            {
                List<UdpPackage> pkgList = timeoutArr[index];
                for (int n = 0; n < pkgList.Count; n++)
                {
                    UdpPackage pkg = pkgList[n];
                    if (bStop)
                    {
                        couldApplyPkgs.Add(pkg);
                    }
                    else
                    {
                        sendPkgs.Add(pkg);
                        ////Log.Error("----------------------- TryTimeout: " + pkg.channelID + " SeqNo:" + pkg.seqNo + " DelayTime:" + ((nowTime - pkg.sentTime) / 10000));
                        selfSendLen += pkg.length;
                    }
                    if ((selfSendLen + sendLen) > 1500)
                    {
                        bStop = true;
                    }
                }
                if (bStop) break;
                if (index <= 4 && selfSendLen > 800)
                {//如果包只超时了一点点　４＊３７范围内，并且已经发送了超过８００ｂｙｔｅ　就不要再多发了
                    break;
                }
                if (selfSendLen > 1200)
                {
                    break;
                }
            }
            for (; index >= 0; index--)
            {
                List<UdpPackage> pkgList = timeoutArr[index];
                for (int n = 0; n < pkgList.Count; n++)
                {
                    UdpPackage pkg = pkgList[n];
                    couldApplyPkgs.Add(pkg);
                    ////Log.Error("----------------------- TryCouldApply: " + pkg.channelID + " SeqNo:" + pkg.seqNo + " DelayTime:" + ((nowTime - pkg.sentTime) / 10000));
                }
            }
            return selfSendLen;
        }

        protected void ClearCacheList()
        {
            for (int i = 0; i < maxTimeoutArrLen; i++)
            {
                timeoutArr[i].Clear();
            }
            couldApplyPkgs.Clear();
        }

        protected void TryFindCouldApplyPkgs(long delayTime)
        {
            if (sendPkgs.Count == 0) return;
            long nowTime = DateTime.Now.Ticks;
            if (true) //找到那些适合附带一起发送的，为了合理利用mtu
            {
                foreach (var channel in channelArr.Values)
                {
                    if (channel.Empty()) continue; //当前channel为空的
                    UdpPackage[] pkgs = pkgMatrix[channel.channelID];
                    int curCntOfChannel = 0;
                    for (long seqIdx = channel.firstSeqno; seqIdx < channel.lastSeqno; seqIdx++)
                    {
                        if (curCntOfChannel >= 5) break;
                        UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                        if (pkg == null) continue;
                        if (pkg.sendIndex == sendIndex) continue; //当前发送队列已经标记发送
                        //if (i < channel.firstFrame || channel.lastFrame <= i) continue; //遍历的帧号 当前channel不包含
                        if (pkg == null || pkg.isAckBack || pkg.sentTime == 0 || (pkg.sentTime + (peer.RoundTripTime + delayTime) * 10000) > nowTime) continue; //如果这个刚发送不久,则不需要重发

                        ////Log.Error("----------------------- TryCouldApplyPkgs: " + pkg.channelID + " SeqNo:" + pkg.seqNo);
                        couldApplyPkgs.Add(pkg);
                        curCntOfChannel++;
                    }
                }
                if (couldApplyPkgs.Count > 0)
                {
                    ////Log.Error("----------------------- TryCouldApplyPkgs: " + couldApplyPkgs.Count);
                }
            }
        }

        /*
         * 发送pkgs的接口
         * _sendPkgs：必须发送的pkg
         * _couldApplyPkgs: 为了补足mtu，可以附加发送的pkg
         */

        protected void SendPkgs()
        {
            int sentLen = 0;
            sendPkgsEx.Clear();
            for (int i = 0; i < sendPkgs.Count; i++)
            {
                UdpPackage pkg = sendPkgs[i];
                if (((sentLen + pkg.length) > maxBytesOnceSend && sendPkgsEx.Count > 0) || sendPkgsEx.Count > 50)
                {
                    SendPkgsEx(sendPkgsEx);
                    sentLen = 0;
                    sendPkgsEx.Clear();
                }
                sendPkgsEx.Add(pkg);
                sentLen += pkg.length;
            }
            if (sentLen > 0)
            {
                if (couldApplyPkgs != null)
                {
                    for (int i = 0; i < couldApplyPkgs.Count; i++)
                    {
                        UdpPackage pkg = couldApplyPkgs[i];
                        if (((sentLen + pkg.length) > maxBytesOnceSend && sendPkgsEx.Count > 0) || sendPkgsEx.Count > 50)
                        {
                            break; // 只有最后一个包需要补充到mtu大小
                        }
                        else
                        {
                            sendPkgsEx.Add(pkg);
                        }
                    }
                }
                SendPkgsEx(sendPkgsEx);
                sentLen = 0;
                sendPkgsEx.Clear();
            }
        }

        private List<UdpPackage> sendPkgsEx = new List<UdpPackage>();
        /*
         * 发送pkgs的最底层接口
         */

        private void SendPkgsEx(List<UdpPackage> _sendPkgs)
        {
            Dictionary<byte, object> param = new Dictionary<byte, object>();
            OperationResponse response = new OperationResponse((byte)OperationCode_Other, param);
            long nowTime = DateTime.Now.Ticks;

            ////Log.Error("====================SendPkgs:" + _sendPkgs.Count);
            int beginIdx = 1;
            int pkgCount = 0;
            int range = 4;
            for (int i = 0; i < _sendPkgs.Count; i++)
            {
                UdpPackage sPkg = _sendPkgs[i];
                param[(byte)(beginIdx + range * i + 0)] = (long)(sPkg.channelID * 1000 + sPkg.code);
                param[(byte)(beginIdx + range * i + 1)] = sPkg.seqNo;
                param[(byte)(beginIdx + range * i + 2)] = nowTime;
                param[(byte)(beginIdx + range * i + 3)] = sPkg.msg;
                sPkg.sentCnt++;
                sPkg.sentTime = nowTime;
                sPkg.bLost = false;
                pkgCount++;
                ////Log.Error("====================SendPkgs:" + pkgCount + " code:" + sPkg.code + " c: " + sPkg.channelID + " SeqNo:" + sPkg.seqNo + " SentTime:" + nowTime);
            }
            param[0] = pkgCount;
            //...do send...
            SendResult ret = peer.SendOperationResponse(response, sendParam);
            _sendPkgs.Clear();
        }

        //==============================floor above: Recv pkg From Client============================
        protected void OnAck(Dictionary<byte, object> ackParam)
        {
            byte count = (byte)ackParam[0];
            byte idx = 1;

            ////Log.Error("====================OnAck count:" + count);
            for (byte i = 0; i < count; i++)
            {
                int channelID = (int)ackParam[idx++];
                long seqNo = (long)ackParam[idx++];
                long sentTime = (long)ackParam[idx++];
                OnAck(channelID, seqNo, sentTime);
            }
        }

        private void OnAck(int channelID, long seqNo, long sentTime)
        {
            ////Log.Error("====================OnAck detail: channelID: " + channelID + " seqNo: " + seqNo + " sentTime:" + sentTime);
            UdpPackage[] pkgArr;
            if (pkgMatrix.TryGetValue(channelID, out pkgArr))
            {
                ChannelInfo cInfo = channelArr[channelID];
                if (cInfo.InRange(seqNo))
                {
                    if (sentTime == 0)
                    {
                        SetAllAckBack(channelID, seqNo);
                    }
                    else
                    {
                        UdpPackage pkg = pkgArr[SeqnoIdx(seqNo)];
                        if (!pkg.isAckBack)
                        {
                            pkg.SetAckBacked(sentTime);
                        }
                        SetLostFlag(cInfo, pkgArr, seqNo, sentTime);
                    }
                    AssignSeqno(channelID, seqNo);
                }
            }
        }

        protected void OnRecvMsg(OperationRequest _req)
        {
            int seqNo = Convert.ToInt32(_req.Parameters[1]);
            if (maxSeqnoRecved < seqNo)
            {
                maxSeqnoRecved = seqNo;
            }
            SendAck();

            if (_calcedSeqNoSet.Contains(seqNo))
            {
                return;
            }

            if (_calcedSeqNoSet.Count < 1000)
            {
                _calcedSeqNoSet.Add(seqNo);
            }
            else
            {
                int cachedMinSeqNo = _calcedSeqNoSet.Min();
                if (seqNo > cachedMinSeqNo)
                {
                    _calcedSeqNoSet.Remove(cachedMinSeqNo);
                    _calcedSeqNoSet.Add(seqNo);
                }
            }
        }

        private Dictionary<byte, object> ackParam = null;
        private OperationResponse ackResponse = null;

        private void SendAck()
        {
            ackParam[0] = maxSeqnoRecved;
            SendResult ret = peer.SendOperationResponse(ackResponse, sendParam);
            ////Log.Error("====================SendAck:" + maxSeqnoRecved);
            if (ret != SendResult.Ok)
            {
                ////Log.Error("SendAck Error:" + ret);
            }
        }

        private void SetAllAckBack(int channelID, long seqNoRecved)
        {
            ChannelInfo channel = channelArr[channelID];
            if (channel.Empty()) return; //当前channel为空的
            UdpPackage[] pkgs = pkgMatrix[channelID];
            for (long seqIdx = (maxSeqnoRecved - 1); seqIdx >= channel.firstSeqno; seqIdx--)
            {
                UdpPackage pkg = pkgs[SeqnoIdx(seqIdx)];
                if (pkg != null && !pkg.isAckBack)
                {
                    pkg.SetAckBacked(0);
                }
            }
            channel.firstSeqno = seqNoRecved;
        }

        private void AssignSeqno(int channelID, long seqnoRecved)
        {
            ChannelInfo cInfo = channelArr[channelID];
            UdpPackage[] pkgs = pkgMatrix[channelID];
            if (cInfo.firstSeqno == seqnoRecved)
            {
                long i = seqnoRecved;
                for (i = seqnoRecved; i < cInfo.lastSeqno; i++)
                {
                    int idx = SeqnoIdx(i);
                    if (pkgs[idx] == null || pkgs[idx].isAckBack)
                    {
                        cInfo.firstSeqno = i;
                    }
                    else
                    {
                        break;
                    }
                }
                cInfo.firstSeqno = i;
            }
        }

        private void SetLostFlag(ChannelInfo cInfo, UdpPackage[] pkgs, long seqNoRecved, long sentTime)
        {
            for (long i = (seqNoRecved - 1); i >= cInfo.firstSeqno; i--)
            {
                int idx = SeqnoIdx(i);
                UdpPackage pkg = pkgs[idx];
                if (pkg != null && !pkg.isAckBack && !pkg.bLost && pkg.sentTime < sentTime)
                {
                    pkg.bLost = true;
                }
            }
        }
    }

    public class UdpPackage
    {
        public byte code = 0;
        public int channelID = 0; //channel No of sent
        public long sentTime = 0; // last time of sent
        public int sentCnt = 0; // count of resent
        public long seqNo = 0; //
        public bool isAckBack = false; //client是否已经收到
        public long sentTimeAckBack; //收到ACK里带的this.sentTime
        public bool bLost = false; //其他pkg在收到ack时 可以判断之前的包是不是lost了
        public int length = 0;  //length of pkg to CastUnit
        public long sendIndex = 0;
        public byte[] msg;

        public void SetAckBacked(long _sentTime)
        {
            isAckBack = true;
            bLost = false;
            sentTimeAckBack = _sentTime;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="channelId"></param>
        /// <param name="seqNo"></param>
        /// <param name="length"></param>
        /// <param name="msg"></param>
        public UdpPackage(byte code, int channelId, long seqNo, int length, byte[] msg)
        {
            this.code = code;
            channelID = channelId;
            this.length = length;
            this.seqNo = seqNo;
            this.msg = msg;
        }
    }

    public class ChannelInfo
    {
        public int channelID;
        public long firstSeqno = 0;
        public long firstSeqnoIter = 0;
        public long lastSeqno = 0;

        public long AllocSeqNo()
        {
            return lastSeqno++;
        }

        public ChannelInfo(int _channelID)
        {
            channelID = _channelID;
        }

        public bool Full()
        {
            return UdpDriver.SeqnoIdx(firstSeqno) == UdpDriver.SeqnoIdx(lastSeqno + 1);
        }

        public bool Empty()
        {
            return firstSeqno == lastSeqno;
        }

        public bool InRange(long _seqNo)
        {
            return _seqNo >= firstSeqno && _seqNo < lastSeqno;
        }

        public void Reset()
        {
            firstSeqno = 0;
            lastSeqno = 0;
        }

        public void AlignSeqNo(long _newSeqNo)
        {
            if (Empty())
            {
                firstSeqno = _newSeqNo;
                lastSeqno = _newSeqNo + 1;
            }
            else
            {
                if (_newSeqNo >= lastSeqno)
                {
                    lastSeqno = _newSeqNo + 1;
                }
            }
        }
    }
}