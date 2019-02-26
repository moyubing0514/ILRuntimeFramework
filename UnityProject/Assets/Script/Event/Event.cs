using System;
using System.Linq;

public class BaseEvent
{
    public string Type { get { return m_pType; } }
    public object EventObject { get { return m_pEventObject; } }
    public object[] EventObjects { get { return m_pEventObjects; } }
    protected string m_pType;
    protected object m_pEventObject;
    protected object[] m_pEventObjects;

    public BaseEvent(string pType, params object[] pObjects) {
        m_pType = pType;
        if (pObjects == null || pObjects.Length == 0)
            m_pEventObject = null;
        else {
            m_pEventObject = pObjects[0];
            m_pEventObjects = pObjects;
        }
    }

    public BaseEvent(string pType, object pObject) {
        m_pType = pType;
        m_pEventObject = pObject;
    }
}
