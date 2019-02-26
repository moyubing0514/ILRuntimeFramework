using System;

public class SingletonInstance<T> where T : new()
{
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Activator.CreateInstance<T>();
			}
			return _instance;
		}
	}

	private static T _instance;
}
