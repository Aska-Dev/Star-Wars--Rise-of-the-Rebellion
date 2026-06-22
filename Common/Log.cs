using Godot;
using System;

public static class Log
{
    public static void Error(string callerClass, string callerFunc, string message)
    {
        GD.PrintErr(callerClass,".",callerFunc," - ",message);
    }
}
