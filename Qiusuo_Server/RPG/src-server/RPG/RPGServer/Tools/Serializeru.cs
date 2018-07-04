using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace RPGServer.Tools
{
    public class SerializerUtils
    {
        /// <summary>
        ///  /**基于二进制反序列化**/
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns></returns>
        public static object binaryDerialize(byte[] bytes, ref string errMsg)
        {
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new UBinder();
                object data = formatter.Deserialize(ms);//TODO:姚茂新 必须要bindata.dll支持,怎么解决
                ms.Close();
                ms.Dispose();
                return data;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
            return null;
        }

        /**Json反序列化**/

        public static object jsonDerialize(byte[] bytes)
        {
            /*Type typeObj = null;
            Dictionary<string, string> jsonTable = null;
            Dictionary<string, object> objectTable = null;

            string output = Encoding.UTF8.GetString(bytes);
            object objData = JsonConvert.DeserializeObject(output, typeof(Dictionary<string, Dictionary<string, string>>));
            Dictionary<string, Dictionary<string, string>> SkillData = (Dictionary<string, Dictionary<string, string>>)objData;
            Dictionary<String, Dictionary<String, object>> releaseData = new Dictionary<string, Dictionary<string, object>>();
            //int time = Environment.TickCount;

            foreach (string Key in SkillData.Keys)
            {
                jsonTable = SkillData[Key];
                typeObj = BaseDataMgr.Instance.getClzType(Key);
                objectTable = new Dictionary<string, object>();

                foreach (string keyId in jsonTable.Keys)
                {
                    objectTable.Add(keyId, JsonConvert.DeserializeObject(jsonTable[keyId], typeObj));
                    //Log.Info("SerializerUtils", jsonTable[keyId]);
                }
                releaseData.Add(Key, objectTable);
                Log.Info("SerializerUtils", "Key:" + Key + ",typeObj:" + typeObj.ToString());
                //Log.Info("", "\n");
            }
            //Log.Info("SerializerUtils", "-jsonDeserialize() 基础数据初始化耗时:" + (Environment.TickCount - time) + " ms");
            return releaseData;*/
            return null;
        }
    }

    public class UBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            return ass.GetType(typeName);
        }
    }
}