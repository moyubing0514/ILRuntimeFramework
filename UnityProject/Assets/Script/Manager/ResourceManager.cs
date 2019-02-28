using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : SingletonInstance<ResourceManager>
{
    // 资源依赖表 //
    Dictionary<string, BundleDependInfo> bundleDepends = new Dictionary<string, BundleDependInfo>();
    // 已加载任务表 //
    Dictionary<string, LoadTask> loadedResource = new Dictionary<string, LoadTask>();

    Queue<LoadTask> queue = new Queue<LoadTask>();
    /// <summary>
    /// 正在加载的请求
    /// </summary>
    LoadTask loadingTask = null;

    AssetBundleManifest _manifest;
    AssetBundleManifest manifest
    {
        get
        {
            if (_manifest == null)
                LoadAssetBundleManifest();
            return _manifest;
        }
    }
    public ResourceManager()
    {
        // 检查Bundle目录 //
        if (!Directory.Exists(AppConst.LOCAL_DOWNLOAD_PATH))
        {
            Directory.CreateDirectory(AppConst.LOCAL_DOWNLOAD_PATH);
        }
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="resName">资源名</param>
    /// <param name="OnGetComplete">资源获取后回调 参数可能为NULL 表示获取失败</param>
    /// <returns></returns>
    public string GetResource(string resName, Action<object> OnGetComplete)
    {
        LoadTask task = null;
        if (loadedResource.ContainsKey(resName))
        {
            task = loadedResource[resName];
            OnGetComplete?.Invoke(task.resource);
        }
        else
        {

            task = new LoadTask();
            task.resName = resName;
            task.resPath = resName;
            task.completeCallback = OnGetComplete;


            //获取依赖
            if (AppConst.s_IsUseAB)
            {
                string[] dependence = manifest.GetAllDependencies(task.relativePath);
                for (int i = 0; i < dependence.Length; ++i)
                {
                    if (!loadedResource.ContainsKey(dependence[i]))
                    {
                        LoadTask dependenciesTask = new LoadTask();
                        dependenciesTask.resName = null;
                        dependenciesTask.resPath = dependence[i];
                        dependenciesTask.completeCallback = null;
                        queue.Enqueue(dependenciesTask);
                    }
                }
            }

            queue.Enqueue(task);
            StartLoadTask();
        }
        return task.resPath;
    }

    private void StartLoadTask()
    {
        if (loadingTask == null && queue.Count > 0)
        {
            loadingTask = queue.Dequeue();
            loadingTask.startTime = Time.fixedTime;
            if (!AppConst.s_IsUseAB)
            {
                loadingTask.aysncOpertation = Resources.LoadAsync(loadingTask.resPath);
                loadingTask.aysncOpertation.completed += OnTaskComplete;

            }
            else
            {

                loadingTask.aysncOpertation = AssetBundle.LoadFromFileAsync(loadingTask.fullPath);
                loadingTask.aysncOpertation.completed += OnTaskComplete;
            }
        }
    }

    private void OnTaskComplete(AsyncOperation async)
    {
        loadingTask.aysncOpertation.completed -= OnTaskComplete;
        if (async != loadingTask.aysncOpertation)
            return;

        //AssetBundle加载
        if (async is AssetBundleCreateRequest)
        {
            AssetBundleCreateRequest request = (AssetBundleCreateRequest)async;
            if (request.assetBundle == null)
            {
                FileLog.LogError("<" + loadingTask.relativePath + "> 不存在于目录:" + loadingTask.resPath);
                loadingTask.resource = null;
            }
            else
            {
                loadingTask.resource = request.assetBundle.LoadAsset(loadingTask.resName);
                if (!loadedResource.ContainsKey(loadingTask.resPath))
                    loadedResource[loadingTask.resPath] = loadingTask;
            }
        }
        else if (async is ResourceRequest)//Resource下直接加载
        {
            // FileLog.Log("资源：" + loadingTask.resName + "加载耗时：" + (Time.fixedTime - loadingTask.startTime));
            if (async == loadingTask.aysncOpertation)
            {
                object obj = ((ResourceRequest)async).asset;
                if (obj is Texture2D)
                {
                    Texture2D tex = (Texture2D)obj;
                    Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    loadingTask.resource = sp;
                }
                else
                {
                    loadingTask.resource = obj;
                }
                if (!loadedResource.ContainsKey(loadingTask.resPath))
                    loadedResource[loadingTask.resPath] = loadingTask;
            }
        }

        loadingTask.completeCallback?.Invoke(loadingTask.resource);
        loadingTask = null;

        StartLoadTask();
    }

    // 加载Manifest
    private void LoadAssetBundleManifest() {
        string path = Path.Combine(AppConst.LOCAL_DOWNLOAD_PATH, "AB");
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "AB", "AB")))
            path = Path.Combine(Application.streamingAssetsPath, "AB", "AB");
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        if (ab == null)
        {
            FileLog.LogError("<AB.manifest> 不存在于目录:" + path);
            return;
        }
        else
        {
            _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            // 压缩包释放掉
            ab.Unload(false);
        }
    }
    //todo
    internal void Release(string resPath, bool isUnloadAB = false)
    {
        if (!AppConst.s_IsUseAB)
            return;
        if (string.IsNullOrEmpty(resPath))
            return;
        if (isUnloadAB && loadedResource.ContainsKey(resPath))
        {
            LoadTask task = loadedResource[resPath];
            task.Unload(true);
            loadedResource.Remove(resPath);
            string[] dependence = _manifest.GetAllDependencies(resPath);
            for (int i = 0; i < dependence.Length; ++i)
            {
                if (loadedResource.ContainsKey(dependence[i]))
                {
                    loadedResource[dependence[i]].Unload(false);
                    loadedResource.Remove(dependence[i]);
                }
            }
        }
    }
}
