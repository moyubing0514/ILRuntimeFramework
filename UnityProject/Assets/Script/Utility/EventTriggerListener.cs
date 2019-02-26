using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 非按钮 点击等操作封装事件
/// </summary>
public class EventTriggerListener : EventTrigger
{
	public static EventTriggerListener Get(GameObject go)
	{
		EventTriggerListener eventTriggerListener = go.GetComponent<EventTriggerListener>();
		if (eventTriggerListener == null)
		{
			eventTriggerListener = go.AddComponent<EventTriggerListener>();
		}
		return eventTriggerListener;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
        onClick?.Invoke(base.gameObject);
    }

	public override void OnPointerDown(PointerEventData eventData)
	{
        onDown?.Invoke(base.gameObject);
        onDetailDown?.Invoke(base.gameObject, eventData);
    }

	public override void OnPointerEnter(PointerEventData eventData)
	{
        onEnter?.Invoke(base.gameObject);
    }

	public override void OnPointerExit(PointerEventData eventData)
	{
        onExit?.Invoke(base.gameObject);
    }

	public override void OnPointerUp(PointerEventData eventData)
	{
        onUp?.Invoke(base.gameObject);
        onDetailUp?.Invoke(base.gameObject, eventData);
    }

	public override void OnSelect(BaseEventData eventData)
	{
        onSelect?.Invoke(base.gameObject);
    }

	public override void OnUpdateSelected(BaseEventData eventData)
	{
        onUpdateSelect?.Invoke(base.gameObject);
    }

	public override void OnEndDrag(PointerEventData eventData)
	{
        onEndDrag?.Invoke(base.gameObject);
        onDetailEndDrag?.Invoke(base.gameObject, eventData);
    }

	public override void OnDrag(PointerEventData eventData)
	{
        onDrag?.Invoke(base.gameObject, eventData);
    }

	public override void OnBeginDrag(PointerEventData eventData)
	{
        onBeginDrag?.Invoke(base.gameObject);
        onDetailBeginDrag?.Invoke(base.gameObject, eventData);
    }

	public VoidDelegate onClick;

	public VoidDelegate onDown;

	public VoidDelegate onEnter;

	public VoidDelegate onExit;

	public VoidDelegate onUp;

	public VoidDelegate onSelect;

	public VoidDelegate onUpdateSelect;

	public VoidDelegate onEndDrag;

	public VoidDelegate onBeginDrag;

    public VoidDragDelegate onDrag;

    public VoidDragDelegate onDetailEndDrag;

    public VoidDragDelegate onDetailBeginDrag;

    public VoidDragDelegate onDetailDown;

	public VoidDragDelegate onDetailUp;

	public delegate void VoidDelegate(GameObject go);

	public delegate void VoidDragDelegate(GameObject go, PointerEventData eventData);
}
