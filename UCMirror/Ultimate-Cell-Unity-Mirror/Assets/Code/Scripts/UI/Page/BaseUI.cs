using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using System;

public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// �������ں���
    /// ����Transform,Gameobject
    /// </summary>

    #region ����
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

    #region ����UIҳ��״̬��״̬����
    /// <summary>
    /// ������ǰҳ��״̬�����Ҹı�״̬
    /// </summary>
    protected UIState _state = UIState.None;
    public event UIStateDelegate StateChange;
    public UIState State
    {
        //�õ���ǰ״̬
        get { return _state; }
        //1.����֮ǰ��״̬ 2.�����µ�״̬ 3.����״̬�ı��֪ͨ
        set { UIState OldState = _state;_state = value;if (StateChange != null) { StateChange(this, OldState, _state); } }
    }
    #endregion

    //��ȡ��ͬ��UIҳ��
    public abstract UIType GetUIType();

    #region ui״̬����
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
        //����ȴ�UI������غò���ִ��
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

    #region �̳и�д����
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public virtual void OnAwake()
    {
        this.State = UIState.Loading;
        //��������
        PlayUIAudio();
    }

    /// <summary>
    /// ��ʼ
    /// </summary>
    public virtual void OnStart()
    {
        //ҵ���߼�
    }

    /// <summary>
    /// ֡ͬ������UI
    /// </summary>
    /// <param name="dealTime"></param>
    public virtual void OnUpdate(float dealTime)
    {

    }

    /// <summary>
    /// �ر�UI
    /// </summary>
    public virtual void OnRelease()
    {
        this.State = UIState.None;
        //�ر�����
        StopUIAudio();
    }
    #endregion

    #region �������ֺ���
    /// <summary>
    /// ��������
    /// </summary>
    public virtual void PlayUIAudio()
    {

    }

    /// <summary>
    /// �ر�����
    /// </summary>
    public virtual void StopUIAudio()
    {

    }
    #endregion

    #region ���ݼ���
    /// <summary>
    /// ����UI���ݷ���
    /// </summary>
    public virtual void OnLoadData(params object[] param)
    {

    }

    /// <summary>
    /// ����UI����(�������첽����)
    /// </summary>
    /// <param name="param">��������</param>
    public virtual void SetUI(params object[] param)
    {
        SetUI();
        StartCoroutine(LoadDataAsync(param));
    }

    /// <summary>
    /// ����UI����(ֱ�Ӽ���)
    /// </summary>
    public virtual void SetUI()
    {
        this.State = UIState.Loading;
    }

    /// <summary>
    /// �첽�������ݷ���
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadDataAsync(params object[] param)
    {
        //�����Ҫ������
        yield return new WaitForSeconds(0);
        if(this.State == UIState.Loading)
        {
            //��������
            OnLoadData(param);
            //����״̬
            this.State = UIState.Ready;
        }
    }

    #endregion
}
