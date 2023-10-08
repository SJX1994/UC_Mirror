using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Mirror;
using UC_PlayerData;
using UnityEngine.Events;
public class ChangeLiquid : NetworkBehaviour
{
    /// <summary>
    /// 液体
    /// </summary>
    public GameObject columnLiquid;
    public GameObject ColumnLiquid
    {
        get
        {
            if(columnLiquid)return columnLiquid;
            columnLiquid = transform.Find("liquidSet").Find("liquid_Mask").Find("liquid_bottom").gameObject;
            return columnLiquid;
        }
    }
    public GameObject topLiquid;
    public GameObject TopLiquid
    {
        get
        {
            if(topLiquid)return topLiquid;
            topLiquid = transform.Find("liquidSet").Find("liquid_Mask").Find("liquid_top").gameObject;
            return topLiquid;
        }
    }
    public GameObject readyLight;
    public GameObject ReadyLight
    {
        get
        {
            if(readyLight)return readyLight;
            readyLight = transform.Find("IdelReady").gameObject;
            return readyLight;
        }
    }
    public GameObject Halo
    {
        get
        {
            if(ReadyLight.transform.childCount > 0)
            {
                return ReadyLight.transform.GetChild(0).gameObject;
            }
            return null;
        }
    }
    private IdelBox ideaBox;
    float fade;
    int level = 0;
    int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            OnLevelUp?.Invoke(level);
        }
    }
    public UnityAction<int> OnLevelUp;
    Vector3 originPositionTop;
    Sequence blinkSequence;
    float blinkDuration = 3f;
    float resetStartBlinkIntensity;
    float startBlinkIntensity = 0.25f;
    float StartBlinkIntensity
    {
        get
        {
            float maxStartBlinkIntensity = 0.72f;
            if(startBlinkIntensity > maxStartBlinkIntensity)startBlinkIntensity = maxStartBlinkIntensity;
            return startBlinkIntensity;
        }
    }
    float resetEndBlinkIntensity;
    float endBlinkIntensity = 0.3f;
    float EndBlinkIntensity
    {
        get
        {
            float maxEndBlinkIntensity = 1f;
            if(endBlinkIntensity > maxEndBlinkIntensity)endBlinkIntensity = maxEndBlinkIntensity;
            return endBlinkIntensity;
        }
    }
    float haloblinkDuration = 3.2f;
    float resetHaloStartBlinkIntensity;
    float haloStartBlinkIntensity = 0.0f;
    float HaloStartBlinkIntensity
    {
        get
        {
            float maxHaloStartBlinkIntensity = 0.32f;
            if(haloStartBlinkIntensity > maxHaloStartBlinkIntensity)haloStartBlinkIntensity = maxHaloStartBlinkIntensity;
            return haloStartBlinkIntensity;
        }
    }
    float resetHaloEndBlinkIntensity;
    float haloEndBlinkIntensity = 0.32f;
    float HaloEndBlinkIntensity
    {
        get
        {
            float maxHaloEndBlinkIntensity = 0.9f;
            if(haloEndBlinkIntensity > maxHaloEndBlinkIntensity)haloEndBlinkIntensity = maxHaloEndBlinkIntensity;
            return haloEndBlinkIntensity;
        }
    }
    Sequence blinkAnimationSequence;
    Sequence haloBlinkAnimationSequence;
    Tween topTween;
    Tween fillLiquidTween;
    void Start()
    {

        ReadyLight.SetActive(true);
        originPositionTop = TopLiquid.GetComponent<RectTransform>().anchoredPosition;
        ideaBox = transform.GetComponent<IdelBox>();
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ColumnLiquid.GetComponent<Image>().fillAmount = 1;
        resetStartBlinkIntensity = StartBlinkIntensity;
        resetEndBlinkIntensity = EndBlinkIntensity;
        resetHaloStartBlinkIntensity = HaloStartBlinkIntensity;
        resetHaloEndBlinkIntensity = HaloEndBlinkIntensity;
        UpLevelBlinkAnimation();
    }
    void UpLevelBlinkAnimation(float strengthIncrement = 0.21f)
    {
       
        blinkAnimationSequence?.Kill();
        haloBlinkAnimationSequence?.Kill();
        int loopForever = -1;
        startBlinkIntensity += strengthIncrement * Level;
        endBlinkIntensity += strengthIncrement * Level;
        blinkAnimationSequence = DOTween.Sequence();
        Image readyLightImage = ReadyLight.GetComponent<Image>();
        readyLightImage.color = new Color(readyLightImage.color.r,readyLightImage.color.g,readyLightImage.color.b,StartBlinkIntensity * Level);
        blinkAnimationSequence.Append(readyLightImage.DOFade(StartBlinkIntensity, blinkDuration / 2));
        blinkAnimationSequence.Append(readyLightImage.DOFade(EndBlinkIntensity, blinkDuration / 2));
        blinkAnimationSequence.SetLoops(loopForever,LoopType.Yoyo); 
        if( ReadyLight.transform.childCount <= 0 )return;
        Image haloImage = Halo.GetComponent<Image>();
        haloImage.color = new Color(haloImage.color.r,haloImage.color.g,haloImage.color.b,HaloStartBlinkIntensity * Level);
        haloStartBlinkIntensity += strengthIncrement * Level;
        haloEndBlinkIntensity += strengthIncrement * Level;
        haloBlinkAnimationSequence = DOTween.Sequence();
        haloBlinkAnimationSequence.Append(haloImage.DOFade(HaloStartBlinkIntensity, haloblinkDuration / 2));
        haloBlinkAnimationSequence.Append(haloImage.DOFade(HaloEndBlinkIntensity, haloblinkDuration / 2));
        haloBlinkAnimationSequence.SetLoops(loopForever,LoopType.Yoyo); 
    }
    public void DoCount(float countdown = 6f)
    {
        ResetLiquidLoader();
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ReadyLight.SetActive(false);
        ideaBox.idelContainer.SetActive(false);
        ideaBox.ClickEvent.SetActive(false);
        Vector3 finallyReachesPosition = new Vector3(-46f,59f,0f);
        float fillAmountStart = 0;
        float fillAmountEnd = 1;
        topTween = TopLiquid.GetComponent<RectTransform>().DOAnchorPos(finallyReachesPosition,countdown);
        fillLiquidTween = DOVirtual.Float(fillAmountStart, fillAmountEnd, countdown, (TweenCallback<float>)((float value) =>
        {
            fade = value;
            this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
        }));
        fillLiquidTween.onComplete=() =>
        {
            ReadyLight.SetActive(true);
            ideaBox.ClickEvent.SetActive(true);
            ideaBox.idelContainer.SetActive(true);
            ideaBox.OnGameObjCreate();
            DoCount_UpLevel();
        };
    }
    public void DoCount_UpLevel(float countdown = 6f)
    {
        topTween?.Kill();
        fillLiquidTween?.Kill();
        int maxLevel = 3;
        if(level >= maxLevel)return;
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        Vector3 finallyReachesPosition = new Vector3(-46f,59f,0f);
        float fillAmountStart = 0;
        float fillAmountEnd = 1;
        topTween = TopLiquid.GetComponent<RectTransform>().DOAnchorPos(finallyReachesPosition,countdown);
        fillLiquidTween = DOVirtual.Float(fillAmountStart, fillAmountEnd, countdown, (TweenCallback<float>)((float value) =>
        {
            fade = value;
            this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
        }));
        fillLiquidTween.onComplete=() =>
        {
            Level++;
            UpLevelBlinkAnimation();
            DoCount_UpLevel(countdown);
        };
    }
    public void ResetLiquidLoader()
    {
        level = 0;
        List<Tween> tweens = new List<Tween>();
        if(blinkAnimationSequence!=null){tweens.Add(blinkAnimationSequence);}
        if(haloBlinkAnimationSequence!=null){tweens.Add(haloBlinkAnimationSequence);}
        if(topTween!=null){tweens.Add(topTween);}
        if(fillLiquidTween!=null){tweens.Add(fillLiquidTween);}
        foreach(var tween in tweens)
        {
            if(tween == null)continue;
            if(tween.IsPlaying())tween?.Kill();
        }
        startBlinkIntensity = resetStartBlinkIntensity;
        endBlinkIntensity = resetEndBlinkIntensity;
        haloStartBlinkIntensity = resetHaloStartBlinkIntensity;
        haloEndBlinkIntensity = resetHaloEndBlinkIntensity;
        Image readyLightImage = ReadyLight.GetComponent<Image>();
        readyLightImage.color = new Color(readyLightImage.color.r,readyLightImage.color.g,readyLightImage.color.b,0);
        Image haloImage = Halo.GetComponent<Image>();
        haloImage.color = new Color(haloImage.color.r,haloImage.color.g,haloImage.color.b,0);
        ColumnLiquid.GetComponent<Image>().fillAmount = 0;
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
    }
    [Client]
    public void Client_DoCount()
    {
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ReadyLight.SetActive(false);
        ideaBox.idelContainer.SetActive(false);
        TopLiquid.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-46f,59f,0f),6f);
        DOVirtual.Float(0, 1, 6.3f, (TweenCallback<float>)((float value) =>
        {
            fade = value;
            this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
            foreach(var tetris in FindObjectsOfType<TetrisBlockSimple>())
            {
                if(tetris.player != Player.NotReady)return;
                // 是否需要在 客户端idelBox 还没获取的时候 隐藏 俄罗斯方块组
            }
        })).onComplete=() =>
        {
            ReadyLight.SetActive(true);
            ideaBox.idelContainer.SetActive(true);
            ideaBox.ClientGetTetrisGroupID();
        };
        
    }
    public void ChangeColor(Color color)
    {
        if(!ideaBox.idelUI.hiden)
        {
            ColumnLiquid.GetComponent<Image>().DOColor(color,0.5f);
            TopLiquid.GetComponent<Image>().DOColor(color,0.5f);
        }else
        {
            Color colorHiden = new(color.r,color.g,color.b,Dispaly.HidenAlpha);
            ColumnLiquid.GetComponent<Image>().DOColor(colorHiden,0.5f);
            TopLiquid.GetComponent<Image>().DOColor(colorHiden,0.5f);
        }
        
    }
    


  
}
