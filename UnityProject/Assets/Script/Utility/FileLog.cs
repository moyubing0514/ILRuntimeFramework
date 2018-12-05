using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 输出文件LOG的管理类
/// Author:Moyubing
/// </summary>
public class FileLog
{
#if ENCRYPT_LOG
	static System.IO.BinaryWriter logfile = null;
#else
    static System.IO.StreamWriter logfile = null;
#endif


    const string To_String_Param = "F4";
    // 注意：不能在初始化线程中调用 //
    static public void CheckInit()
    {
        if (logfile != null)
            return;

        try
        {
            // 创建文件 //
            string filePath = AppConst.LOCAL_LOG_PATH;

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // 删除旧日志 //
            DirectoryInfo info = new DirectoryInfo(filePath);
            FileInfo[] files = info.GetFiles();
            for (int i = 0; i < files.Length; ++i)
            {
                long deltaTicks = DateTime.Now.Ticks - files[i].LastWriteTime.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(deltaTicks);
                if (elapsedSpan.Days >= 2)
                {
                    files[i].Delete();
                }
            }

            string fileName = filePath + "/Log_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";
#if ENCRYPT_LOG
		logfile = new System.IO.BinaryWriter(File.Open(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write));
#else
            logfile = new System.IO.StreamWriter(fileName);
#endif

        }
        catch (Exception e)
        {
            Debug.LogError("LOG日志文件初始化错误,请检查是否有文件读取权限:" + e);
        }

        // 监听系统LogError //
        Application.logMessageReceived += HandleSysLog;
    }

    static public void Log(string str)
    {
#if UNITY_EDITOR || CONSOLE_LOG
        UnityEngine.Debug.Log("[" + Time.time.ToString(To_String_Param) + "]:" + str);
#endif
        CheckInit();
        WriteToFile("[L " + Time.realtimeSinceStartup.ToString(To_String_Param) + "]" + str + "\r\n");
    }

    static public void LogWarning(string str)
    {
#if UNITY_EDITOR || CONSOLE_LOG
        UnityEngine.Debug.LogWarning("[" + Time.time.ToString(To_String_Param) + "]:" + str);
#endif
        CheckInit();
        WriteToFile("[W " + Time.realtimeSinceStartup.ToString(To_String_Param) + "]" + str + "\r\n");
    }


    static public void LogError(string str)
    {
#if UNITY_EDITOR || CONSOLE_LOG
        UnityEngine.Debug.LogError("[" + Time.time.ToString(To_String_Param) + "]:" + str + "\n" + (new System.Diagnostics.StackTrace(true)).ToString());
#endif
        CheckInit();

        string tempStr = "[E " + Time.realtimeSinceStartup.ToString(To_String_Param) + "]" + str + "\r\n";
        WriteToFile(tempStr);
    }

    static public void LogObject(object obj)
    {
      ;

#if UNITY_EDITOR || CONSOLE_LOG
        UnityEngine.Debug.Log("[" + Time.time.ToString(To_String_Param) + "]: " + LitJson.JsonMapper.ToJson(obj));
#endif
        CheckInit();
        WriteToFile("[L " + Time.realtimeSinceStartup.ToString(To_String_Param) + "]: " + LitJson.JsonMapper.ToJson(obj));
    }

    static public void Important(string str)
    {
        CheckInit();

        string tempStr = "[I " + Time.realtimeSinceStartup.ToString(To_String_Param) + "]" + str + "\r\n";
        WriteToFile(tempStr);
    }

#pragma warning disable 0162
    // 额外记录系统的LogError Exception信息 //
    static void HandleSysLog(string logString, string stackTrace, LogType type)
    {
#if UNITY_EDITOR
        return;
#endif
        CheckInit();
        string tempStr;
        if (type != LogType.Log && type != LogType.Warning)
        {
            tempStr = "[Sys " + type + "]: " + logString + "\r\n" + stackTrace + "\r\n";
            WriteToFile(tempStr);
        }
        else
        {
            tempStr = "[Sys " + type + "]: " + logString + "\r\n";
            WriteToFile(tempStr);
        }
    }

    static void WriteToFile(string str)
    {
        if (logfile != null)
        {
#if ENCRYPT_LOG
			byte[] encrypted = XOREncrypter.EncryptToBytes(str);
			logfile.Write(encrypted.Length);
			logfile.Write(encrypted, 0, encrypted.Length);
			logfile.Flush();
#else
            logfile.Write(str);
            logfile.Flush();
#endif
        }
    }

    static public void Flush()
    {
        if (logfile != null)
        {
            logfile.Flush();
            logfile.Close();
        }
    }
}
