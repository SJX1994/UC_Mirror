using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingTon<T> : MonoBehaviour where T: MonoBehaviour
{
    /// <summary>
    /// 继承MonoBehaviour单例类，异步依赖于MonoBehaviour
    /// </summary>

    #region 单例
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject go = GameObject.Find("MonoSingObj");
                if(go == null)
                {
                    go = new GameObject("MonoSingObj");
                    DontDestroyOnLoad(go);
                }
                _instance = go.AddComponent<T>();
            }
            return _instance;
        }
    }
    #endregion
}
