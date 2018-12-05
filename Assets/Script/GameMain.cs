using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static GameMain instance;
    private GAME_STATUS _currentStatus;                            //主程序当前状态
    private bool _processing = false;                              //是否在处理当前状态
    private ulong _pauseTime = 0;                                  //程序切入后台的时间

    //游戏状态
    enum GAME_STATUS {
        STATUS_CHECK_VERSION,                     //版本检查
        STATUS_UPDATE,                            //版本更新
        STATUS_LOAD_DB,                           //加载配置所在的数据库
        STATUS_LOAD_DLL,                          //加载DLL
        STATUS_FINISH,                            //初始化完成
    }


    void Awake() {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }




}
