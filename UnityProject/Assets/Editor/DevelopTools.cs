using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevelopTools
{

    [MenuItem("Tools/反向查找引用")]
    private static void FindReferences()
    {
        //确保鼠标右键选择的是一个Prefab  
        if (Selection.gameObjects.Length != 1)
        {
            Debug.Log("请选择一个GameObject");
            return;
        }
        Scene scene = EditorSceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            MonoBehaviour[] monos = root.transform.GetComponentsInChildren<MonoBehaviour>();
            foreach (var mono in monos)
            {
                if (mono == null)
                {
                    continue;
                }
                FieldInfo[] fields = mono.GetType().GetFields();
                foreach (var field in fields)
                {
                    if (field.GetValue(mono) == null)
                    {
                        // Debug.Log(mono.GetType() + "." + field.GetType() + "== null");
                        continue;
                    }
                    object obj = field.GetValue(mono);

                    if (obj.GetType() == typeof(GameObject))
                    {
                        if ((GameObject)obj == Selection.gameObjects[0])
                        {
                            Selection.activeObject = mono.gameObject;
                            Debug.Log("是个GameObject在脚本:" + mono.GetType() + " 属性:" + field.Name);
                            return;
                        }
                    }
                    else if (obj.GetType() == typeof(Sprite))
                    {
                        if (Selection.gameObjects[0].GetComponent<Image>() != null && (Sprite)obj == Selection.gameObjects[0].GetComponent<Image>().sprite)
                        {
                            Selection.activeObject = mono.gameObject;
                            Debug.Log("是个Sprite在脚本:" + mono.GetType() + " 属性:" + field.Name);
                            return;
                        }
                    }
                    else if (obj.GetType().ToString().EndsWith("[]"))
                    {
                        object[] objArr = obj as object[];
                        for (int i = 0; i < objArr.Length; i++)
                        {
                            if (objArr[i] == null)
                            {
                                Debug.Log("空数组 在脚本:" + mono.GetType() + " 数组:" + field.Name + " 元素index:" + i);
                                continue;
                            }
                            if (objArr[i].GetType() == typeof(GameObject))
                            {
                                if ((GameObject)objArr[i] == Selection.gameObjects[0])
                                {
                                    Selection.activeObject = mono.gameObject;
                                    Debug.Log("是个数组 在脚本:" + mono.GetType() + " 数组:" + field.Name + " 元素index:" + i);
                                }
                            }
                            else if (objArr[i].GetType() == typeof(GameObject))
                            {
                                if ((GameObject)objArr[i] == Selection.gameObjects[0])
                                {
                                    Selection.activeObject = mono.gameObject;
                                    Debug.Log("是个数组 在脚本:" + mono.GetType() + " 数组:" + field.Name + " 元素index:" + i);
                                }
                            }
                            else if (objArr[i].GetType() == typeof(Sprite))
                            {
                                if (Selection.gameObjects[0].GetComponent<Image>() != null && (Sprite)objArr[i] == Selection.gameObjects[0].GetComponent<Image>().sprite)
                                {
                                    Selection.activeObject = mono.gameObject;
                                    Debug.Log("是个数组 在脚本:" + mono.GetType() + " 数组:" + field.Name + " 元素index:" + i);
                                }
                            }
                            else
                            {
                                MonoBehaviour script = objArr[i] as MonoBehaviour;
                                if (script != null && script.gameObject == Selection.gameObjects[0])
                                {
                                    Selection.activeObject = mono.gameObject;
                                    Debug.Log("在脚本" + mono.GetType() + " 属性:" + field.Name);
                                    return;
                                }
                                else if (script == null)
                                {
                                    Debug.Log("------------" + field.GetValue(mono).GetType());
                                }
                            }
                        }
                    }
                    else
                    {
                        MonoBehaviour script = obj as MonoBehaviour;
                        if (script != null && script.gameObject == Selection.gameObjects[0])
                        {
                            Selection.activeObject = mono.gameObject;
                            Debug.Log("在脚本" + mono.GetType() + " 属性:" + field.Name);
                            return;
                        }
                        else if (script == null)
                        {
                            Debug.Log(field.GetValue(mono).GetType());
                        }
                    }
                }
            }

        }
    }

    [MenuItem("Tools/查找 Missing Script")]
    private static void FindMissingScript()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            Transform ret = FindMissingScriptRecursive(root.transform);
            if (ret != null)
            {
                Selection.activeTransform = ret;
                return;
            }
        }
        Debug.Log("没有missing 脚本");
    }

    private static Transform FindMissingScriptRecursive(Transform p)
    {
        MonoBehaviour[] monos = p.GetComponents<MonoBehaviour>();
        foreach (var mono in monos)
        {
            if (mono == null)
                return p;
        }
        if (p.childCount > 0)
        {
            for (var i = 0; i < p.childCount; i++)
            {
                Transform ret = FindMissingScriptRecursive(p.GetChild(i));
                if (ret != null)
                    return ret;
            }
        }
        return null;
    }

    [MenuItem("GameObject/CopyPath",priority =1)]
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    [MenuItem("Tools/PrintDepends")]
    static public void PrintDepends()
    {
        GameObject[] selections = Selection.gameObjects;
        string[] assets = new string[selections.Length];

        for (int i = 0; i < selections.Length; ++i)
        {
            assets[i] = AssetDatabase.GetAssetPath(selections[i]);
        }
        string[] depends = AssetDatabase.GetDependencies(assets);
        File.WriteAllLines("PrintDepends.txt", depends);
    }

}
