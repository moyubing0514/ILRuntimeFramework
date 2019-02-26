using ICSharpCode.SharpZipLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MainState_CheckVersion : MainState
{
    private static MainState_CheckVersion m_instance;
    public new static MainState_CheckVersion Instance {
        get{
            if (m_instance == null)
                m_instance = new MainState_CheckVersion();
            return m_instance;
        }
    }

    private TotalProgressHandler m_TotalProgressHandler;
    private TotalFinishHandler m_TotalFinishHandler;

    public override void Enter(GameMain pEntity) {
        base.Enter(pEntity);
        pEntity.ShowCheckVersionUI();

        //下载本地Main配置
        string path = AppConst.GetStreemingAssetURL("Main.json");
        DownloadManager.Instance.DownloadFileAsync(path , OnMainFileDownloadSuccess);

        GameMain.DispatcherEvent(CommonEvents.UpdateCheckVersionProgress, 1f, GameMain.Instance.GetText(1));
    }

    private void OnMainFileDownloadSuccess(byte[] pBytes) {
        string json = Encoding.UTF8.GetString(pBytes);
        Debug.Log(json);
        MainJson mainJson = JsonUtility.FromJson<MainJson>(json);

        Application.targetFrameRate = AppConst.s_FrameRate = mainJson.frameRate;
        AppConst.s_IsDebug = mainJson.isDebug;
        AppConst.s_Host = mainJson.host;
        AppConst.s_AppVersion = AppVersion.Create(mainJson.version);
        //资源版本取PlayerPrefs存的
        if (PlayerPrefs.HasKey("ResVersion")) {
            AppConst.s_AppVersion.d = PlayerPrefs.GetInt("ResVersion");
        }

        foreach (MainTextJson mainTextJson in mainJson.texts) {
            GameMain.Instance.m_Texts[mainTextJson.id] = mainTextJson.text;
        }

        //下载远程Version配置
        string path = AppConst.GetRemoteURL("Version.json");
        DownloadManager.Instance.DownloadFileAsync(path, OnVersionFileDownloadSuccess, pProgressCallback: OnVersionFileDowloadProgress);

    }

    private void OnVersionFileDowloadProgress(float pProgress) {
        GameMain.DispatcherEvent(CommonEvents.UpdateCheckVersionProgress, pProgress, GameMain.Instance.GetText(1));
    }

    private void OnVersionFileDownloadSuccess(byte[] pBytes) {
        string json = Encoding.UTF8.GetString(pBytes);
        VersionJson versionJson = JsonUtility.FromJson<VersionJson>(json);
        Debug.Log(json);
        AppVersion newVersion = AppVersion.Create(versionJson.version);
#if UNITY_ANDROID
        AppConst.s_AppUrl = versionJson.aOSAppUrl;
        AppConst.s_AppUrl = versionJson.aOSResUrl;
#else
        AppConst.s_AppUrl = versionJson.iOSAppUrl;
        AppConst.s_ResUrl = versionJson.iOSResUrl;
#endif
        if (AppVersion.Bigger(newVersion, AppConst.s_AppVersion)) {
            //需要版更
            Target.ShowAlert(2,OnAppUpdate);
        } else if (newVersion.d > AppConst.s_AppVersion.d) {
            //需要热更
            Target.ShowAlert(3, OnResUpdate);
        } else {
            Target.GetFSM().ChangeState(MainState_LoadDll.Instance);
        }
    }

    private void OnAppUpdate() {
        Application.OpenURL(AppConst.s_AppUrl);
    }

    private void OnResUpdate() {
        DownloadManager.Instance.DownloadFileAsync(AppConst.s_ResUrl, OnResDownloadSuccess, pProgressCallback: OnResDowloadProgress);
    }

    private void OnResDowloadProgress(float pProgress) {
        GameMain.DispatcherEvent(CommonEvents.UpdateCheckVersionProgress, pProgress, GameMain.Instance.GetText(4));
    }

    private void OnResDownloadSuccess(byte[] pBytes) {
        GameMain.DispatcherEvent(CommonEvents.UpdateCheckVersionProgress, 0f, GameMain.Instance.GetText(5));
        string zipPath = AppConst.GetTmpPath("tmp.zip");
        File.WriteAllBytes(zipPath, pBytes);
        m_TotalProgressHandler += OnTotalProgressHandler;
        m_TotalFinishHandler += OnTotalFinishHandler;
        FZipUtil.UnzipAsync(zipPath, AppConst.LOCAL_DOWNLOAD_PATH, m_TotalProgressHandler, m_TotalFinishHandler);
    }

    private void OnTotalFinishHandler(object sender, bool successed) {
        m_TotalProgressHandler -= OnTotalProgressHandler;
        m_TotalFinishHandler -= OnTotalFinishHandler;
        Target.GetFSM().ChangeState(MainState_LoadDll.Instance);
    }

    private void OnTotalProgressHandler(object sender, long done, long total) {
        GameMain.DispatcherEvent(CommonEvents.UpdateCheckVersionProgress, (float)(done / total), GameMain.Instance.GetText(5));
    }


    public override void Execute(GameMain pEntity) {
        base.Execute(pEntity);
    }

    public override void Exit(GameMain pEntity) {
        base.Exit(pEntity);
    }
}
