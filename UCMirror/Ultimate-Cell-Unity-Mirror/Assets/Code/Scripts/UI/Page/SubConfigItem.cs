using Common;
using GameFrameWork;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubConfigItem : BaseUI
{
    /// <summary>
    /// 配置英雄子页
    /// 用于继承BaseUI实现的抽象类
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.SubConfigItem;
    }

    #region 页面交互控件
    public Button HeroBtn;
    public TextMeshProUGUI HeroName;
    public Image HeroImage;
    public Image HeroBG;
    public GameObject HeroPoint;
    #endregion

    #region PrivateValue

    private object[] hero = null;
    private GameObject SwimEffect;
    //拖动时候的父物体
    private GameObject DragingObj;
    private ResManager.Hero heroTemp;
    #endregion

    #region 业务逻辑
    public override void OnAwake()
    {
        DragingObj = GameObject.FindWithTag("FatherObj");

        //建立事件监听
        //点击事件
        BtnEvent.RigisterButtonClickEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnClick(); });
        //浮动监听事件
        BtnEvent.RigisterButtonEnterEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnEnter();});
        BtnEvent.RigisterButtonExitEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnExit(); });
        //拖动事件
        BtnEvent.RigisterButtonDragEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnDrag(); });
        BtnEvent.RigisterButtonDragEndEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnEndGrag(); });
    }

    public override void OnUpdate(float dealTime)
    {
        base.OnUpdate(dealTime);
    }
    #endregion

    #region 数据加载
    public override void OnLoadData(params object[] param)
    {
        //得到数据
        hero = param;
        heroTemp = (ResManager.Hero)hero[0];
        //确认数据是否异常
        if(heroTemp != null)
        {
            //如果当前英雄已经解锁了
            if (heroTemp._unlock)
            {
                if (heroTemp._beConfig)
                {
                    HeroBG.sprite = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "BeConfigItemBG");
                    // Resources.Load("Img/BeConfigItemBG", typeof(Sprite)) as Sprite;
                }
                else
                {
                    HeroBG.sprite = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "ConfigItemBG");
                    // Resources.Load("Img/ConfigItemBG", typeof(Sprite)) as Sprite;
                }
                //读取已解锁数据到UI上
                HeroName.text = heroTemp._name;
                HeroImage.sprite = heroTemp._image;
            }
            else
            {
                //将UI设置为未解锁状态
                HeroName.text = "???";
                HeroImage.color = Color.black;
            }
        }
        else
        {
            //关闭当前页面
            //UIManager.Instance.CloseSubUI();
            throw new Exception("英雄数据异常，检查数据完整性！");
        }
    }
    #endregion

    #region 事件监听

    #region 点击事件
    void OnHeroBtnClick()
    {
        if(hero != null)
        {
            //打开英雄升级页面
            UIManager.Instance.OpenUI(UIType.HeroUpgradePage, hero);
        }
        else
        {
            throw new Exception("英雄为空");
        }
    }
    #endregion

    #region 浮动事件
    void OnHeroBtnEnter()
    {
        //加载特效
        if (!SwimEffect)
        {
            // TODO there is no blue fire field
            SwimEffect = ABManager.Instance.LoadResource<GameObject>("gameobj", "Ulimate_BgEffect");
            // Resources.Load<GameObject>("Prefabs/Effect/FireFieldBlue");
            Vector3 pos = HeroBtn.transform.position;
            pos.y = 0;
            Quaternion rot  = Quaternion.Euler(0,0,0);
            SwimEffect = GameObject.Instantiate(SwimEffect, pos, rot);

        }

    }

    void OnHeroBtnExit()
    {
        //移除特效
        if (SwimEffect)
        {
            Destroy(SwimEffect);
        }
    }
    #endregion

    #region 拖动事件
    void OnHeroBtnDrag()
    {
        //设置角色被拖动状态
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        HeroImage.transform.position = screenToWorld;
        HeroImage.transform.SetParent(DragingObj.transform,true);
    }

    void OnHeroBtnEndGrag()
    {
        //获取当前点下所有的UI
        HeroImage.transform.position = HeroPoint.transform.position;
        HeroImage.transform.SetParent(HeroPoint.transform, true);
        GameObject tempHeroItem = GetHeroUiItem();
        if (tempHeroItem != null)
        {
            BaseUI _baseUI = tempHeroItem.GetComponent<BaseUI>();
            _baseUI.SetUI(hero);
        }
    }

    /// <summary>
    /// 获取是BeConfigItem的子物体（如果是BeConfigItem Tag，就代表可以传输）
    /// </summary>
    /// <returns></returns>
    private GameObject GetHeroUiItem()
    {
        
        List<RaycastResult> list = new List<RaycastResult>();
        GameObject heroItem = null;
        //获取场景中的EventSystem,将鼠标位置传给eventData
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, list);
        for(int i = 0; i < list.Count; i++)
        {
            GameObject obj = list[i].gameObject;
            if(obj.tag == "BeConfigItem")
            {
                heroItem = obj;
            }
        }
        return heroItem;
    }
    #endregion

    #endregion

}
