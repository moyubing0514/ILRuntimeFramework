using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainState_LoadDll : MainState
{
    private static MainState_LoadDll m_instance;

    public new static MainState_LoadDll Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MainState_LoadDll();
            return m_instance;
        }
    }

    public override void Enter(GameMain pEntity) {
        base.Enter(pEntity);

        string path = AppConst.LOCAL_DOWNLOAD_PATH + "HotFix.bin";
        if (!File.Exists(path))
            path = AppConst.GetStreemingAssetURL("HotFix.bin");
        DownloadManager.Instance.DownloadFileAsync(path,OnSuccess);

    }

    private void OnSuccess(byte[] pBytes) {
        byte[] pdb = null;
        if (AppConst.s_IsDebug) {
            pdb = File.ReadAllBytes(AppConst.GetStreemingAssetURL("HotFix.pdb"));
        }
        ILRuntimeManager.Instance.Init(pBytes, pdb);

        Target.m_HotFixLoop = ILRuntimeManager.Instance.appDomain.Instantiate<IGameHotFixInterface>("HotFix.HotFixLoop");
        Target.m_HotFixLoop.Start();
        Target.GetFSM().ChangeState(MainState_Running.Instance);
    }

    public override void Execute(GameMain pEntity) {
        base.Execute(pEntity);
    }

    public override void Exit(GameMain pEntity) {
        base.Exit(pEntity);
    }
}
