namespace Common.Protocol
{
    /// <summary>
    /// 系统命令
    /// 协议规则
    /// 0-10系统预留
    /// 11-50共用部分
    /// 51-255各协议专有
    /// </summary>
    public enum MobaSysCode : byte
    {
        SysCode0 = 0,

        SysCode9 = 9
    }

    /// <summary>
    /// 共享命令
    /// </summary>
    public enum MobaShareCode : byte
    {
        ShareStart = 10,

        /// 加入队列
        /// </summary>
        JoinQueue,

        /// <summary>
        /// 取消排队()
        /// </summary>
        OutQueue,

        /// <summary>
        /// pvpserver通知lobby可以开启了(int roomId)
        /// </summary>
        StartGame,

        /// <summary>
        /// 查询人物pvp状态
        /// </summary>
        QueryPvpState,

        /// <summary>
        /// 查询修正角色状态
        /// </summary>
        RefreshPlayerPvpState,

        /// <summary>
        /// 强制退出pvp,测试通道
        /// </summary>
        ForceQuitPvp,

        /// <summary>
        /// lobby关服修复提示
        /// </summary>
        LobbyShutdown,

        /// <summary>
        /// 玩家异常退出
        /// </summary>
        OnUserQuit,

        /// <summary>
        /// pvp玩家下线(int newUid)
        /// </summary>
        PlayerOutline,

        /// <summary>
        /// pvp玩家上线(int newuid)
        /// </summary>
        Playerconnect,

        ShareEnd = 50
    }

    public enum LobbyCode : byte
    {
        #region share part

        /// <summary>
        /// 加入队列(byte gameType,int battleId)
        /// </summary>
        C2L_JoinQueue = MobaShareCode.JoinQueue,

        /// <summary>
        /// 加入队列(byte gameType,int battleId, int ServerId, string Uid)
        /// </summary>
        G2L_JoinQueue = MobaShareCode.JoinQueue,

        /// <summary>
        /// 加入队列结果(int battleId,byte joinType, byte isOk)
        /// </summary>
        L2C_JoinQueue = MobaShareCode.JoinQueue,

        L2S_JoinQueue = MobaShareCode.JoinQueue,
        S2L_JoinQueue = MobaShareCode.JoinQueue,

        /// <summary>
        /// 取消排队()
        /// </summary>
        C2L_OutQueue = MobaShareCode.OutQueue,

        /// <summary>
        /// 取消排队(byte result)
        /// </summary>
        L2C_OutQueue = MobaShareCode.OutQueue,

        /// <summary>
        /// 游戏开始进入pvpserver(byte[]) PvpStartGameInfo
        /// </summary>
        L2C_StartGame = MobaShareCode.StartGame,

        /// <summary>
        /// 查询人物pvp状态
        /// </summary>
        C2L_QueryPvpState = MobaShareCode.QueryPvpState,

        /// <summary>
        /// 查询人物返回（byte state）
        /// </summary>
        L2C_QueryPvpState = MobaShareCode.QueryPvpState,

        /// <summary>
        /// 查询修正角色状态
        /// </summary>
        RefreshPlayerPvpState = MobaShareCode.RefreshPlayerPvpState,

        /// <summary>
        /// 强制退出pvp,测试通道
        /// </summary>
        C2L_ForceQuitPvp = MobaShareCode.ForceQuitPvp,

        L2C_ForceQuitPvp = MobaShareCode.ForceQuitPvp,

        /// <summary>
        /// lobby关闭
        /// </summary>
        L2C_LobbyShutdown = MobaShareCode.LobbyShutdown,

        /// <summary>
        /// 玩家异常退出
        /// </summary>
        C2L_OnUserQuit = MobaShareCode.OnUserQuit,

        /// <summary>
        /// 玩家pvp离线
        /// </summary>
        L2S_PlayerOutline = MobaShareCode.PlayerOutline,

        /// <summary>
        /// 玩家pvp上线
        /// </summary>
        L2S_Playerconnect = MobaShareCode.Playerconnect,

        #endregion share part

        LobbyStart = MobaShareCode.ShareEnd,

        /// <summary>
        /// 登录(string account, string password)
        /// </summary>
        C2L_LoginLobby,

        C2L_FakeLoginLobby,      //假登录(string account,int uid,int serverId)

        /// <summary>
        /// 登录结果(byte isOk)
        /// </summary>
        L2C_LoginLobby,

        /// <summary>
        /// 队伍加入队列(int battleId)
        /// </summary>
        JoinQueueTeam,

        S2L_JoinQueueTeam = JoinQueueTeam,
        L2S_JoinQueueTeam = JoinQueueTeam,

        /// <summary>
        /// 自定义游戏
        /// </summary>
        //C2L_JoinSefDefineTeam,

        /// <summary>
        /// 服务器返回给客户端的notify协议，不需要特殊处理，弹出对话框即可(int code[ServerNotifyCode], byte[] data)
        /// </summary>
        L2C_ServerNotify,

        ///新手引导
        C2L_Newbie,

        L2C_Newbie = C2L_Newbie,

        /// <summary>
        /// 取消准备()
        /// </summary>
        C2L_OutReady,

        /// <summary>
        /// 取消准备(byte result)
        /// </summary>
        L2C_OutReady = C2L_OutReady,

        /// <summary>
        /// 房间返回(string teamId)
        /// </summary>
        L2S_RoomBack,

        S2C_RoomBack = L2S_RoomBack,

        /// <summary>
        /// 房间回收(string teamId)
        /// </summary>
        L2S_RoomEnd,

        /// <summary>
        /// 准备好选择英雄
        /// </summary>
        C2L_ReadySelectHero,

        L2C_ReadySelectHero = C2L_ReadySelectHero,

        /// <summary>
        /// 进入准备房间(int roomId, int myNewId,List(ReadyPlayerSampleInfo) allplayers) TODO这里不够，还得包含房间内都有谁
        /// </summary>
        L2C_RoomReady,

        /// <summary>
        /// 准备确认(bool) todo:lc
        /// </summary>
        C2L_ReadyCheckOK,

        /// <summary>
        /// 准备确认(int newuid,bool ok) todo:lc
        /// </summary>
        L2C_ReadyCheckOK = C2L_ReadyCheckOK,

        /// <summary>
        /// 准备确认完成
        /// </summary>
        L2C_ReadyCheckAllOK,

        /// <summary>
        /// 选择召唤师技能(string id)
        /// </summary>
        C2L_SelectSelfDefSkill,

        /// <summary>
        /// 选择召唤师技能(int uid,string id)
        /// </summary>
        L2C_SelectSelfDefSkill = C2L_SelectSelfDefSkill,

        /// <summary>
        /// 选择皮肤(string id)
        /// </summary>
        C2L_SelectHeroSkin,

        /// <summary>
        /// 选择皮肤(int uid,string id)
        /// </summary>
        L2C_SelectHeroSkin = C2L_SelectHeroSkin,

        /// <summary>
        /// 等选皮肤进游戏(int waitTimes)
        /// </summary>
        L2C_WaitForSkin,

        /// <summary>
        /// 选择英雄操作(string heroId)
        /// </summary>
        C2L_RoomSelectHero,

        /// <summary>
        /// 选择英雄操作(int newUid , byte[] HoreInfo)
        /// </summary>
        L2C_RoomSelectHero = C2L_RoomSelectHero,

        /// <summary>
        /// 确认选择英雄
        /// </summary>
        C2L_RoomSelectHeroOK,

        /// <summary>
        /// 确认选择英雄(int newUid,bool ok)
        /// </summary>
        L2C_RoomSelectHeroOK = C2L_RoomSelectHeroOK,

        /// <summary>
        /// 聊天
        /// </summary>
        C2L_CaptionLobby,

        L2C_CaptionLobby = C2L_CaptionLobby,

        #region rand

        C2L_RoomSelectRandomHero,
        L2C_RoomSelectRandomHero = C2L_RoomSelectRandomHero,

        G2L_RoomSelectRandomHeroId,
        L2G_RoomSelectRandomHeroId = G2L_RoomSelectRandomHeroId,

        //L2C_RoomUpdateChangeHeroInfo,

        C2L_RoomReqChangeHero,
        L2C_RoomReqChangeHero = C2L_RoomReqChangeHero,

        C2L_RoomRespChangeHero,
        L2C_RoomRespChangeHero = C2L_RoomRespChangeHero,

        C2L_RoomRandomHero,
        L2C_RoomRandomHero = C2L_RoomRandomHero,
        L2C_CanExchangeHeroList,
        L2C_ModifyState,
        C2L_RoomCancelReqChangeHero,
        L2C_RoomCancelReqChangeHero = C2L_RoomCancelReqChangeHero,

        #endregion rand

        #region 断线重连相关

        /// <summary>
        /// 玩家断线返回()
        /// </summary>
        C2L_UserBack,

        /// <summary>
        /// 玩家返回消息（byte state,byte[] info）info是InBattleAllInfo文件里的各种类型
        /// </summary>
        L2C_UserBack = C2L_UserBack,

        /// <summary>
        /// 观看好友战斗(string friendSummerId)
        /// </summary>
        C2L_LookFriendFight,

        /// <summary>
        /// 观看好友战斗，返回信息(byte friendState,byte[] InBattleLobbyInfo)
        /// 类似UserBack消息,只有PlayerState.InFight状态才有效
        /// </summary>
        L2C_LookFriendFight = C2L_LookFriendFight,

        #endregion 断线重连相关

        /// <summary>
        /// 通知客户端消息
        /// </summary>
        L2C_TipMessage,

        C2L_GM_CMD,

        /// <summary>
        /// 通知pvp状态改变
        /// </summary>
        L2S_NoticePlayerPvpState,

        /// <summary>
        /// 通知session Lobby停服
        /// </summary>
        L2Se_LobbyShutDown,

        /// <summary>
        /// 战斗中的公告通知客户端
        /// </summary>
        L2C_BattleNotification
    }

    public enum PvpCode : byte
    {
        //==============pvp相关=============
        // TODO 需要固定code编号， 上线后修改只能新增
        C2P_NetworkStatus = 1,

        #region share part

        /// 加入队列(byte gameType,int battleId, int ServerId, string Uid)
        /// </summary>
        G2L_JoinQueue = MobaShareCode.JoinQueue,

        L2G_JoinQueue = MobaShareCode.JoinQueue,

        /// <summary>
        /// 取消排队()
        /// </summary>
        L2G_OutQueue = MobaShareCode.OutQueue,

        G2L_OutQueue = MobaShareCode.OutQueue,

        /// <summary>
        /// pvpserver通知lobby可以开启了(int roomId)
        /// </summary>
        P2L_StartGame = MobaShareCode.StartGame,

        /// <summary>
        /// 游戏开始进入pvpserver(byte[]) PvpStartGameInfo
        /// </summary>
        L2G_StartGame = MobaShareCode.StartGame,

        L2G_Newbie = LobbyCode.C2L_Newbie,

        /// <summary>
        /// 查询人物pvp状态
        /// </summary>
        L2P_QueryPvpState = MobaShareCode.QueryPvpState,

        P2L_QueryPvpState = MobaShareCode.QueryPvpState,

        /// <summary>
        /// 查询修正角色状态
        /// </summary>
        RefreshPlayerPvpState = MobaShareCode.RefreshPlayerPvpState,

        /// <summary>
        /// 强制退出pvp,测试通道
        /// </summary>
        L2P_ForceQuitPvp = MobaShareCode.ForceQuitPvp,

        P2L_ForceQuitPvp = MobaShareCode.ForceQuitPvp,
        L2G_ForceQuitPvp = MobaShareCode.ForceQuitPvp,
        G2L_ForceQuitPvp = MobaShareCode.ForceQuitPvp,

        /// <summary>
        /// lobby关服修复提示
        /// </summary>
        L2G_LobbyShutdown = MobaShareCode.LobbyShutdown,

        G2C_LobbyShutdown = MobaShareCode.LobbyShutdown,

        /// <summary>
        /// 玩家异常退出
        /// </summary>
        G2L_OnUserQuit = MobaShareCode.OnUserQuit,

        /// <summary>
        /// 玩家下线(int newUid)
        /// </summary>
        P2C_PlayerOutline = MobaShareCode.PlayerOutline,

        P2L_PlayerOutline = MobaShareCode.PlayerOutline,
        L2G_PlayerOutline = MobaShareCode.PlayerOutline,

        /// <summary>
        /// 同步重连完成(int newuid)
        /// </summary>
        P2L_Playerconnect = MobaShareCode.Playerconnect,

        L2G_Playerconnect = MobaShareCode.Playerconnect,

        #endregion share part

        PvpStartCode = MobaShareCode.ShareEnd,
        Unknow = MobaShareCode.ShareEnd,

        /// <summary>
        /// 登录(string account, string password)
        /// </summary>
        C2L_LoginLobby,

        C2L_FakeLoginLobby,      //假登录(string account,int uid,int serverId)

        /// <summary>
        /// 登录结果(byte isOk)
        /// </summary>
        L2C_LoginLobby,

        /// <summary>
        /// 队伍加入队列(int battleId)
        /// </summary>
        C2L_JoinQueueTeam,

        //G2L_JoinQueueTeam = C2L_JoinQueueTeam,
        //L2C_JoinQueueTeam = C2L_JoinQueueTeam,

        /// <summary>
        /// 自定义游戏
        /// </summary>
        C2L_JoinSefDefineTeam,

        #region closed

        /// <summary>
        /// 服务器返回给客户端的notify协议，不需要特殊处理，弹出对话框即可(int code[ServerNotifyCode], byte[] data)
        /// </summary>
        //L2C_ServerNotify = 17,

        ///新手引导
        //C2L_Newbie = 18,
        //L2C_Newbie = C2L_Newbie,
        ///

        /// <summary>
        /// 取消排队(byte result)
        /// </summary>
        //L2C_OutQueue = C2L_OutQueue,

        /// <summary>
        /// 取消准备()
        /// </summary>
        //C2L_OutReady = 22,

        /// <summary>
        /// 取消准备(byte result)
        /// </summary>
        //L2C_OutReady = C2L_OutReady,

        ///// <summary>
        ///// 房间返回(string teamId)
        ///// </summary>
        //L2G_RoomBack = 24,
        ///// <summary>
        ///// 房间返回(string teamId)
        ///// </summary>
        //G2C_RoomBack = L2G_RoomBack,

        ///// <summary>
        ///// 房间回收(string teamId)
        ///// </summary>
        //L2G_RoomEnd = 25,

        /// <summary>
        /// 进入准备房间(int roomId, int myNewId,List(ReadyPlayerSampleInfo) allplayers) TODO这里不够，还得包含房间内都有谁
        /// </summary>
        //L2C_RoomReady = 30,

        /// <summary>
        /// 准备确认(bool) todo:lc
        /// </summary>
        //C2L_ReadyCheckOK,
        /// <summary>
        /// 准备确认(int newuid,bool ok) todo:lc
        /// </summary>
        //L2C_ReadyCheckOK = C2L_ReadyCheckOK,
        /// <summary>
        /// 准备确认完成
        /// </summary>
        //L2C_ReadyCheckAllOK,

        /// <summary>
        /// 选择召唤师技能(string id)
        /// </summary>
        //C2L_SelectSelfDefSkill,
        /// <summary>
        /// 选择召唤师技能(int uid,string id)
        /// </summary>
        //L2C_SelectSelfDefSkill = C2L_SelectSelfDefSkill,

        /// <summary>
        /// 选择皮肤(string id)
        /// </summary>
        //C2L_SelectHeroSkin,
        /// <summary>
        /// 选择皮肤(int uid,string id)
        /// </summary>
        //L2C_SelectHeroSkin = C2L_SelectHeroSkin,

        /// <summary>
        /// 等选皮肤进游戏(int waitTimes)
        /// </summary>
        //L2C_WaitForSkin,

        /// <summary>
        /// 选择英雄操作(string heroId)
        /// </summary>
        //C2L_RoomSelectHero,

        /// <summary>
        /// 选择英雄操作(int newUid , byte[] HoreInfo)
        /// </summary>
        //L2C_RoomSelectHero = C2L_RoomSelectHero,

        /// <summary>
        /// 确认选择英雄
        /// </summary>
        //C2L_RoomSelectHeroOK,
        /// <summary>
        /// 确认选择英雄(int newUid,bool ok)
        /// </summary>
        //L2C_RoomSelectHeroOK = C2L_RoomSelectHeroOK,

        #endregion closed

        /// <summary>
        /// 更新信息到pvpserver(byte[] battleInfo, byte[] btTeam)
        /// </summary>
        L2P_UpateNewRoom,

        /// <summary>
        /// pve创建房间
        /// </summary>
        G2P_UpateNewPveRoom,

        P2G_UpateNewPveRoom = G2P_UpateNewPveRoom,
        G2C_UpateNewPveRoom = G2P_UpateNewPveRoom,

        /// <summary>
        /// 登录到pvpserver (int roomId, int newUid, string newKey)
        /// </summary>
        C2P_LoginFight,

        /// <summary>
        /// 登录到pvpserver (int newUid, byte isOk)
        /// </summary>
        P2C_LoginFight,

        /// <summary>
        /// 重连到pvpserver (int roomId, int newUid, string newKey)
        /// </summary>
        C2P_Reconnect,

        /// <summary>
        /// 重连结果(byte result)  result=0成功，其他失败
        /// </summary>
        P2C_Reconnect,

        /// <summary>
        /// 玩家断线返回()
        /// </summary>
        //C2L_UserBack,
        /// <summary>
        /// 玩家返回消息（byte state,byte[] info）info是InBattleAllInfo文件里的各种类型
        /// </summary>
        //L2C_UserBack = C2L_UserBack,
        /// <summary>
        /// 断线返回(int roomId, int newUid, string newKey)
        /// </summary>
        C2P_BackGame,

        /// <summary>
        /// 断线返回结果（byte result）result=0:load,result<100:fight,result=101：角色不存在，result=102：密码错误
        /// </summary>
        P2C_BackGame,

        /// <summary>
        /// 载入进度，只客户端解析，客户端自己定义规则(int newUid,byte process)
        /// </summary>
        C2P_LoadProcess,

        /// <summary>
        /// 载入进度，回传(int newUid,byte process)
        /// </summary>
        P2C_LoadProcess = C2P_LoadProcess,

        /// <summary>
        /// 载入完成 （）
        /// </summary>
        C2P_LoadingOK,

        /// <summary>
        /// 发送给lobby，载入完成？ （）
        /// </summary>
        P2L_LoadingOK,

        /// <summary>
        /// 广播（byte[] inBattleRuntimeInfo）， 全部都加载完才发这个， TODO 最好是发送进度
        /// </summary>
        P2C_LoadingOK,

        /// <summary>
        /// load返回信息(byte[] bt) InLoadingRuntimeInfo
        /// </summary>
        P2C_BackLoadingInfo,

        /// <summary>
        /// 玩家战斗结束
        /// </summary>
        L2C_PlayerFightEnd,

        /// <summary>
        /// 准备好选择英雄
        /// </summary>
        //C2L_ReadySelectHero,
        //L2C_ReadySelectHero = C2L_ReadySelectHero,

        /// <summary>
        /// 战斗相关的消息段begin
        /// </summary>
        FightMsgBegin,

        /// <summary>
        /// 战斗开始
        /// </summary>
        BattleStart,

        /// <summary>
        /// 开始刷兵
        /// </summary>
        //BattleLogicStart,

        /// <summary>
        /// 战斗结束(isLmWin,byte[]PvpTeamInfo)
        /// </summary>
        BattleEnd,

        L2P_BattleEndCheck = BattleEnd,//回传确认消息

        /// 新手引导战斗内
        C2P_NewbieInBattle,

        P2C_NewbieInBattle = C2P_NewbieInBattle,

        // 单位可见状态变化
        P2C_UnitsVisiableChanged,

        /// <summary>
        /// 战场相关消息 #特殊事件，一般要单独写函数管理
        /// </summary>
        FightInfoBegin,

        /// <summary>
        /// 创建主角(byte[])   List(int unitId, HeroInfoData data,  svector3 pos, float rotate)
        /// </summary>
        P2C_CreateHero,          //

        /// <summary>
        /// 创建单位(byte[])  UnitInfo
        /// </summary>
        P2C_CreateUnits,             //

        /// <summary>
        /// 创建地图脚本物品 P2CCreateScriptItem
        /// </summary>
        P2C_CreateScriptItem,

        /// <summary>
        /// 创建地图物品（血瓶，buff等）(byte[]) BuffItemInfo
        /// </summary>
        P2C_CreateMapItem,

        /// <summary>
        /// 获得item，并删除(int unitId,int itemUnitId)
        /// </summary>
        C2P_GetMapItem,

        P2C_GetMapItem = C2P_GetMapItem,

        /// <summary>
        /// 旋转(int unitId,float rotatoY)
        /// </summary>
        P2C_RotatoTo,

        /// <summary>
        /// 带path的MoveToPos, 单独抽出来，用开关做新的, 目前方法中没用用到， 保留
        /// </summary>
        C2P_MoveToPosWithPath,

        P2C_MoveToPosWithPath = C2P_MoveToPosWithPath,

        /// <summary>
        /// ping  (long clientTick)
        /// </summary>
        C2P_Ping,

        /// <summary>
        /// pingBack (long clientTick, long serverTick)
        /// </summary>
        P2C_Ping = C2P_Ping,

        /// <summary>
        /// 通知某人获得单位逻辑控制(int targetUnitId)
        /// </summary>
        P2C_AddPlayerControlUnit,

        /// <summary>
        /// 通知某人取消单位逻辑控制(int targetUnitId)
        /// </summary>
        P2C_RemovePlayerControlUnit,

        /// <summary>
        /// 杀死某人(int attackerId,int targetId)
        /// </summary>
        C2P_Kill,

        /// <summary>
        /// 杀死某人(int attackerId,int targetId,long relivetime)
        /// </summary>
        P2C_Kill = C2P_Kill,

        /// <summary>
        /// 玩家重生(unitId)
        /// </summary>
        P2C_ReliveHero,

        /// <summary>
        /// 激活单位(int unitId)
        /// </summary>
        P2C_ActiveUnit,

        /// <summary>
        /// 增加经验(int unitId,int addon,int newexp,int newlevel)
        /// </summary>
        P2C_AddExp,

        #region 战斗中间数据

        /// <summary>
        /// 关键数值修改
        /// </summary>
        C2P_DataChange,

        P2C_DataChange = C2P_DataChange,

        /// <summary>
        /// 跳字效果
        /// </summary>
        C2P_JumpFont,

        P2C_JumpFont = C2P_JumpFont,

        /// <summary>
        /// 添加Buff
        /// </summary>
        C2P_AddBuff,

        P2C_AddBuff = C2P_AddBuff,

        /// <summary>
        /// 执行Buff
        /// </summary>
        C2P_DoBuff,

        P2C_DoBuff = C2P_DoBuff,

        /// <summary>
        /// 移除Buff
        /// </summary>
        C2P_RemoveBuff,

        P2C_RemoveBuff = C2P_RemoveBuff,

        ///// <summary>
        ///// 增加buff(int unitId,string buffId)todo：已弃用
        ///// </summary>
        P2C_AddBuffExt,

        ///// <summary>
        ///// 移除buff(int unitId,string buffId)
        ///// </summary>
        //P2C_RemoveBuffExt,
        /// <summary>
        /// 添加高级效果
        /// </summary>
        C2P_AddHighEffect,

        P2C_AddHighEffect = C2P_AddHighEffect,

        /// <summary>
        /// 执行高级效果
        /// </summary>
        C2P_DoHighEffect,

        P2C_DoHighEffect = C2P_DoHighEffect,

        /// <summary>
        /// 移除高级效果
        /// </summary>
        C2P_RemoveHighEffect,

        P2C_RemoveHighEffect = C2P_RemoveHighEffect,

        /// <summary>
        /// 关键数值更新
        /// </summary>
        C2P_DataUpdate,

        P2C_DataUpdate = C2P_DataUpdate,

        /// <summary>
        /// 伤害消息
        /// </summary>
        C2P_Wound,

        P2C_Wound = C2P_Wound,

        #endregion 战斗中间数据

        #region 转发段

        /// <summary>
        /// 战斗直接转发的相关的消息段begin #这些消息直接转发到其他玩家
        /// </summary>
        FightTransMsgBegin,//战斗直接转发的其他相关的消息段

        //目前废弃
        FightTransMsgEnd,  //战斗直接转发的其他相关的消息段end

        FightTransMsgAllBegin,      //战斗直接会转发给所有玩家begin

        /// <summary>
        /// 地图指示性功能，通告队友（int, int, SVector3）
        /// </summary>
        C2P_NotifyTeamPos,

        P2C_NotifyTeamPos = C2P_NotifyTeamPos,

        /// <summary>
        /// 地图指示性功能，通告队友（int, int, int）
        /// </summary>
        C2P_NotifyTeamTarget,

        P2C_NotifyTeamTarget = C2P_NotifyTeamTarget,

        FightTransMsgAllEnd,        //战斗直接会转发给所有玩家end
        FightMsgEnd,       //战斗相关的消息段begin

        #endregion 转发段

        /// <summary>
        /// 查询战场信息()
        /// </summary>
        C2P_QueryInFightInfo,

        /// <summary>
        /// 返回战场信息(byte[]) InBattleRuntimeInfo or null
        /// </summary>
        P2C_QueryInFightInfo = C2P_QueryInFightInfo,

        /// <summary>
        /// 击杀英雄 (byte[] KillResult)
        /// </summary>
        P2C_KillHore,

        /// <summary>
        /// 补兵
        /// (int attackerId,int killcount)
        /// </summary>
        P2C_LastKillMonster,

        /// <summary>
        /// 队伍补兵，针对特殊野怪和小兵
        /// (byte ,int killcount)
        /// </summary>
        P2C_LastTeamKillMonster,

        /// <summary>
        /// 刷新战场信息(byte[]) InBattleRuntimeInfo
        /// </summary>
        P2C_RefreshInFightInfo,

        P2C_TipMessage,

        /// <summary>
        /// 设置场景变量
        /// </summary>
        P2C_SetSceneValue,

        /// <summary>
        /// 观察玩家log
        /// </summary>
        C2P_WatchPlayer,

        /// <summary>
        /// 其他玩家登录
        /// </summary>
        P2C_OtherLogin,

        /// <summary>
        /// 其他玩家登录(string uid)
        /// </summary>
        G2L_OtherLogin,

        /// <summary>
        /// 升级技能(string skillId)
        /// </summary>
        C2P_UpSkillLevel,

        /// <summary>
        /// 升级技能(int unitId,string skillId,int skillLevel)
        /// </summary>
        P2C_UpSkillLevel = C2P_UpSkillLevel,

        /// <summary>
        /// 增加技能点(int unitId,int add,int currVal)
        /// </summary>
        P2C_AddSkillPoint,

        /// <summary>
        /// 增加金币(int unitId,int add,int currVal,byte type)
        /// </summary>
        P2C_AddMoney,

        /// <summary>
        /// 击杀增加金币(int unitId,int add,int killUnitId)
        /// </summary>
        P2C_KillAddMoney,

        /// <summary>
        /// 原地复活(int unitId)
        /// </summary>
        P2C_Rebirth,

        /// <summary>
        /// 延迟较大的客户端发送check（ReadySkillInfo)
        /// </summary>
        C2P_ReadySkillCheck,

        P2C_ReadySkillCheck = C2P_ReadySkillCheck,

        /// <summary>
        /// 同步技能信息
        /// </summary>
        P2C_SynSkillInfo,

        /// <summary>
        /// 开始技能CD
        /// </summary>
        P2C_StartSkillCD,

        #region 道具相关

        /// <summary>
        /// 买装备(string shopId,string itemId)
        /// </summary>
        C2P_BuyItem,

        /// <summary>
        /// 买装备，只同步自己(int itemoid)
        /// </summary>
        P2C_BuyItem = C2P_BuyItem,

        /// <summary>
        /// 卖装备(string shopId,int itemoid)
        /// </summary>
        C2P_SellItem,

        /// <summary>
        /// 卖装备，只同步自己(int itemoid)
        /// </summary>
        P2C_SellItem = C2P_SellItem,

        /// <summary>
        /// 撤销操作
        /// </summary>
        C2P_RevertShop,

        /// <summary>
        /// 撤销操作(byte ok) 0成功
        /// </summary>
        P2C_RevertShop = C2P_RevertShop,

        /// <summary>
        /// 使用道具(int itemoid)
        /// </summary>
        C2P_UseItem,

        /// <summary>
        /// 使用道具(int itemoid,byte ok)0成功
        /// </summary>
        P2C_UseItem = C2P_UseItem,

        /// <summary>
        /// 加装备(int unitId,byte[] ItemDynData)
        /// </summary>
        P2C_AddItem,

        /// <summary>
        /// 移除装备(int unitId,int itemoid)
        /// </summary>
        P2C_RemoveItem,

        /// <summary>
        /// 更新装备,道具属性变动了(int unitId,byte[] ItemDynData)
        /// </summary>
        P2C_UpdateItem,

        #endregion 道具相关

        /// <summary>
        /// 回复hp,mp (int unitId,float addhp,float addmp)
        /// </summary>
        P2C_RestoreData,

        #region 调试相关

        P2C_AIDebugInfo,

        #endregion 调试相关

        #region 观众模式

        /// <summary>
        /// 观看好友战斗(string friendSummerId)
        /// </summary>
        //C2L_LookFriendFight,
        /// <summary>
        /// 观看好友战斗，返回信息(byte friendState,byte[] InBattleLobbyInfo)
        /// 类似UserBack消息,只有PlayerState.InFight状态才有效
        /// </summary>
        //L2C_LookFriendFight = C2L_LookFriendFight,
        P2C_PlayerReconnect,

        /// <summary>
        /// 登录(int roomId)
        /// </summary>
        C2P_LoginAsViewer,

        /// <summary>
        /// 聊天(string content)
        /// </summary>
        C2P_ViewerChat,

        P2C_ViewerChat = C2P_ViewerChat,

        /// <summary>
        /// 延迟的全场数据(ReplaySceneCachePacks packs)
        /// </summary>
        P2C_ViewerDelaySceneData,

        /// <summary>
        /// 同步观众信息(int count)
        /// </summary>
        P2C_ViewerUpdate,

        #endregion 观众模式

        /// <summary>
        /// ping请求,反加速使用(long serverTime)
        /// </summary>
        P2C_CheckPing,

        C2P_CheckPing = P2C_CheckPing,

        /// <summary>
        /// 协议文件MD5校验
        /// </summary>
        P2L_CheckMd5,

        G2L_CheckMd5 = P2L_CheckMd5,

        //added by shaoming
        P2C_MonsterPseudoDeath,

        //@added by shaoming

        //liuchen
        P2C_Packages,

        P2C_FrameSync,

        #region pve相关

        /// <summary>
        /// 登录pve(int battleId,byte[] btLM,byte[] btBL) List PvePlayerInfo
        /// </summary>
        C2P_LoginPve,

        /// <summary>
        /// 登录结果(int roomId,string key,PvpErrorCode retCode)
        /// </summary>
        P2C_LoginPve = C2P_LoginPve,

        /// <summary>
        /// pve重连(int roomId,string key)
        /// </summary>
        C2P_PveReconnect,

        /// <summary>
        /// pve重连(PvpErrorCode retCode)
        /// </summary>
        P2C_PveReconnect = C2P_PveReconnect,

        /// <summary>
        /// 暂停(bool isPause)
        /// </summary>
        C2P_PvePause,

        P2C_PvePause = C2P_PvePause,

        /// <summary>
        /// pve退出
        /// </summary>
        C2P_PveQuit,

        #endregion pve相关

        C2P_GMCheat,
        P2C_GMCheat = C2P_GMCheat,

        //弹幕
        C2P_Caption,

        P2C_Caption = C2P_Caption,

        //added by shaoming start
        P2C_NotifySpawnSuperSoldier,

        P2C_NotifyMonsterCreepAiStatus,
        //@added by shaoming end

        #region 投降逻辑

        C2P_StartSurrender,
        P2C_StartSurrender = C2P_StartSurrender,

        C2P_VoteSurrender,
        P2C_VoteSurrender = C2P_VoteSurrender,

        P2C_SurrenderTakeEffect,

        #endregion 投降逻辑

        G2L_CloseServer,
        P2C_DebugCollider,  // 调试碰撞体 对应ColliderInfo结构

        C2L_RoomSelectRandomHero,
        L2C_RoomSelectRandomHero = C2L_RoomSelectRandomHero,

        G2L_RoomSelectRandomHeroId,
        L2G_RoomSelectRandomHeroId = G2L_RoomSelectRandomHeroId,

        //L2C_RoomUpdateChangeHeroInfo,

        C2L_RoomReqChangeHero,
        L2C_RoomReqChangeHero = C2L_RoomReqChangeHero,

        C2L_RoomRespChangeHero,
        L2C_RoomRespChangeHero = C2L_RoomRespChangeHero,

        C2L_RoomRandomHero,
        L2C_RoomRandomHero = C2L_RoomRandomHero,
        L2C_CanExchangeHeroList,
        L2C_ModifyState,
        C2L_RoomCancelReqChangeHero,
        L2C_RoomCancelReqChangeHero = C2L_RoomCancelReqChangeHero,

        /// <summary>
        /// 更新pvpserver信息
        /// </summary>
        L2P_UpatePvpserverData,

        P2L_UpatePvpserverData = L2P_UpatePvpserverData,

        /// <summary>
        /// 设置玩家变量(byte[] C2PSetPlayerVar)
        /// </summary>
        C2P_SetPlayerVar,

        /// <summary>
        /// 查找单位信息(C2PQueryUnit)
        /// </summary>
        C2P_QueryUnit,

        /// <summary>
        /// 查找单位信息(byte[] P2CQueryUnit or null)
        /// </summary>
        P2C_QueryUnit,

        L2G_AllBattleEnd,

        /// <summary>
        /// 战斗中的公告
        /// </summary>
        P2L_BattleNotification,

        P2C_ACK = 240,                                                              //以后是保留字段
        C2P_ACK = P2C_ACK,
        P2C_DestroyChannel,

        /// <summary>
        /// 使用技能(byte[] UseSkillInfo)
        /// </summary>
        C2P_UseSkill,

        /// <summary>
        /// 技能前摇(byte[] ReadySkillCheckInfo)
        /// </summary>
        C2P_ReadySkill,

        P2C_ReadySkill = C2P_ReadySkill,

        /// <summary>
        /// 技能施法(byte[] StartSkillInfo)
        /// </summary>
        C2P_StartSkill,

        P2C_StartSkill = C2P_StartSkill,

        /// <summary>
        /// 触发技能(byte[] DoSkillInfo)
        /// 光辉的3技能
        /// </summary>
        C2P_DoSkill,

        P2C_DoSkill = C2P_DoSkill,

        /// <summary>
        /// 技能命中
        /// </summary>
        C2P_HitSkill,

        P2C_HitSkill = C2P_HitSkill,

        /// <summary>
        /// 技能打断
        /// </summary>
        C2P_StopSkill,

        P2C_StopSkill = C2P_StopSkill,

        /// <summary>
        /// 技能结束
        /// </summary>
        C2P_EndSkill,

        P2C_EndSkill = C2P_EndSkill,

        /// <summary>
        /// 瞬移
        /// </summary>
        C2P_FlashTo,

        /// <summary>
        /// 同步移动包（大量,非关键）(byte[] List(UnitSnapInfo))
        /// </summary>
        C2P_UnitsSnap,           //

        /// <summary>
        /// 同步移动包（大量,非关键）(byte[] List(UnitSnapInfo))
        /// </summary>
        P2C_UnitsSnap = C2P_UnitsSnap,//

        /// <summary>
        /// 移动到特定位置，(SVector3)
        /// </summary>
        C2P_MoveToPos,

        /// <summary>
        /// 移动到特定位置, (SVector3)... 暂时先这样
        /// </summary>
        P2C_MoveToPos = C2P_MoveToPos,

        C2P_MoveToTarget,
        P2C_MoveToTarget = C2P_MoveToTarget,

        // 停止移动
        C2P_StopMove,

        P2C_StopMove = C2P_StopMove,

        //不选人惩罚
        L2G_PunishMatch,

        G2C_PunishMatch = L2G_PunishMatch
    }

    public enum ChannelType
    {
        NormalRelivableUdp = 0,
        CustomRelivableUdp = 1,

        CurrentUdpType = NormalRelivableUdp,
    }
}