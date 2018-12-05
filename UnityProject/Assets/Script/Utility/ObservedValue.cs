using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 观察者模式的值
/// Author:Moyubing
/// </summary>
/// <typeparam name="T">最好使用值类型</typeparam>
public class ObservedValue<T>
{
    public event Action<T> OnValueChange;

    private T _Value;
    public T Value
    {
        get
        {
            return _Value;
        }
        set
        {
            if (!_Value.Equals(value))
            {
                _Value = value;
                if (null != OnValueChange)
                {
                    OnValueChange(_Value);
                }
            }
        }
    }

    public void SetSilently(T value)
    {
        _Value = value;
    }
    public ObservedValue(T value)
    {
        _Value = value;
    }

}
