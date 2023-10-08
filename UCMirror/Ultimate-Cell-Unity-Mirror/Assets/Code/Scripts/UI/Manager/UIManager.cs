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
    /// ���UI������
    /// _type ״̬
    /// _path ·��
    /// _uipaeam ����
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
    /// ��������Ĵʵ�
    /// </summary>
    public Dictionary<UIType, GameObject> UITypesDics = null;

    /// <summary>
    /// UI�����ջ
    /// </summary>
    public Stack<UIInfoData> stackOpenUI = null;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public override void Init()
    {
        UITypesDics = new Dictionary<UIType,GameObject>();
        stackOpenUI = new Stack<UIInfoData>();
    }

    /// <summary>
    /// ���ݴʵ��ȡUI Prefab
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
            throw new System.Exception("�ֵ�û�ж�Ӧֵ");
        }
    }

    /// <summary>
    /// �鿴�����Ƿ��ڴʵ��У��Ƿ񱻴򿪣�
    /// </summary>
    /// <param name="_type"></param>
    /// <returns>�Ƿ񱻴�</returns>
    public bool TryGetUI(UIType _type)
    {
        bool _isInstance;
        GameObject obj = null;
        _isInstance = UITypesDics.TryGetValue(_type, out obj);
        return _isInstance;
    }

    /// <summary>
    /// ��ȡ����BaseUI���߼̳���BaseUI����
    /// ����ʵ����ҵ���Ӧ��UI����ô�ͷ���UI�µ�BaseUI��
    /// </summary>
    /// <typeparam name="T">����UIType�ķ���</typeparam>
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
    /// ��UI
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
    /// ����UI���淽��
    /// </summary>
    /// <param name="_isCloseOtherUI">�Ƿ�ر�����UI</param>
    /// <param name="_types">UI����/param>
    /// <param name="_uiparam">����</param>
    public void OpenUI(bool _isCloseOtherUI, UIType[] _types,object[] _uiparam)
    {
        if (_isCloseOtherUI)
        {
            //�ر�����UI
            CloseAllUI();
        }
        //�����ҵ����е�UItype
        for(int i = 0; i < _types.Length; i++)
        {
            UIType _type = _types[i];
            //���ջ��û�ж�ӦType�ģ��ͽ���ӦType����������ջ
            if (!UITypesDics.ContainsKey(_type))
            {
                string _path = PathDefines.GetUIPathWithUIType(_type);
                stackOpenUI.Push(new UIInfoData(_type, _path, _uiparam));
            }
        }
        //���ջ�������ݵĻ�,�ʹ򿪽���
        if(stackOpenUI.Count > 0)
        {
            //�첽����UI
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
        // GameObject _prefab = ABManager.Instance.LoadResource<GameObject>("uipage", _path);
        GameObject _prefab = Resources.Load<GameObject>(_path);
        GameObject _uiObj = GameObject.Instantiate(_prefab);
        _uiObj.transform.SetParent(parentObj.transform, false);
        BaseUI _baseUI = _uiObj.GetComponent<BaseUI>();
        _baseUI.SetUI(_uiparam);

    }

    /// <summary>
    /// �첽����UI����
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
                    throw new Exception("û���ҵ�UI��Դ");
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
                    //��Obj����ʵ�
                    UITypesDics.Add(_infoData._type, _uiObj);
                }
            } while (stackOpenUI.Count > 0);
        }

        yield return 0;
    }

    #endregion

    #region CloseUI
    /// <summary>
    /// �ر�����UI
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
    /// �ر�UI���淽��
    /// </summary>
    /// <param name="_uiType"></param>
    public void CloseUI(UIType _uiType)
    {
        GameObject _uiTypeObj = GetUIObj(_uiType);
        //���û��obj����ô�Ӵʵ��Ƴ�UI
        if(_uiTypeObj == null)
        {
            UITypesDics.Remove(_uiType);
        }
        //�����obj����baseui�������Release���ٹر�
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
