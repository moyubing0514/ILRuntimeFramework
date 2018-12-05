using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConst {

    public static int GameFrameRate = 30;                                                             //游戏帧频
    public static bool IsDebug = false;                                                               //是否调试状态


    //***************
    //****本地路径***
    //***************
    public static string LOCAL_LOG_PATH = Application.persistentDataPath + "/Logs";                   //日志目录
    public static string LOCAL_DOWNLOAD_PATH = Application.persistentDataPath + "/Download";          //下载文件总目录
    public static string LOCAL_DOWNLOAD_IMAGE_PATH = LOCAL_DOWNLOAD_PATH + "/Image";                  //下载大图缓存目录
    public static string LOCAL_DOWNLOAD_AB_PATH = LOCAL_DOWNLOAD_PATH + "/AB";                        //AssetBundle目录
    public static string LOCAL_DOWNLOAD_BIN_PATH = LOCAL_DOWNLOAD_PATH + "/Bin";                      //配置数据目录

    private static string _LOCAL_DOWNLOAD_TEMP_PATH = string.Empty;                                   //下载时临时文件目录
    public static string LOCAL_DOWNLOAD_TEMP_PATH
    {
        get
        {
            if (string.IsNullOrEmpty(_LOCAL_DOWNLOAD_TEMP_PATH)) {
#if UNITY_EDITOR
                _LOCAL_DOWNLOAD_TEMP_PATH = Application.persistentDataPath + "/DownloadTmp";
#elif UNITY_IPHONE
			_LOCAL_DOWNLOAD_TEMP_PATH = Application.temporaryCachePath + "/DownloadTmp";
#else
			_LOCAL_DOWNLOAD_TEMP_PATH = Application.temporaryCachePath + "/DownloadTmp";
#endif
            }
            return _LOCAL_DOWNLOAD_TEMP_PATH;
        }
    }

}
