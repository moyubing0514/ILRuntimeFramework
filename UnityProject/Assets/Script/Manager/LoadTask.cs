using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadTask
{
    /// <summary>
    /// 资源名,无路径,无后缀 (用于在AssetBundle加载后从AB中取对应资源)
    /// </summary>
    public string resName;
    public Action<object> completeCallback;
    public string resPath;
    public AsyncOperation aysncOpertation;
    public float startTime;
    public object resource;
    public bool sync = false;//是否同步加载

    //AssetBundle文件相对路径 (含AB后缀,只有小写,用于查询依赖关系)
    public string relativePath {
    get {
            return resPath.ToLower() + AppConst.s_AB_Suffix;//AB文件名
        }
    }

    public string _fullPath;
    /// <summary>
    /// 完整路径,可以通过文件系统读取到文件
    /// </summary>
    public string fullPath
    {
        get
        {
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, "AB", relativePath)))
                _fullPath = Path.Combine(Application.streamingAssetsPath, "AB", relativePath);
            else
                _fullPath = Path.Combine(AppConst.LOCAL_DOWNLOAD_PATH, relativePath);
            return _fullPath;
        }
    }
    public LoadTask()
    {
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        //assetbundle
        if (aysncOpertation is AssetBundleCreateRequest)
        {
            AssetBundleCreateRequest req = aysncOpertation as AssetBundleCreateRequest;
            req.assetBundle.Unload(unloadAllLoadedObjects);
        }
        completeCallback = null;
        aysncOpertation = null;
        _fullPath = null;
    }
}
public class LoadSpriteTask : LoadTask
{
    public LoadSpriteTask() : base()
    {
    }

}
