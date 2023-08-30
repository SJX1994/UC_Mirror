using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingTon<T> where T : class, new()
{
    /// <summary>
    /// �����࣬���ڸ�UI��������̳�
    /// </summary>


    #region ������
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
            Debug.Log("ʵ�����쳣��");
        }
    }

    public virtual void Init()
    {
        
    }
}
