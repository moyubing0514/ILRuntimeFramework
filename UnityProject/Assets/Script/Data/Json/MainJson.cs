using System;

[Serializable]
public class MainJson 
{
    public bool isDebug;
    public int frameRate;
    public string host;
    public string version;
    public MainTextJson[] texts;
}
[Serializable]
public class MainTextJson {
    public int id;
    public string text;
}