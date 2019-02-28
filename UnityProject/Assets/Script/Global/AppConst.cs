using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AppConst {

    public static AppVersion s_AppVersion ;
    public static int s_FrameRate = 30;                                                             //游戏帧频
    public static bool s_IsDebug = false;                                                           //是否调试状态
    public static int s_Timeout = 1200;                                                             //下载timeout
    public static string s_Host = "http://sinacloud.net/moyubing-mytest/";                          //远程地址
    public static string s_AppUrl;
    public static string s_ResUrl;
    public static string s_AB_Suffix = ".rs";
    public static bool s_IsUseAB = true;

    //***************
    //****本地路径***
    //***************
    public static string LOCAL_LOG_PATH = Application.persistentDataPath + "/Logs";                   //日志目录
    public static string LOCAL_DOWNLOAD_PATH = Application.persistentDataPath + "/Download";          //下载文件总目录

    private static string _LOCAL_DOWNLOAD_TEMP_PATH = string.Empty;                                   //下载时临时文件目录


    public static string GetTmpPath(string pFilename) {
        if (string.IsNullOrEmpty(_LOCAL_DOWNLOAD_TEMP_PATH)) {
#if UNITY_EDITOR
            _LOCAL_DOWNLOAD_TEMP_PATH = Application.persistentDataPath + "/DownloadTmp/";
#else
			_LOCAL_DOWNLOAD_TEMP_PATH = Application.temporaryCachePath + "/DownloadTmp/";
#endif
        }
        if (!Directory.Exists(_LOCAL_DOWNLOAD_TEMP_PATH))
            Directory.CreateDirectory(_LOCAL_DOWNLOAD_TEMP_PATH);
        return _LOCAL_DOWNLOAD_TEMP_PATH + pFilename;
    }

    public static string GetStreemingAssetURL(string pFilename) {
#if UNITY_ANDROID
        return  Application.streamingAssetsPath + "/" + pFilename;
#else
        return "file:///" + Application.streamingAssetsPath + "/" + pFilename;
#endif
    }

    public static string GetRemoteURL(string pFileName) {
        return s_Host + pFileName;
    }


}
                          