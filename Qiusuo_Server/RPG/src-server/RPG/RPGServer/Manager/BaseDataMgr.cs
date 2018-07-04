using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ExitGames.Logging;
using RPGServer.Tools;

namespace RPGServer.Manager
{
    /// <summary>
    /// 加载Bindata类:原始数据管理者,那么多引用,可以做成单列?
    /// </summary>
    public class BaseDataMgr
    {
        #region 字段和属性

        /// <summary>
        /// 自己的实例
        /// </summary>
        public static BaseDataMgr Instance = new BaseDataMgr();

        /// <summary>
        /// 是否已经初始化基础数据
        /// </summary>
        public bool IsInitBaseData { get; set; }

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 临时存储变量，避免多次创建对象，带来内存消耗
        /// </summary>
        private Dictionary<string, object> _tempDict;

        /// <summary>
        /// 数据字典,字典套字典:
        /// Key ==> (同一个类型的配置表的转化类名为键如:SysSkillMainVo)
        /// Value ==>( 嵌套字典=( Key ==> 配置表的具体一条数据Id字段如:Attack_Huonv_01,Value ==> 配置表这一条数据)
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _dataDict;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lockObj = new object();

        #endregion 字段和属性

        #region 构造函数 解析数据,反序列化 初始化基础数据

        /// <summary>
        /// 构造函数先执行
        /// </summary>
        public BaseDataMgr()
        {
            _tempDict = new Dictionary<string, object>();
        }

        /// <summary>
        /// 初始化基础数据
        /// </summary>
        /// <returns></returns>
        public bool InitBaseConfigData()
        {
            var xmlPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "bindata.xml";
            Log.Info("===>PVP Load bindata:" + xmlPath);
            byte[] fs = File.ReadAllBytes(xmlPath);
            {
                Log.Info("===>基础数据长度:" + (fs.Length / 1024) + "KB");
                string errMsg = "";
                object dataVo = SerializerUtils.binaryDerialize(fs, ref errMsg);
                if (dataVo == null)
                {
                    Log.Error("DataVo null 反序列化失败!: " + errMsg + "\n path:" + xmlPath);
                    return false;
                }
                Init(dataVo);
                ParseCfgTable();
                Instance.IsInitBaseData = true;
                Log.Info("===>基础数据初始化【OK】");
                return true;
            }
        }

        /// <summary>
        /// 把反序列化的数据转化为字典
        /// </summary>
        /// <param name="data">数据</param>
        private void Init(object data)
        {
            _dataDict = (Dictionary<string, Dictionary<string, object>>)data;
        }

        /// <summary>
        /// 解析配置数据表
        /// </summary>
        private void ParseCfgTable()
        {
            //            CharacterDataMgr.Instance.Init();//解析影响部分数据最高等级,升级所需经验
            //            ItemDataMgr.Instance.ParseTables();//解析商店商品信息
            //            DamageDataManager.Instance.ParseTables(); //解析伤害表
            //            PerformDataManager.Instance.ParseTables(); //解析表现包表
            //            HighEffectDataManager.Instance.ParseTables(); //解析高级效果表
            //            BuffDataManager.Instance.ParseTables(); //解析Buff表
            //            SkillDataManager.Instance.ParseTables(); //解析技能表
            //            SkillUnitDataMgr.Instance.ParseTables(); //解析技能单位表
            //            UtilDataSceneMgr.Instance.Init();//经验分配
            //            SpawnPosTools.Instance.InitData();//出生点
            //            SkillDataPool.Instance.InitPool();//技能等级数据缓存
            //            LanguageMgr.Init();
            //            InitDataName<SysSkillMainVo>();
            //            InitDataName<SysSkillHigheffVo>();
            //            InitDataName<SysSkillBuffVo>();
            //            InitDataName<SysSkillPerformVo>();
            //            BaseDataParseMgr.Instance.Parse();
            //            AStarDataPool.Init();
        }

        /// <summary>
        /// 初始化配置数据
        /// TODO:不知有何用
        /// </summary>
        /// <typeparam name="T">配置表转化而来的类</typeparam>
        private void InitDataName<T>()
        {
            Dictionary<string, object> dataMap;
            if (_dataDict.TryGetValue(typeof(T).Name, out dataMap))
            {
                foreach (var key in dataMap.Keys)
                {
                    int id = Name.add(key);
                    if (id > 32700)
                    {
                        Log.Error("Name.Key must be short , but now is " + id);
                        System.Diagnostics.Debug.Assert(id > 32700);//如果条件为false则弹出一个对话框
                    }
                }
            }
        }

        #endregion 构造函数 解析数据,反序列化 初始化基础数据

        #region 对外接口

        /// <summary>
        /// 获得T类型的具体一条数据
        /// </summary>
        /// <typeparam name="T">配置表转化成的类</typeparam>
        /// <param name="id">具体一条数据的Id,如具体SkillId,buffId</param>
        /// <returns>返回这个类型这个ID的一条完整数据</returns>
        public T GetDataById<T>(string id) where T : class
        {
            lock (_lockObj)
            {
                try
                {
                    return _dataDict[typeof(T).Name].GetValue(id) as T;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
                return default(T);
            }
        }

        /// <summary>
        /// 根据数据表的类型，返回此数据表的字典
        /// </summary>
        /// <typeparam name="T">数据表类</typeparam>
        /// <returns></returns>
        public Dictionary<string, object> GetDictByType<T>() where T : class
        {
            return _dataDict.GetValue(typeof(T).Name);
        }

        /// <summary>
        ///获得某一个类型所有数据字典
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        public Dictionary<string, T> GetTypeDicByType<T>() where T : class
        {
            _tempDict = _dataDict[typeof(T).Name];
            if (_tempDict == null)
            {
                Log.Error("GetTypeDicByType error,null table:" + typeof(T).Name);
                return null;
            }
            Dictionary<string, T> typeDict = new Dictionary<string, T>();
            foreach (var item in _tempDict)
            {
                typeDict[item.Key] = item.Value as T;
            }
            return typeDict;
        }

        /// <summary>
        /// 下载网络文件（BinData）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        private static string DownloadFile(string url, string filename)
        {
            HttpWebRequest myrq = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myrp = (HttpWebResponse)myrq.GetResponse();
            Stream st = myrp.GetResponseStream();
            string localPath = filename + "bindata.xml";
            Stream so = new FileStream(filename + "\\bindata.xml", FileMode.Create);
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, @by.Length);
            while (osize > 0)
            {
                so.Write(by, 0, osize);
                osize = st.Read(by, 0, @by.Length);
            }
            so.Close();
            st.Close();
            myrp.Close();
            myrq.Abort();
            return localPath;
        }

        #endregion 对外接口
    }
}