using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionUtils 
{
    public static void SetActivityExt(this GameObject go,bool value) {
        if (go.activeSelf != value)
            go.SetActive(value);
    }
}
