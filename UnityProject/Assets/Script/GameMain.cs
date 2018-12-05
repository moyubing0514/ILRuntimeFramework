using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class GameMain : MonoBehaviour {
    public static GameMain instance;
    private GAME_STATUS _currentStatus;                            //主程序当前状态
    private bool _processing = false;                              //是否在处理当前状态
    private ulong _pauseTime = 0;                                  //程序切入后台的时间

    private Dictionary<string, string> _texts = new Dictionary<string, string>(); //主工程内的语言提示

    //游戏状态
    enum GAME_STATUS {
        STATUS_LOAD_GAME_CONFIG,                  //加载版本配置
        STATUS_CHECK_VERSION,                     //版本检查
        STATUS_UPDATE,                            //版本更新
        STATUS_LOAD_DB,                           //加载配置所在的数据库
        STATUS_LOAD_DLL,                          //加载DLL
        STATUS_FINISH,                            //初始化完成
    }


    void Awake() {
        instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //常驻内存
        DontDestroyOnLoad(gameObject);

        _currentStatus = GAME_STATUS.STATUS_LOAD_GAME_CONFIG;
        _processing = false;
    }

    void Start() {

    }

    void Update() {
        if (!_processing) {
            switch (_currentStatus) {
                case GAME_STATUS.STATUS_LOAD_GAME_CONFIG:
                    _processing = true;
                    StartCoroutine(LoadMainXml());
                    //LoadMainXml();
                    break;
                case GAME_STATUS.STATUS_CHECK_VERSION:
                    //TODO
                    break;
                case GAME_STATUS.STATUS_UPDATE:
                    //TODO
                    break;
                case GAME_STATUS.STATUS_LOAD_DB:
                    //TODO
                    break;
                case GAME_STATUS.STATUS_LOAD_DLL:
                    StartCoroutine(LoadHotFixAssembly());
                    break;
                case GAME_STATUS.STATUS_FINISH:
                    break;
            }

        }
    }

    private IEnumerator LoadMainXml() {
        string mainXml = "file:///" + Application.streamingAssetsPath + "/Main.xml";
        UnityWebRequest req = UnityWebRequest.Get(mainXml);
        req.timeout = 30;
        req.SendWebRequest();
        while (!req.isDone && !req.isHttpError && !req.isNetworkError) {
            yield return new WaitForEndOfFrame();
        }
        if (req.isNetworkError) {
            FileLog.LogError("网络异常" + req.url);
        } else if (req.isHttpError) {
            FileLog.LogError("请求异常" + req.url);
        } else {
            FileLog.Log("Main.Xml加载完毕 \n" + req.downloadHandler.text);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(req.downloadHandler.text);
            XmlNodeList nodeList = xml.SelectSingleNode("Root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++) {
                XmlElement elementXml = (XmlElement)nodeList[i];
                if (elementXml.Name == "Configs") {
                    XmlNodeList confChildNodes = elementXml.ChildNodes;
                    for (int j = 0; j < confChildNodes.Count; j++) {
                        XmlElement conElement = (XmlElement)confChildNodes[j];
                        if (conElement.Name == "IsDebug") {
                            AppConst.IsDebug = conElement.InnerText.ToLower().Equals("true");
                        } else if (conElement.Name == "FrameRate") {
                            int frameRate = 0;
                            int.TryParse(conElement.InnerText, out frameRate);
                            if (frameRate == 0)
                                frameRate = 30;
                            AppConst.GameFrameRate = frameRate;
                            Application.targetFrameRate = AppConst.GameFrameRate;
                        }
                    }
                } else if (elementXml.Name == "Texts") {
                    XmlNodeList textChildNodes = elementXml.ChildNodes;
                    for (int j = 0; j < textChildNodes.Count; j++) {
                        XmlElement textElement = (XmlElement)textChildNodes[j];
                        string key = textElement.GetAttribute("Key");
                        string value = textElement.InnerText;
                        _texts[key] = value;
                    }
                }
            }
        }
        req.Dispose();
        yield return new WaitForEndOfFrame();
    }


    private IEnumerator LoadHotFixAssembly() {
        byte[] dll = null;
        byte[] pdb = null;
        string path = string.Empty;
#if UNITY_EDITOR
        path = "file:///" + Application.streamingAssetsPath + "/ProjectLepus_HotFix.bin";
#else
        if (File.Exists(AppConst.LOCAL_DOWNLOAD_BIN_PATH + "/ProjectLepus_HotFix.bin"))
            path = AppConst.LOCAL_DOWNLOAD_BIN_PATH + "/ProjectLepus_HotFix.bin";
        else
            path = Application.streamingAssetsPath + "/ProjectLepus_HotFix.bin";
#endif
        UnityWebRequest req = UnityWebRequest.Get(path);
        req.timeout = 30;
        req.SendWebRequest();
        while (!req.isDone && !req.isHttpError && !req.isNetworkError) {
            yield return new WaitForEndOfFrame();
        }
        if (req.isNetworkError) {
            FileLog.LogError("网络异常" + req.url);
        } else if (req.isHttpError) {
            FileLog.LogError("请求异常" + req.url);
        } else {
            dll = req.downloadHandler.data;
        }
        req.Dispose();

#if ILRuntime_DEBUG && UNITY_EDITOR
        path = "file:///" + Application.streamingAssetsPath + "/ProjectLepus_HotFix.pdb";
        req.timeout = 30;
        req.SendWebRequest();
        while (!req.isDone && !req.isHttpError && !req.isNetworkError) {
            yield return new WaitForEndOfFrame();
        }
        if (req.isNetworkError) {
            FileLog.LogError("网络异常" + req.url);
        } else if (req.isHttpError) {
            FileLog.LogError("请求异常" + req.url);
        } else {
            pdb = req.downloadHandler.data;
        }
        req.Dispose();
#endif

        ILRuntimeManager.Instance.Init(dll, pdb);

        //变动大,可以在热更完毕后再使用的管理器放在热更项目中
        //初始化热更项目内的主管理器
        ILRuntimeManager.Instance.appDomain.Invoke(AppConst.HotFixPackageName + ".MainManager", "Setup", null, this);

        FileLog.Log("DLL加载完毕");
        _processing = false;
        _currentStatus = GAME_STATUS.STATUS_LOAD_DB;
    }
}
