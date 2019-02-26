using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileUtils {
    /// <summary>
    /// 是否有文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="MD5"></param>
    /// <returns></returns>
    public static bool CheckHasFile(string path, string MD5)
    {
        if (File.Exists(path))
        {
            string curMD5 = SpawnMD5(path);
            if (curMD5 != null && MD5 != null && curMD5.ToLower() == MD5.ToLower())
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取MD5码
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string SpawnMD5(string path)
    {
        string result = null;
        try
        {
            FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash_byte = get_md5.ComputeHash(get_file);
            get_file.Close();

            result = System.BitConverter.ToString(hash_byte);
            result = result.Replace("-", "");
            result = result.ToLower();
        }
        catch (System.Exception ex)
        {
            FileLog.LogError(ex.Message);
        }
        return result;
    }

    public static string GetFileNameWithoutExtension(this FileInfo file)
    {
        return file.Name.Replace(file.Extension,"");
    }
}
