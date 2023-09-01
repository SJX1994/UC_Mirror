using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class ChangeLiquid : MonoBehaviour
{
    /// <summary>
    /// 液体
    /// </summary>
    public GameObject buttom;
    /// <summary>
    /// 液面
    /// </summary>
    public GameObject top;
    /// <summary>
    /// 完成指示灯
    /// </summary>
    public GameObject ready;
    private IdelBox ideaBox;
    float fade;
    Vector3 originPositionTop;
   
    void Start()
    {
        // 通信获取
        // TODO 暂时获取方式

        buttom = transform.Find("liquidSet").Find("liquid_bottom").gameObject;
        top = transform.Find("liquidSet").Find("liquid_top").gameObject;
        ready = transform.Find("IdelReady").gameObject;

        ready.SetActive(true);
        originPositionTop = top.GetComponent<RectTransform>().anchoredPosition;
        ideaBox = transform.GetComponent<IdelBox>();
        top.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        buttom.GetComponent<Image>().fillAmount = 1;
        ChangeColor();
    }
   
        
    public void DoCount()
    {
        //ChangeColor();
        top.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ready.SetActive(false);
        ideaBox.Idel.SetActive(false);
        top.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f,0f,0f),6f);
        DOVirtual.Float(0, 1, 6f, (float value) =>
        {
            fade = value;
            buttom.GetComponent<Image>().fillAmount = value;
        }).onComplete=() =>
        {
             ready.SetActive(true);
             ideaBox.Idel.SetActive(true);
             ideaBox.OnGameObjCreate();
        };
        
    }

    public void ChangeColor()
    {
        switch(ideaBox.blockGrade)
        {
            case EventType.BlocksGrade.BottomGrade:
                buttom.GetComponent<Image>().DOColor(Color.green,0.5f);
                top.GetComponent<Image>().DOColor(Color.green,0.5f);
            break;
            case EventType.BlocksGrade.MiddleGrade:
                buttom.GetComponent<Image>().DOColor(Color.cyan,0.5f);
                top.GetComponent<Image>().DOColor(Color.cyan,0.5f);
            break;
            case EventType.BlocksGrade.TopGrade:
                buttom.GetComponent<Image>().DOColor(new Color32( 143 , 0 , 254, 255 ),0.5f);
                top.GetComponent<Image>().DOColor(new Color32( 143 , 0 , 254, 255 ),0.5f);
            break;
        }
    }


  
}
