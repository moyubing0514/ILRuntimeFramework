using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadTask {

    public Action<byte[]> successCallback;
    public Action<string> errorCallback;
    public Action<float> progressCallback;
    public string url;
    public byte[] bytes;
    public string errorMessage;
}
