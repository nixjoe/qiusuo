using System.Collections.Generic;

public static class DictHelper
{
    /// <summary>
    ///     在字典中根据key获取相应的值,找不到返回default(TValue)
    /// </summary>
    /// <typeparam name="TKey">key类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sourceDict">源字典</param>
    /// <param name="tKey">key</param>
    /// <returns></returns>
    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> sourceDict, TKey tKey)
    {
        if (sourceDict == null) return default(TValue);
        TValue tValue;
        return sourceDict.TryGetValue(tKey, out tValue) ? tValue : default(TValue);
    }
}