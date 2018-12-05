using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 延迟执行
/// </summary>
public class SetTimeout : MonoBehaviour
{
    private static Dictionary<Action, SetTimeout> functionDict = new Dictionary<Action, SetTimeout>();

    private bool isDetroyed = false;
    private Action fun;
    public static void Start(Action function, float delay, bool isIgnoreTimeScale = true)
	{
		SetTimeout.Clear(function);
		if (function == null)
		{
			return;
		}
		SetTimeout setTimeout = (new GameObject()).AddComponent<SetTimeout>();
        setTimeout.name = "[SetTimeout]";

        setTimeout.Add(function, delay, isIgnoreTimeScale);
		SetTimeout.functionDict.Add(function, setTimeout);
	}

	public static void Clear(Action function)
	{
		if (function == null)
		{
			return;
		}
		if (SetTimeout.functionDict.ContainsKey(function))
		{
			SetTimeout setTimeout = SetTimeout.functionDict[function];
			SetTimeout.functionDict.Remove(function);
			setTimeout.fun = null;
            if (!setTimeout.isDetroyed)
                UnityEngine.Object.Destroy(setTimeout.gameObject);
		}
	}

	public void Add(Action function, float delay, bool isIgnoreTimeScale = true)
	{
		this.fun = function;
		float time = isIgnoreTimeScale ? delay : (delay * Time.timeScale);
		base.Invoke("Execute", time);
	}

	private void Execute()
	{
		if (this.fun != null)
		{
			this.fun();
		}
		SetTimeout.Clear(this.fun);
	}

    private void OnDestroy()
    {
        isDetroyed = true;
    }
}
