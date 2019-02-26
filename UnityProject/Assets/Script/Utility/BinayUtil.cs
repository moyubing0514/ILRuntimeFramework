using System;
using System.Text;
using UnityEngine;

public class BinayUtil
{
	public BinayUtil(byte[] bytes)
	{
		this.bytes = bytes;
	}
    public void readBool(out bool outBool)
    {
        outBool = false;
        outBool = BitConverter.ToBoolean(this.bytes, this.position);
        this.position += 1;
    }
    public void readInt(out int outInt)
    {
        outInt = 0;
        outInt = BitConverter.ToInt32(this.bytes, this.position);
        this.position += 4;
    }

    public void readUInt(out uint outInt)
	{
		outInt = 0;
		outInt = BitConverter.ToUInt32(this.bytes, this.position);
		this.position += 4;
	}
    public void readULong(out ulong outLong)
    {
        outLong = 0;
        outLong = BitConverter.ToUInt64(this.bytes, this.position);
        this.position += 4;
    }

    public void readFloat(out float outFloat)
	{
		outFloat = 0f;
		outFloat = BitConverter.ToSingle(this.bytes, this.position);
		this.position += 4;
	}

	public void readUTFBytes(out string outStr, int strLen)
	{
		outStr = string.Empty;
		int num = 0;
		for (int i = this.position + strLen - 1; i >= this.position; i--)
		{
			if (this.bytes[i] != 0)
			{
				break;
			}
			num++;
		}
		outStr = Encoding.UTF8.GetString(this.bytes, this.position, strLen - num);
		this.position += strLen;
	}

	private int position = 0;

	private byte[] bytes;
}
