using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingTon<T> : MonoBehaviour where T: MonoBehaviour
{
    /// <summary>
    /// �̳�MonoBehaviour�����࣬�첽������MonoBehaviour
    /// </summary>

    #region ����
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
