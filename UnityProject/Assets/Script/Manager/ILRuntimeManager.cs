using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using UnityEngine.EventSystems;

/// <summary>
/// ILRuntime的管理器,负责ILRuntime初始化和相关委托代理的注册
/// Author:Moyubing
/// </summary>
public class ILRuntimeManager : SingletonInstance<ILRuntimeManager> {

    public AppDomain appDomain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dllBytes">dll 二进制数据</param>
    /// <param name="pdbBytes">pdb调试文件二进制数据,正式环境设置为null</param>
    /// 
    public void Init(byte[] dllBytes, byte[] pdbBytes = null)
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if ILRuntime_DEBUG
        //是否启动调试
        appDomain.DebugService.StartDebugService(56000);
        appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        using (System.IO.MemoryStream fs = new MemoryStream(dllBytes)) {
            Mono.Cecil.Cil.ISymbolReaderProvider symbolReaderProvider = null;
            if (pdbBytes == null) {
                appDomain.LoadAssembly(fs, null, symbolReaderProvider);
            } else {
                using (System.IO.MemoryStream p = new MemoryStream(pdbBytes)) {
                    symbolReaderProvider = new Mono.Cecil.Pdb.PdbReaderProvider();
                    appDomain.LoadAssembly(fs, p, symbolReaderProvider);
                }
            }
        }

        //这里做一些ILRuntime的注册

        //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter（详细请看04_Inheritance教程），Demo已经直接写好，直接注册即可
        //协程注册
        appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        //各种委托参数注册
        appDomain.DelegateManager.RegisterMethodDelegate<int>();
        appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
        appDomain.DelegateManager.RegisterMethodDelegate<GameObject, PointerEventData>();
        appDomain.DelegateManager.RegisterMethodDelegate<System.Object>();
        //UnityAction委托注册(ILRuntime必须转换为C#的事件委托)
        appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)act)();
            });
        });

        //EventTriggerHandler封装的委托注册
        appDomain.DelegateManager.RegisterDelegateConvertor<EventTriggerListener.VoidDelegate>((act) =>
        {
            return new EventTriggerListener.VoidDelegate((go) =>
            {
                ((System.Action<GameObject>)act)(go);
            });
        });
        //EventTriggerHandler封装的委托注册
        appDomain.DelegateManager.RegisterDelegateConvertor<EventTriggerListener.VoidDragDelegate>((act) =>
        {
            return new EventTriggerListener.VoidDragDelegate((go, dt) =>
            {
                ((System.Action<GameObject, PointerEventData>)act)(go, dt);
            });
        });
    }

}
