using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneReference[] sceneToLoads;
    [HideInInspector]
    public int data; 
    [HideInInspector]
    public Test3DState state3D;
    public enum Test3DState
    {
        Active,
        Destory
    }
    public UnityAction<int> OnDataPass;
    public UnityAction<Test3DState> OnDestoryPass;
    void Awake()
    {
        //数据通讯：
        data = 0;
        DontDestroyOnLoad(this.transform.gameObject);
        //场景加载：
        foreach (var sceneToLoad in sceneToLoads)
        {
            SceneManager.LoadSceneAsync(sceneToLoad,LoadSceneMode.Additive);
        }
        

    }
    public void fireDataEvent()
    {
        if(OnDataPass != null)
		{
			OnDataPass(this.data);
		}
    }

    public void fireDestory3DEvent()
    {
        if(OnDestoryPass != null)
		{
			OnDestoryPass(this.state3D);
		}
    }
}
