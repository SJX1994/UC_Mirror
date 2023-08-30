using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using System.Collections;
using System;
using static UIManager;
using Common;

public class UIManager : SingTon<UIManager>
{
    /// <summary>
    /// 存放UI的数据
    /// _type 状态
    /// _path 路径
    /// _uipaeam 参数
    /// </summary>
    public class UIInfoData
    {
        public UIType _type { private set; get; }
        public string _path { private set; get; }
        public object[] _uipaeam { private set; get; }
        public Type _ScriptsType { private set; get; }

        public UIInfoData(UIType type, string path, object[] uipaeam)
        {
            _type = type;
            _path = path;
            _uipaeam = uipaeam;
            _ScriptsType = PathDefines.GetScriptsWithUIType(_type);
        }
    }
    
    /// <summary>
    /// 界面管理的词典
    /// </summary>
    public Dictionary<UIType, GameObject> UITypesDics = null;

    /// <summary>
    /// UI推入的栈
    /// </summary>
    public Stack<UIInfoData> stackOpenUI = null;

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        UITypesDics = new Dictionary<UIType,GameObject>();
        stackOpenUI = new Stack<UIInfoData>();
    }

    /// <summary>
    /// 根据词典获取UI Prefab
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public GameObject GetUIObj(UIType _type)
    {
        GameObject obj = null;
        if(UITypesDics.TryGetValue(_type, out obj))
        {
            return obj;
        }
        else
        {
            throw new System.Exception("字典没有对应值");
        }
    }

    /// <summary>
    /// 查看场景是否在词典中（是否被打开）
    /// </summary>
    /// <param name="_type"></param>
    /// <returns>是否被打开</returns>
    public bool TryGetUI(UIType _type)
    {
        bool _isInstance;
        GameObject obj = null;
        _isInstance = UITypesDics.TryGetValue(_type, out obj);
        return _isInstance;
    }

    /// <summary>
    /// 获取所有BaseUI或者继承自BaseUI的类
    /// 如果词典能找到对应的UI，那么就返回UI下的BaseUI类
    /// </summary>
    /// <typeparam name="T">所有UIType的泛类</typeparam>
    /// <param name="_type"></param>
    /// <returns></returns>
    public T GetUI<T>(UIType _type) where T : BaseUI
    {
        GameObject getObj = GetUIObj(_type);
        if(getObj != null)
        {
            return null;
        }
        else
        {
            return getObj.GetComponent<T>();
        }
    }

    #region OpenUI
    /// <summary>
    /// 打开UI
    /// </summary>
    public void OpenUI(UIType[] _types)
    {
        OpenUI(false, _types, null);
    }

    public void OpenUICloseOtherUI(UIType[] _types)
    {
        OpenUI(true, _types, null);
    }

    public void OpenUI(UIType _type, object[] _uiparam)
    {
        UIType[] _types = new UIType[1];
        _types[0] = _type;
        OpenUI(false, _types, _uiparam);
    }

    public void OpenUICloseOtherUI(UIType _type, object[] _uiparam)
    {
        UIType[] _types = new UIType[1];
        _types[0] = _type;
        OpenUI(true, _types, _uiparam);
    }

    /// <summary>
    /// 开启UI界面方法
    /// </summary>
    /// <param name="_isCloseOtherUI">是否关闭其他UI</param>
    /// <param name="_types">UI类型/param>
    /// <param name="_uiparam">数据</param>
    public void OpenUI(bool _isCloseOtherUI, UIType[] _types,object[] _uiparam)
    {
        if (_isCloseOtherUI)
        {
            //关闭其他UI
            CloseAllUI();
        }
        //首先找到所有的UItype
        for(int i = 0; i < _types.Length; i++)
        {
            UIType _type = _types[i];
            //如果栈内没有对应Type的，就将对应Type的数据推入栈
            if (!UITypesDics.ContainsKey(_type))
            {
                string _path = PathDefines.GetUIPathWithUIType(_type);
                stackOpenUI.Push(new UIInfoData(_type, _path, _uiparam));
            }
        }
        //如果栈内有数据的话,就打开界面
        if(stackOpenUI.Count > 0)
        {
            //异步加载UI
            CoroutineCtrl.Instance.StartCoroutine(AsyncLoadData());

        }
    }

    public void RefreshUI() 
    {
        UITypesDics = new();
    }

    public void OpenSubUI( UIType _type, object[] _uiparam,GameObject parentObj)
    {
        string _path = PathDefines.GetUIPathWithUIType(_type);
        GameObject _prefab = ABManager.Instance.LoadResource<GameObject>("uipage", _path);
            // Resources.Load<GameObject>(_path);
        GameObject _uiObj = GameObject.Instantiate(_prefab);
        _uiObj.transform.SetParent(parentObj.transform, false);
        BaseUI _baseUI = _uiObj.GetComponent<BaseUI>();
        _baseUI.SetUI(_uiparam);

    }

    /// <summary>
    /// 异步加载UI界面
    /// </summary>
    /// <returns></returns>
    public IEnumerator AsyncLoadData()
    {
        UIInfoData _infoData = null;
        GameObject _prefab = null;
        GameObject _uiObj = null;
        if(stackOpenUI.Count > 0 && stackOpenUI != null)
        {
            do
            {
                _infoData = stackOpenUI.Pop();
                _prefab = ABManager.Instance.LoadResource<GameObject>("uipage", _infoData._path);
                if(_prefab == null)
                {
                    throw new Exception("没有找到UI资源");
                }
                else
                {
                    _uiObj = GameObject.Instantiate(_prefab);
                    BaseUI _baseUI = _uiObj.GetComponent<BaseUI>();
                    if(_baseUI == null)
                    {
                        _baseUI = _uiObj.AddComponent(_infoData._ScriptsType) as BaseUI;
                    }
                    else
                    {
                        _baseUI.SetUI(_infoData._uipaeam);
                    }
                    //将Obj加入词典
                    UITypesDics.Add(_infoData._type, _uiObj);
                }
            } while (stackOpenUI.Count > 0);
        }

        yield return 0;
    }

    #endregion

    #region CloseUI
    /// <summary>
    /// 关闭所有UI
    /// </summary>
    public void CloseAllUI()
    {
        List<UIType> _uiTypeList = new List<UIType>(UITypesDics.Keys);
        for(int i = 0; i < _uiTypeList.Count; i++)
        {
            CloseUI(_uiTypeList[i]);
        }
    }

    /// <summary>
    /// 关闭UI界面方法
    /// </summary>
    /// <param name="_uiType"></param>
    public void CloseUI(UIType _uiType)
    {
        GameObject _uiTypeObj = GetUIObj(_uiType);
        //如果没有obj，那么从词典移除UI
        if(_uiTypeObj == null)
        {
            UITypesDics.Remove(_uiType);
        }
        //如果有obj，在baseui里面调用Release销毁关闭
        else
        {
            BaseUI _baseUI = _uiTypeObj.GetComponent<BaseUI>();
            if (_baseUI == null)
            {
                GameObject.Destroy(_uiTypeObj);
                UITypesDics.Remove(_uiType);
            }
            else
            {
                _baseUI.StateChange += CloseUIHandle;
                _baseUI.Release();
            }
        }
    }

    public void CloseSubUI(UIType _uiType)
    {

    }

    public void CloseUI(UIType[] _uiTypes)
    {
        for(int i = 0; i < _uiTypes.Length; i++)
        {
            CloseUI(_uiTypes[i]);
        }
    }

    private void CloseUIHandle(object obj, UIState OldState, UIState NewState)
    {
        if(NewState == UIState.Closing)
        {
            BaseUI _baseUI = obj as BaseUI;
            UITypesDics.Remove(_baseUI.GetUIType());
            _baseUI.StateChange -= CloseUIHandle;
        }
    }
    #endregion
}
