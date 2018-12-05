using System;
using UnityEngine;

public abstract class SingleMono<T> : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (SINGLE == null)
			{
				string name = "SINGLE - " + typeof(T).ToString();
				GameObject gameObject = new GameObject(name);
				object obj = gameObject.AddComponent(typeof(T));
				SINGLE = (T)((object)obj);
			}
			return SINGLE;
		}
	}
	private static T SINGLE;
}
