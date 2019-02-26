using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class GameMain : MonoBehaviour {
    public static GameMain Instance;

    public Dictionary<int, string> m_Texts = new Dictionary<int, string>();
    public IGameHotFixInterface m_HotFixLoop;

    private StateMachine<GameMain> m_pStateMachine;//定义一个状态机
    /**
     * UI
    */
    private CheckVersionUI m_CheckVersionUI;

    void Awake() {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        m_CheckVersionUI = transform.Find("UIRoot/UICanvas/CheckVersionUI").gameObject.AddComponent<CheckVersionUI>();
        m_CheckVersionUI.Hide();

        //常驻内存
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        m_pStateMachine = new StateMachine<GameMain>(this);
        m_pStateMachine.SetCurrentState(MainState_CheckVersion.Instance);
    }

    void Update() {
        m_pStateMachine.SMUpdate();
        m_HotFixLoop?.Update();
    }

    private void OnDestroy() {
    }

    public StateMachine<GameMain> GetFSM() {
        return m_pStateMachine;
    }

    public void ShowCheckVersionUI() {
        m_CheckVersionUI.Show();
    }

    public void HideCheckVersionUI() {
        m_CheckVersionUI.Hide();
    }

    public void ShowAlert(int id, Action pCallback) {
        m_CheckVersionUI.ShowAlert(GetText(id), pCallback);
    }

    #region 事件分发相关
    private static EventDispatcher m_pEventDispatcher = new EventDispatcher();

    public static void AddEventListener(string pEventKey, System.Action<BaseEvent> pAction) {
        if (null != m_pEventDispatcher)
            m_pEventDispatcher.AddEventListener(pEventKey, pAction);
    }
    public static void RemoveEventListener(string pEventKey, System.Action<BaseEvent> pAction) {
        if (null != m_pEventDispatcher)
            m_pEventDispatcher.RemoveEventListener(pEventKey, pAction);
    }

    public static void DispatcherEvent(string pEventKey, params object[] pEventObjs) {
        if (null != m_pEventDispatcher)
            m_pEventDispatcher.DispatchEvent(new BaseEvent(pEventKey, pEventObjs));
    }

    public static void DispatcherEvent(string pEventKey, object pEventObj) {
        if (null != m_pEventDispatcher)
            m_pEventDispatcher.DispatchEvent(new BaseEvent(pEventKey, pEventObj));
    }

    public string GetText(int key) {
        if (m_Texts.ContainsKey(key))
            return m_Texts[key];
        return string.Empty;
    }
    #endregion

}
