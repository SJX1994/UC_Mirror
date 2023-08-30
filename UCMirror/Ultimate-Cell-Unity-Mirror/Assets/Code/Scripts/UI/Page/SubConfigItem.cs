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
    /// ����Ӣ����ҳ
    /// ���ڼ̳�BaseUIʵ�ֵĳ�����
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.SubConfigItem;
    }

    #region ҳ�潻���ؼ�
    public Button HeroBtn;
    public TextMeshProUGUI HeroName;
    public Image HeroImage;
    public Image HeroBG;
    public GameObject HeroPoint;
    #endregion

    #region PrivateValue

    private object[] hero = null;
    private GameObject SwimEffect;
    //�϶�ʱ��ĸ�����
    private GameObject DragingObj;
    private ResManager.Hero heroTemp;
    #endregion

    #region ҵ���߼�
    public override void OnAwake()
    {
        DragingObj = GameObject.FindWithTag("FatherObj");

        //�����¼�����
        //����¼�
        BtnEvent.RigisterButtonClickEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnClick(); });
        //���������¼�
        BtnEvent.RigisterButtonEnterEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnEnter();});
        BtnEvent.RigisterButtonExitEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnExit(); });
        //�϶��¼�
        BtnEvent.RigisterButtonDragEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnDrag(); });
        BtnEvent.RigisterButtonDragEndEvent(HeroBtn.transform.gameObject, p => { OnHeroBtnEndGrag(); });
    }

    public override void OnUpdate(float dealTime)
    {
        base.OnUpdate(dealTime);
    }
    #endregion

    #region ���ݼ���
    public override void OnLoadData(params object[] param)
    {
        //�õ�����
        hero = param;
        heroTemp = (ResManager.Hero)hero[0];
        //ȷ�������Ƿ��쳣
        if(heroTemp != null)
        {
            //�����ǰӢ���Ѿ�������
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
                //��ȡ�ѽ������ݵ�UI��
                HeroName.text = heroTemp._name;
                HeroImage.sprite = heroTemp._image;
            }
            else
            {
                //��UI����Ϊδ����״̬
                HeroName.text = "???";
                HeroImage.color = Color.black;
            }
        }
        else
        {
            //�رյ�ǰҳ��
            //UIManager.Instance.CloseSubUI();
            throw new Exception("Ӣ�������쳣��������������ԣ�");
        }
    }
    #endregion

    #region �¼�����

    #region ����¼�
    void OnHeroBtnClick()
    {
        if(hero != null)
        {
            //��Ӣ������ҳ��
            UIManager.Instance.OpenUI(UIType.HeroUpgradePage, hero);
        }
        else
        {
            throw new Exception("Ӣ��Ϊ��");
        }
    }
    #endregion

    #region �����¼�
    void OnHeroBtnEnter()
    {
        //������Ч
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
        //�Ƴ���Ч
        if (SwimEffect)
        {
            Destroy(SwimEffect);
        }
    }
    #endregion

    #region �϶��¼�
    void OnHeroBtnDrag()
    {
        //���ý�ɫ���϶�״̬
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        HeroImage.transform.position = screenToWorld;
        HeroImage.transform.SetParent(DragingObj.transform,true);
    }

    void OnHeroBtnEndGrag()
    {
        //��ȡ��ǰ�������е�UI
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
    /// ��ȡ��BeConfigItem�������壨�����BeConfigItem Tag���ʹ�����Դ��䣩
    /// </summary>
    /// <returns></returns>
    private GameObject GetHeroUiItem()
    {
        
        List<RaycastResult> list = new List<RaycastResult>();
        GameObject heroItem = null;
        //��ȡ�����е�EventSystem,�����λ�ô���eventData
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
