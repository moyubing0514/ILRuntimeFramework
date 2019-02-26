using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : SingleMono<DownloadManager> {

    private Dictionary<string, DownloadTask> m_pTaskCache = new Dictionary<string, DownloadTask>();
    private Queue<DownloadTask> m_pLoadingQueue = new Queue<DownloadTask>();
    private bool m_pIsLoading = false;

    void Update() {
        if (!m_pIsLoading && m_pLoadingQueue.Count > 0)
            StartCoroutine(DownloadNow(m_pLoadingQueue.Dequeue()));
    }

    public void DownloadFileAsync(string pURL, Action<byte[]> pSuccessCallback, Action<string> pErrorCallback = null, Action<float> pProgressCallback = null) {
        if (m_pTaskCache.ContainsKey(pURL))
            m_pTaskCache[pURL].successCallback(m_pTaskCache[pURL].bytes);
        else {
            DownloadTask task = new DownloadTask();
            task.url = pURL;
            task.successCallback = pSuccessCallback;
            task.errorCallback = pErrorCallback;
            task.progressCallback = pProgressCallback;
            m_pLoadingQueue.Enqueue(task);
        }
    }

    private IEnumerator DownloadNow(DownloadTask task) {
        m_pIsLoading = true;
        UnityWebRequest req = UnityWebRequest.Get(task.url);
        req.timeout = AppConst.s_Timeout;
        req.SendWebRequest();
        while (!req.isDone && !req.isHttpError && !req.isNetworkError) {
            if (task.progressCallback != null)
                task.progressCallback.Invoke(req.downloadProgress);
            yield return new WaitForEndOfFrame();
        }

        if (req.isNetworkError || req.isHttpError) {
            FileLog.LogError(req.error + " 网络异常:" + req.url);
            if (task.errorCallback != null)
                task.errorCallback.Invoke(req.error);
        } else {
            FileLog.Log("下载成功:" + req.url);
            if (task.successCallback != null)
                task.successCallback.Invoke(req.downloadHandler.data);
        }
        req.Dispose();
        m_pIsLoading = false;
    }
}
