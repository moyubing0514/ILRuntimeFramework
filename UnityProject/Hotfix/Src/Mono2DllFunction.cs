using System;
using System.Collections.Generic;
using System.Text;

namespace HotFix
{
    public class Mono2DllFunction
    {
        public object OnMono2GameDll(string func, params object[] data)
        {
            if (func == "GameResultMessageHF_OnGameOver")
            {
                //GameResultMessageHF_OnGameOver(data);
            }
            else if(func == "PlayerMyself_GetId")
            {
                return 0;// PlayerMyself_GetId();
            }
            return null;
        }
    }
}
