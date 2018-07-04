//#define ThreadLocalNameMgr     //线程独立

using System;
using System.Collections.Generic;
using System.Threading;

public class Name   //临时作为stringID转int使用，为避免server多线程锁操作，只在初始化进行add操作，后续只做get操作
{
#if ThreadLocalNameMgr
    [ThreadStatic]
    static List<string> StringList;
    [ThreadStatic]
    static Dictionary<string, int> StringIDs;
#else
    private static List<string> StringList = new List<string>();
    private static Dictionary<string, int> StringIDs = new Dictionary<string, int>();
#endif

    public static void reset()
    {
        if (StringList != null)
        {
            StringList.Clear();
        }
        else
        {
            StringList = new List<string>();
        }

        if (StringIDs != null)
        {
            StringIDs.Clear();
        }
        else
        {
            StringIDs = new Dictionary<string, int>();
        }
    }

    public static int add(string inStr) //只在初始化添加，之后只做读取，若要同时读写，需要加锁
    {
#if ThreadLocalNameMgr
        if (StringList == null)
        {
            StringList = new List<string>();
            StringIDs = new Dictionary<string, int>();
        }
#endif
        int id = 0;
        if (StringIDs.TryGetValue(inStr, out id))
        {
            return id;
        }
        else
        {
            id = StringList.Count;
            StringIDs.Add(inStr, id);
            StringList.Add(inStr);
            return id;
        }
    }

    public static int getId(string inStr)
    {
#if ThreadLocalNameMgr
        if (StringList == null)
        {
            StringList = new List<string>();
            StringIDs = new Dictionary<string, int>();
        }
#endif
        int id = 0;
        StringIDs.TryGetValue(inStr, out id);
        return id;
    }

    public static string getString(int inId)
    {
#if ThreadLocalNameMgr
        if (StringList == null)
        {
            StringList = new List<string>();
            StringIDs = new Dictionary<string, int>();
        }
#endif
        if (inId < StringList.Count && inId >= 0)
        {
            return StringList[inId];
        }
        return "";
    }
}