using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public static class comm
{
    public static Dictionary<byte, object> BuildParams(object[] args)
    {
        if (args == null || args.Length <= 0)
        {
            return null;
        }
        Dictionary<byte, object> param = new Dictionary<byte, object>();
        for (byte i = 0; i < args.Length; i++)
        {
            param[i] = args[i];
        }
        return param;
    }

    public static long TickToSeconds(long tick)
    {
        return tick / 10000000;
    }

    public static long SecondsToTick(long s)
    {
        return s * 10000000;
    }

    public static long SafeSub(long left, long right)
    {
        if (left >= right)
            return left - right;
        return 0;
    }

    /// <summary>
    /// TODO: 自杀函数，谨慎使用！！！
    /// 实现当一个用程序检测到关停服务指令时，根据自身的进程ID来Kill掉自身，但是前提是调用者在明确已经完全Dump有效数据情况下
    /// 否则很有可能出现数据丢失错误
    /// </summary>
    public static void KillSelfByProcesse()
    {
        var selfProcessId = Process.GetCurrentProcess().Id;
        Process[] process = Process.GetProcesses();
        foreach (Process prc in process.Where(prc => prc.Id == selfProcessId))
        {
            prc.Kill();
        }
    }

    public static string GetTableNum(string uid)
    {
        if (string.IsNullOrEmpty(uid))
        {
        }
        if (uid.Length > 12)
            return (Math.Abs(uid.GetHashCode()) % 8).ToString();
        return (Convert.ToInt64(uid) % 8).ToString();
    }

    public static bool IsRobotByUid(string uid)
    {
        long iid = 0;
        if (long.TryParse(uid, out iid))
        {
            return iid <= 10;
        }
        return false;
    }

    public static void DeepCopy<T>(T src, T target)
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public |
                                       BindingFlags.DeclaredOnly | BindingFlags.GetProperty);

        foreach (var item in fields)
        {
            var v = item.GetValue(src);
            item.SetValue(target, v);
        }

        foreach (var item in props)
        {
            var v = item.GetValue(src);
            item.SetValue(target, v);
        }
    }

    public static void CopyPropertyFromHash(Hashtable hash, object target, bool ignoreEmpty)
    {
        var type = target.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public |
                                       BindingFlags.DeclaredOnly | BindingFlags.GetProperty);

        foreach (var item in fields)
        {
            if (hash.ContainsKey(item.Name))
            {
                var v = hash[item.Name];
                if (ignoreEmpty && v == null)
                {
                    continue;
                }
                item.SetValue(target, v);
            }
        }

        foreach (var item in props)
        {
            if (hash.ContainsKey(item.Name))
            {
                var v = hash[item.Name];
                if (ignoreEmpty && v == null)
                {
                    continue;
                }
                item.SetValue(target, v);
            }
        }
    }

    public static long GetUserIdBySummId(long summonerid, int AreaId)
    {
        if (AreaId == 0)
            return AreaId * 5000000 + summonerid + 100000;
        else
            return AreaId * 5000000 + summonerid;
    }

    public static long GetSummIdByUserid(long userid, int AreaId)
    {
        long tmpid = AreaId * 5000000;
        if (AreaId == 0)
        {
            return comm.SafeSub(comm.SafeSub(userid, tmpid), 100000);
        }
        else
        {
            return comm.SafeSub(userid, tmpid);
        }
    }

    // 判断一个userid是否是这个AreaId区有效的id
    public static bool IsValidUserid(long userid, int AreaId)
    {
        if (AreaId == 0)
        {
            return userid >= (AreaId * 5000000 + 1 + 100000) && userid <= (AreaId + 1) * 5000000;
        }
        else
            return userid >= (AreaId * 5000000 + 1) && userid <= (AreaId + 1) * 5000000;
    }
}