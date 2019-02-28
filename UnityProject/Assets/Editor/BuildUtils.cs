using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Build.Reporting;

public class BuildUtils
{
    [MenuItem("Build Tools/Build Android(AS项目)")]
    // xxx/Unity.exe -quit -batchmode -executeMethod CommandBuild.BuildAndroid 
    public static void BuildAndroid()
    {
        Debug.Log("开始 Build( Android )");
#if !Unity_Android
        Debug.Log("切换到 Android平台");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
#endif
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        // build option
        BuildOptions opt = BuildOptions.None;
        string export = "../Output";
        bool isDebug = false;
        //export 参数 导出目录
        var args = System.Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-export");
        if (-1 != index)
        {
            export = args[index + 1];
        }
        Debug.Log("##### Export 路径 : " + export);

        index = Array.IndexOf(args, "-debug");
        isDebug = (-1 != index);
        Debug.Log("##### Debug 版本 : " + isDebug);

        if (isDebug)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEBUG_BUILD");
            opt |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
        }
        else
        {
        }

        //opt |= BuildOptions.Il2CPP;
        opt |= BuildOptions.AcceptExternalModificationsToPlayer;
        var scenes = GetScenes();
        DirectoryInfo dir = new DirectoryInfo(export);
        if (dir.Exists)
            dir.Delete(true);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, export, BuildTarget.Android, opt);

        // error check
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build( Android ) 成功.");
        }
        else
        {
            Debug.Log("Build( Android ) 失败!");
            Debug.LogError(report.summary.result.ToString());
        }
    }

    [MenuItem("Build Tools/Build Apk Release")]
    public static void BuildAPK_Realse()
    {
        BuildAPK(true);
    }
    [MenuItem("Build Tools/Build Apk Debug")]
    public static void BuildAPK_Debug()
    {
        BuildAPK(false);
    }

    // xxx/Unity.exe -quit -batchmode -executeMethod CommandBuild.BuildAndroid 
    public static void BuildAPK(bool release = true , string version = "0.0.1")
    {
        Debug.Log("开始 Build( Android )");
#if !Unity_Android
        Debug.Log("切换到 Android平台");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
#endif
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.keyaliasPass = "12345678";
        PlayerSettings.keystorePass = "12345678";
        PlayerSettings.bundleVersion = version;
        // build option
        BuildOptions opt = BuildOptions.None;
        System.DateTime.Now.Date.ToString();

        string export = "../" + Application.productName + "_" + PlayerSettings.bundleVersion.Replace('.', '_') + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
        //+ ".apk";
        if (release)
        {
            export += "_release.apk";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
        }
        else
        {
            export += "_debug.apk";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEBUG_BUILD");
        }

        bool isDebug = false;
        //export 参数 导出目录
        var args = System.Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-export");
        if (-1 != index)
        {
            export = args[index + 1];
        }
        Debug.Log("##### Export 路径 : " + export);

        index = Array.IndexOf(args, "-debug");
        isDebug = (-1 != index);
        Debug.Log("##### Debug 版本 : " + isDebug);


        if (isDebug)
            opt |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;

        //opt |= BuildOptions.Il2CPP;
        //PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        //opt |= BuildOptions.AcceptExternalModificationsToPlayer;
        var scenes = GetScenes();
        DirectoryInfo dir = new DirectoryInfo(export);
        if (dir.Exists)
            dir.Delete(true);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, export, BuildTarget.Android, opt);

        // error check
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build( Android ) 成功.");
        }
        else
        {
            Debug.Log("Build( Android ) 失败!");
            Debug.LogError(report.summary.result.ToString());
        }
    }
    public static string[] GetScenes()
    {
        var levels = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            if (scene.enabled)
                levels.Add(scene.path);

        return levels.ToArray();
    }

    [MenuItem("Build Tools/Build AOS 资源")]
    public static void BuildAOSAssetBunlde() {
        string export = Application.streamingAssetsPath + "/AB";
        var args = System.Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-export");
        if (-1 != index) {
            export = args[index + 1];
        }
        if (!Directory.Exists(export))
            Directory.CreateDirectory(export);
        Dictionary<string, AssetBundleBuild> dic = new Dictionary<string, AssetBundleBuild>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources");
        FileInfo[] fileInfos = dir.GetFiles("*.prefab");
        foreach (FileInfo fileInfo in fileInfos) {
            string abName = fileInfo.GetFileNameWithoutExtension().ToLower() + AppConst.s_AB_Suffix;
            string relativePath = "Assets/Resources/" + fileInfo.Name;
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = abName;
            abb.assetNames = new string[] { relativePath };
            dic[abb.assetBundleName] = abb;

            //string[] dependencies = AssetDatabase.GetDependencies("Assets/Resources/" + fileInfo.Name);
            //foreach (string path in dependencies) {
            //    AssetBundleBuild abb = new AssetBundleBuild();
            //    abb.assetBundleName = path.Substring(0, path.IndexOf('.')).Replace("Assets/Resources/", "").ToLower() + AppConst.s_AB_Suffix;
            //    abb.assetNames = new string[] { path };
            //    dic[abb.assetBundleName] = abb;
            //}
        }

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        foreach (var abb in dic.Values) {
            list.Add(abb);
        }

        BuildAssetBundleOptions opt = BuildAssetBundleOptions.DeterministicAssetBundle;
        BuildPipeline.BuildAssetBundles(export, list.ToArray(), opt, BuildTarget.Android);
        //BuildPipeline.BuildAssetBundles(export, opt, BuildTarget.Android);
    }

    [MenuItem("Build Tools/显示路径")]
    static private void ShowPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(path);
    }

}