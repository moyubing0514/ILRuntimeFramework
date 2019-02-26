using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HotFix
{
    public class HotFixLoop : IGameHotFixInterface
    {
        private static HotFixLoop m_Instance;
        private Mono2DllFunction m_Mono2DllFunction = new Mono2DllFunction();


        public override void Start()
        {
            m_Instance = this;
            Debug.Log("HotFix Start Ok");
        }

        public override void Update()
        {

        }

        private void OnCameraPositionChangedEvent()
        {
        }

        public static HotFixLoop GetInstance()
        {
            return m_Instance;
        }

        public override void OnDestroy()
        {

        }
        public override void OnApplicationQuit()
        {
            UnityEngine.Debug.Log("HotFixLoop OnApplicationQuit");

        }

        public override object OnMono2GameDll(string func, params object[] data)
        {
            return m_Mono2DllFunction.OnMono2GameDll(func, data);
        }
    }
}
