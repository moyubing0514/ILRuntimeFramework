using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainState_Running : MainState {
    private static MainState_Running m_instance;

    public new static MainState_Running Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MainState_Running();
            return m_instance;
        }
    }

    public override void Enter(GameMain pEntity) {
        base.Enter(pEntity);

        //加载游戏组件

    }

    public override void Execute(GameMain pEntity) {
        base.Execute(pEntity);
    }

    public override void Exit(GameMain pEntity) {
        base.Exit(pEntity);
    }
}
