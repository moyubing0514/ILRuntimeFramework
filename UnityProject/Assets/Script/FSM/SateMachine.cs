using UnityEngine;
using System.Collections;

public class StateMachine<T> {
    private T m_pOwner;

    private State<T> m_pCurrentState;//当前状态
    private State<T> m_pPreviousState;//上一个状态
    private State<T> m_pGlobalState;//全局状态

    /*状态机构造函数*/
    public StateMachine(T owner) {
        m_pOwner = owner;
        m_pCurrentState = null;
        m_pPreviousState = null;
        m_pGlobalState = null;
    }

    /*进入全局状态*/
    public void GlobalStateEnter() {
        m_pGlobalState.Enter(m_pOwner);
    }

    /*设置全局状态*/
    public void SetGlobalStateState(State<T> pGlobalState) {
        m_pGlobalState = pGlobalState;
        m_pGlobalState.Target = m_pOwner;
        m_pGlobalState.Enter(m_pOwner);
    }

    /*设置当前状态*/
    public void SetCurrentState(State<T> pCurrentState) {
        m_pCurrentState = pCurrentState;
        m_pCurrentState.Target = m_pOwner;
        m_pCurrentState.Enter(m_pOwner);
    }

    /*Update*/
    public void SMUpdate() {

        if (m_pGlobalState != null)
            m_pGlobalState.Execute(m_pOwner);

        if (m_pCurrentState != null)
            m_pCurrentState.Execute(m_pOwner);
    }

    /*状态改变*/
    public void ChangeState(State<T> pNewState) {
        if (pNewState == null) {
            Debug.LogError("can't find this state");
        }

        //触发退出状态调用Exit方法
        m_pCurrentState.Exit(m_pOwner);
        //保存上一个状态 
        m_pPreviousState = m_pCurrentState;
        //设置新状态为当前状态
        m_pCurrentState = pNewState;
        m_pCurrentState.Target = m_pOwner;
        //进入当前状态调用Enter方法
        m_pCurrentState.Enter(m_pOwner);
    }

    public void RevertToPreviousState() {
        //切换到前一个状态
        ChangeState(m_pPreviousState);

    }

    public State<T> CurrentState() {
        //返回当前状态
        return m_pCurrentState;
    }
    public State<T> GlobalState() {
        //返回全局状态
        return m_pGlobalState;
    }
    public State<T> PreviousState() {
        //返回前一个状态
        return m_pPreviousState;
    }

}