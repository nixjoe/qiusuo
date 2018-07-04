using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Time
{
    public static long time
    {
        get { return DateTime.Now.Ticks; }
    }

    public static long Millisecond
    {
        get { return DateTime.Now.Millisecond; }
    }

    public static float deltaTime
    {
        get { return 1f / 60; }
    }

    public static float realtimeSinceStartup
    {
        get { return time - initTime; }
    }

    private static long initTime;

    public static void InitTime()
    {
        initTime = time;
    }
}