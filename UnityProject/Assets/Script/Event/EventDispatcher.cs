using System;
using System.Collections.Generic;


public class EventDispatcher {
    private Dictionary<string, Action<BaseEvent>> m_pEventDictionary = new Dictionary<string, Action<BaseEvent>>();

    public bool HasEventListener(string type) {
        return m_pEventDictionary.ContainsKey(type);
    }

    public void AddEventListener(string type, Action<BaseEvent> listener) {
        if (m_pEventDictionary.ContainsKey(type)) {
            Action<BaseEvent> action = m_pEventDictionary[type];
            action = (Action<BaseEvent>)Delegate.Remove(action, listener);
            action = (Action<BaseEvent>)Delegate.Combine(action, listener);
            m_pEventDictionary.Remove(type);
            m_pEventDictionary.Add(type, action);
        } else {
            m_pEventDictionary.Add(type, listener);
        }
    }

    public void RemoveAllEventListender() {
        m_pEventDictionary.Clear();
    }

    public void DispatchEvent(BaseEvent evt) {
        if (m_pEventDictionary.ContainsKey(evt.Type)) {
            Action<BaseEvent> action = m_pEventDictionary[evt.Type];
            action(evt);
        }
    }

    public void RemoveEventListener(string type, Action<BaseEvent> listener) {
        if (m_pEventDictionary.ContainsKey(type)) {
            Action<BaseEvent> action = m_pEventDictionary[type];
            action = (Action<BaseEvent>)Delegate.Remove(action, listener);
            m_pEventDictionary.Remove(type);
            if (action != null) {
                m_pEventDictionary.Add(type, action);
            }
        }
    }
}
