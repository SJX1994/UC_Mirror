using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using System;

public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// 生命周期函数
    /// 缓存Transform,Gameobject
    /// </summary>

    #region 缓存
    private GameObject _cacheObj;
    public GameObject CacheObj
    {
        get
        {
            if(_cacheObj == null)
            {
                _cacheObj = this.gameObject;
            }
            return _cacheObj;
        }
    }

    private Transform _cachetransform;
    public Transform CacheTransform
    {
        get
        {
            if(_cachetransform == null)
            {
                _cachetransform = this.transform;
            }
            return _cachetransform;
        }
    }
    #endregion

    #region 监听UI页面状态和状态更改
    /// <summary>
    /// 监听当前页面状态，并且改变状态
    /// </summary>
    protected UIState _state = UIState.None;
    public event UIStateDelegate StateChange;
    public UIState State
    {
        //得到当前状态
        get { return _state; }
        //1.保存之前的状态 2.接收新的状态 3.发出状态改变的通知
        set { UIState OldState = _state;_state = value;if (StateChange != null) { StateChange(this, OldState, _state); } }
    }
    #endregion

    //获取不同的UI页面
    public abstract UIType GetUIType();

    #region ui状态函数
    private void Awake()
    {
        this.State = UIState.Init;
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        //必须等待UI界面加载好才能执行
        if(this.State == UIState.Ready)
        {
            OnUpdate(Time.deltaTime);
        }
    }

    public void Release()
    {
        this.State = UIState.Closing;
        Destroy(this.CacheObj);
        OnRelease();
    }

    private void OnDestroy()
    {
        this.State = UIState.None;
    }
    #endregion

    #region 继承覆写函数
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void OnAwake()
    {
        this.State = UIState.Loading;
        //播放音乐
        PlayUIAudio();
    }

    /// <summary>
    /// 开始
    /// </summary>
    public virtual void OnStart()
    {
        //业务逻辑
    }

    /// <summary>
    /// 帧同步更新UI
    /// </summary>
    /// <param name="dealTime"></param>
    public virtual void OnUpdate(float dealTime)
    {

    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    public virtual void OnRelease()
    {
        this.State = UIState.None;
        //关闭音乐
        StopUIAudio();
    }
    #endregion

    #region 背景音乐函数
    /// <summary>
    /// 播放音乐
    /// </summary>
    public virtual void PlayUIAudio()
    {

    }

    /// <summary>
    /// 关闭音乐
    /// </summary>
    public virtual void StopUIAudio()
    {

    }
    #endregion

    #region 数据加载
    /// <summary>
    /// 加载UI数据方法
    /// </summary>
    public virtual void OnLoadData(params object[] param)
    {

    }

    /// <summary>
    /// 加载UI方法(有数据异步加载)
    /// </summary>
    /// <param name="param">传入数据</param>
    public virtual void SetUI(params object[] param)
    {
        SetUI();
        StartCoroutine(LoadDataAsync(param));
    }

    /// <summary>
    /// 加载UI方法(直接加载)
    /// </summary>
    public virtual void SetUI()
    {
        this.State = UIState.Loading;
    }

    /// <summary>
    /// 异步加载数据方法
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadDataAsync(params object[] param)
    {
        //如果需要进度条
        yield return new WaitForSeconds(0);
        if(this.State == UIState.Loading)
        {
            //加载数据
            OnLoadData(param);
            //设置状态
            this.State = UIState.Ready;
        }
    }

    #endregion
}
