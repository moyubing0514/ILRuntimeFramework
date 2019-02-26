using System;
using System.Text;
/// <summary>
/// 异或加密工具
/// </summary>
public class XOREncrypter
{
    static string key = "TAMANYA_COB";

    public static byte[] Encrypt(byte[] source)
    {
        byte[] dest = new byte[source.Length];
        for (int i = 0; i < source.Length; ++i)
        {
            dest[i] = (byte)(source[i] ^ (byte)key[i % key.Length]);
        }
        return dest;
    }

    public static byte[] Decrypt(byte[] source)
    {
        // 异或加密算法，再执行一次异或即可 //
        return Encrypt(source);
    }

    public static byte[] EncryptToBytes(string source)
    {
        return Encrypt(Encoding.UTF8.GetBytes(source));
    }

    public static string DecryptToString(byte[] source)
    {
        return Encoding.UTF8.GetString(Decrypt(source));
    }
}
