using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AB依赖关系
/// </summary>
public class BundleDependInfo
{
    public string bundleName;
    public List<string> dependBundles = new List<string>();
}