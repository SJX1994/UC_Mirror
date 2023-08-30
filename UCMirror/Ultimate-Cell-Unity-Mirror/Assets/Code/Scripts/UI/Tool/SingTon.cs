using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingTon<T> where T : class, new()
{
    /// <summary>
    /// 单例类，用于给UI控制组件继承
    /// </summary>


    #region 单例类
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    #endregion

    public SingTon()
    {
        if (_instance == null)
        {
            Init();
        }
        else
        {
            Debug.Log("实例化异常！");
        }
    }

    public virtual void Init()
    {
        
    }
}
