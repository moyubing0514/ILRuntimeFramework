using UnityEngine;
using System.IO;
using System.Collections.Generic;


/// <summary>
/// �汾�Žṹ
/// Author:Moyubing
/// </summary>
public class AppVersion
{
    public byte a = 0, b = 0, c = 0;
    public int d = 0;
    //null�ַ���ת��ΪZERO
    public static AppVersion ZERO = new AppVersion();

    public static AppVersion Create(string strVer, int dValue)
    {
        AppVersion ver = Create(strVer);
        ver.d = dValue;
        return ver;
    }

    public static AppVersion Create(string strVer)
    {
        if (string.IsNullOrEmpty(strVer))
            return ZERO;

        AppVersion ver = new AppVersion();
        string[] vals = strVer.Split('.');
        switch (vals.Length)
        {
            case 0:
                return ver;
            case 1:
                ver.a = byte.Parse(vals[0]);
                return ver;
            case 2:
                ver.a = byte.Parse(vals[0]); ver.b = byte.Parse(vals[1]);
                return ver;
            case 3:
                ver.a = byte.Parse(vals[0]); ver.b = byte.Parse(vals[1]); ver.c = byte.Parse(vals[2]);
                return ver;
            default: // >= 4 //
                ver.a = byte.Parse(vals[0]); ver.b = byte.Parse(vals[1]); ver.c = byte.Parse(vals[2]); ver.d = int.Parse(vals[3]);
                return ver;
        }
    }
    /// <summary>
    /// �ԱȰ汾�Ƿ�v1�Ƿ��v2��(���Ա�d ��Դ�汾��)
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool Bigger(AppVersion v1, AppVersion v2)
    {
        return v1.ToABC() > v2.ToABC();
    }

    /// <summary>
    /// �ԱȰ汾�Ƿ���ͬ(���Ա�d ��Դ�汾��)
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool Equals(AppVersion v1, AppVersion v2)
    {
        return v1.ToABC() == v2.ToABC();
    }

    /// <summary>
    /// Ĭ��ֻ���abc
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format = null)
    {
        return a.ToString() + "." + b + "." + c;
    }

    public uint ToABC()
    {
        uint mainNum = 0;
        mainNum += ((uint)a) << 24;
        mainNum += ((uint)b) << 16;
        mainNum += ((uint)c) << 8;
        return mainNum;
    }
}
