
using System;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Core;
using UnityEngine;

public class FZipUtil
{
	static ICSharpCode.SharpZipLib.Zip.FastZip2 mZip2 = null;

	public static void Unzip(string zipFile,
	                  string targetDir,
	                  ICSharpCode.SharpZipLib.Core.TotalProgressHandler hTotalProgress = null,
	                  ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler hProcessDirectory = null,
	                  ICSharpCode.SharpZipLib.Core.ProcessFileHandler hProcessFile = null,
	                  ICSharpCode.SharpZipLib.Core.ProgressHandler hProgress = null,
	                  ICSharpCode.SharpZipLib.Core.CompletedFileHandler hCompletedFile = null,
	                  ICSharpCode.SharpZipLib.Core.DirectoryFailureHandler hDirectoryFailure = null,
	                  ICSharpCode.SharpZipLib.Core.FileFailureHandler hFileFailure = null)
	{
		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = hTotalProgress;
		events.ProcessDirectory = hProcessDirectory;
		events.ProcessFile = hProcessFile;
		events.Progress = hProgress;
		events.CompletedFile = hCompletedFile;
		events.DirectoryFailure = hDirectoryFailure;
		events.FileFailure = hFileFailure;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);

		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZip2.ExtractZip(zipFile, targetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, true);
	}


	public static void Unzip(Stream zipStream,
	                  string targetDir,
	                  ICSharpCode.SharpZipLib.Core.TotalProgressHandler hTotalProgress = null,
	                  ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler hProcessDirectory = null,
	                  ICSharpCode.SharpZipLib.Core.ProcessFileHandler hProcessFile = null,
	                  ICSharpCode.SharpZipLib.Core.ProgressHandler hProgress = null,
	                  ICSharpCode.SharpZipLib.Core.CompletedFileHandler hCompletedFile = null,
	                  ICSharpCode.SharpZipLib.Core.DirectoryFailureHandler hDirectoryFailure = null,
	                  ICSharpCode.SharpZipLib.Core.FileFailureHandler hFileFailure = null)
	{

		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = hTotalProgress;
		events.ProcessDirectory = hProcessDirectory;
		events.ProcessFile = hProcessFile;
		events.Progress = hProgress;
		events.CompletedFile = hCompletedFile;
		events.DirectoryFailure = hDirectoryFailure;
		events.FileFailure = hFileFailure;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);

		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZip2.ExtractZip(zipStream, targetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, true, true);
	}

	public static void Update()
	{
		if (mZip2 != null)
		{
			mZip2.ExtractUpdate();
		}
	}


	// -------- async ------- //
	static Stream mZipStream;
	static string mZipFile;
	static string mTargetDir;

	public static void ProgressHandler(object sender, ProgressEventArgs e)
	{
	//	Debug.Log("ProgressHandler: " + e.PercentComplete + "   " + e.Processed + "/" + e.Target);
	}

	public static void UnzipAsync(string fileName,
	                              string targetDir,
	                              ICSharpCode.SharpZipLib.Core.TotalProgressHandler hTotalProgress = null,
	                              ICSharpCode.SharpZipLib.Core.TotalFinishHandler hTotalFinish = null)
	{
		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = hTotalProgress;
		events.TotalFinish = hTotalFinish;
		events.Progress = ProgressHandler;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);
		
		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZipFile = fileName;
		mTargetDir = targetDir;
		
		Thread th = new Thread(StartUnzipFile);
		th.IsBackground = true;
		th.Start();
	}
	
	public static void StartUnzipFile()
	{
		mZip2.ExtractZip(mZipFile, mTargetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, true);
	}
	
	public static void UnzipAsync(Stream zipStream,
	                         string targetDir,
	                         ICSharpCode.SharpZipLib.Core.TotalProgressHandler hTotalProgress = null,
	                         ICSharpCode.SharpZipLib.Core.TotalFinishHandler hTotalFinish = null)
	{
		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = hTotalProgress;
		events.TotalFinish = hTotalFinish;
		events.Progress = ProgressHandler;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);
		
		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZipStream = zipStream;
		mTargetDir = targetDir;

		Thread th = new Thread(StartUnzipStream);
		th.IsBackground = true;
		th.Start();
	}

	public static void StartUnzipStream()
	{
		mZip2.ExtractZip(mZipStream, mTargetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, true, true);
	}
}


public delegate void HandleExtractProgress(uint taskID, uint done, uint total);
public delegate void HandleExtractFinish(uint taskID, bool successed, string errorMsg);

public class UnzipUtil
{
	public enum EState
	{
		None,
		Extracting,
		Error,
		Success, // 成功 //
		Finish, // 结束（） //
	}

    // 实例标记，默认为0，传递到回调函数中，用于区分不同的实例 //
    public uint mark = 0;

	HandleExtractProgress mTotalProgress = null;
	HandleExtractFinish mResultHandler = null;

	bool extractFromFile = false;
	Stream mZipStream;
	string mZipFile;
	string mTargetDir;
	
	volatile EState mState = EState.None;
	volatile uint mDoneSize = 0;
	volatile uint mTotalSize = 0;
	string mErrorMsg = null;

	ICSharpCode.SharpZipLib.Zip.FastZip2 mZip2 = null;
    Thread mThread = null;

	
	public UnzipUtil(string fileName,
	                  string targetDir,
	                  HandleExtractProgress hTotalProgress = null,
	                  HandleExtractFinish hTotalFinish = null)
	{
		mTotalProgress = hTotalProgress;
		mResultHandler = hTotalFinish;

		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = this.TotalProgressHandler;
		events.TotalFinish = this.FinishHandler;
		events.Progress = this.ProgressHandler;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);
		events.FileFailure = this.ErrorHandler;
		events.DirectoryFailure = this.ErrorHandler;
		
		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZipFile = fileName;
		mTargetDir = targetDir;
		extractFromFile = true;
	}

	public UnzipUtil(Stream zipStream,
	                  string targetDir,
	                  HandleExtractProgress hTotalProgress = null,
	                  HandleExtractFinish hTotalFinish = null)
	{
		mTotalProgress = hTotalProgress;
		mResultHandler = hTotalFinish;

		ICSharpCode.SharpZipLib.Zip.FastZipEvents2 events = new ICSharpCode.SharpZipLib.Zip.FastZipEvents2();
		events.TotalProgress = this.TotalProgressHandler;
		events.TotalFinish = this.FinishHandler;
		events.Progress = this.ProgressHandler;
		events.ProgressInterval = TimeSpan.FromSeconds(0.5);
		events.FileFailure = this.ErrorHandler;
		events.DirectoryFailure = this.ErrorHandler;
		
		mZip2 = new ICSharpCode.SharpZipLib.Zip.FastZip2(events);
		mZipStream = zipStream;
		mTargetDir = targetDir;
		extractFromFile = false;
	}

	public void Start()
	{
        mThread = new Thread(StartUnzip);
        mThread.IsBackground = true;
        mThread.Start();
	}

	void StartUnzip()
	{
		mState = EState.Extracting;
		if (extractFromFile)
		{
			mZip2.ExtractZip(mZipFile, mTargetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, false);
		}
		else
		{
			mZip2.ExtractZip(mZipStream, mTargetDir, ICSharpCode.SharpZipLib.Zip.FastZip2.Overwrite.Always, null, null, null, false, true);
		}
	}
	
	// error handler //
	void ErrorHandler(object sender, ScanFailureEventArgs e)
	{
		e.ContinueRunning = false;
		mErrorMsg = "File: " + e.Name + "\nexception: " + e.Exception.Message;
		mState = EState.Error;
	}

	// call in unzip trhead //
	void ProgressHandler(object sender, ProgressEventArgs e)
	{
	}

	// call in unzip trhead //
	void TotalProgressHandler(object sender, long done, long total)
	{
		mDoneSize = (uint)done;
		mTotalSize = (uint)total;
	}

	// call in unzip trhead //
	void FinishHandler(object sender, bool successed)
	{
		if (mState != EState.Error)
		{
			mState = successed ? EState.Success : EState.Error;
		}
	}

	// call in main trhead //
	public void Update()
	{
		if (mState != EState.Finish && mState != EState.None)
		{
			if (mState == EState.Extracting)
			{
				if (mTotalProgress != null)
				{
                    mTotalProgress(mark, mDoneSize, mTotalSize);
				}
			}
			else if (mState == EState.Success)
			{
				if (mResultHandler != null)
				{
                    mResultHandler(mark, true, mErrorMsg);
				}
				mState = EState.Finish;
			}
			else if (mState == EState.Error)
			{
				if (mResultHandler != null)
				{
                    mResultHandler(mark, false, mErrorMsg);
				}
				mState = EState.Finish;
			}
		}
    }

    public void OnDestroy()
    {
        EndThread();
    }

    void EndThread()
    {
        if (mThread != null && mThread.ThreadState == ThreadState.Running)
        {
            mThread.Interrupt();
        }
    }
}

