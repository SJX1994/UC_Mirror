using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UC_PlayerData;
using DG.Tweening;
using UnityEngine.Events;

public class BlocksUI_KeyTimeCounter : MonoBehaviour
{
    public Sprite zero,one,two,three,four,five,six,seven,eight,nine;
    Image imageRenderer;
    Image ImageRenderer
    {
        get
        {
            if(!imageRenderer)imageRenderer = transform.GetComponent<Image>();
            return imageRenderer;
        }
    }
    int time = 0;
    int Time_BeforEndGame
    {
        get
        {
            return time;
        }
        set
        {
            if(time == value)return;
            time = value;
            DisplayTheKeyTimeCounter_BeforEndGame();
            if(isGameStart)return;
            time_WhenGameStart = Time_WhenGameStart;
        }
    }
    int time_WhenGameStart = 0;
    bool isGameStart = false;
    int Time_WhenGameStart
    {
        get
        {
            time_WhenGameStart = (int)(Referee.InIdelbox_CreatFirstCountdown + 1f) - Mathf.Abs(time - (int)Referee.TotalTime_ReverseOrder);
            if(time_WhenGameStart == 0)
            {
                isGameStart = true;
                DisplayTheKeyTimeCounter_WhenGameStart_Stop();
            }else
            {
                DisplayTheKeyTimeCounter_WhenGameStart();
            }
            return time_WhenGameStart;
        }
    }
    void Update()
    {
        Time_BeforEndGame = (int)Referee.currentTime_ReverseOrder;    
    }
    void DisplayTheKeyTimeCounter_BeforEndGame()
    {
        float endCountdown = 9f;
        if(Time_BeforEndGame > endCountdown)return;

        Vector3 startSize = Vector3.one * 3f;
        Vector3 endSize = Vector3.one;
        float endAlpha = 0f;
        ImageRenderer.rectTransform.localScale = startSize;
        Tween tween_WhenGameStart_Alpha = ImageRenderer.DOFade(endAlpha,1f).SetEase(Ease.InOutSine);
        tween_WhenGameStart_Alpha.OnComplete(()=>{ImageRenderer.color = Color.white;});
        Tween tween_WhenGameStart_Scale = ImageRenderer.rectTransform.DOScale(endSize,1f).SetEase(Ease.InOutSine);
        tween_WhenGameStart_Scale.OnComplete(()=>{
            ImageRenderer.rectTransform.localScale = startSize;
        });

        ImageRenderer.color = Color.white;
        switch (Time_BeforEndGame)
        {
            case 0:
                ImageRenderer.sprite = zero;
                break;
            case 1:
                ImageRenderer.sprite = one;
                break;
            case 2:
                ImageRenderer.sprite = two;
                break;
            case 3:
                ImageRenderer.sprite = three;
                break;
            case 4:
                ImageRenderer.sprite = four;
                break;
            case 5:
                ImageRenderer.sprite = five;
                break;
            case 6:
                ImageRenderer.sprite = six;
                break;
            case 7:
                ImageRenderer.sprite = seven;
                break;
            case 8:
                ImageRenderer.sprite = eight;
                break;
            case 9:
                ImageRenderer.sprite = nine;
                break;
            default:
                break;
        }
    }
    void DisplayTheKeyTimeCounter_WhenGameStart()
    {
        Vector3 startSize = Vector3.one * 4.2f;
        Vector3 endSize = Vector3.one;
        ImageRenderer.rectTransform.localScale = startSize;
        Tween tween_WhenGameStart = ImageRenderer.rectTransform.DOScale(endSize,1f).SetEase(Ease.OutBounce);
        tween_WhenGameStart.OnComplete(()=>{
            ImageRenderer.rectTransform.localScale = startSize;
        });
        if(ImageRenderer.color != Color.white)ImageRenderer.color = Color.white;
        switch (time_WhenGameStart)
        {
            case 0:
                ImageRenderer.sprite = zero;
                break;
            case 1:
                ImageRenderer.sprite = one;
                break;
            case 2:
                ImageRenderer.sprite = two;
                break;
            case 3:
                ImageRenderer.sprite = three;
                break;
            case 4:
                ImageRenderer.sprite = four;
                break;
            case 5:
                ImageRenderer.sprite = five;
                break;
            case 6:
                ImageRenderer.sprite = six;
                break;
            case 7:
                ImageRenderer.sprite = seven;
                break;
            case 8:
                ImageRenderer.sprite = eight;
                break;
            case 9:
                ImageRenderer.sprite = nine;
                break;
            default:
                break;
        }
    }
    void DisplayTheKeyTimeCounter_WhenGameStart_Stop()
    {
        ImageRenderer.color = Color.clear;
        Referee.OnTimeBeforStartFinish_FromKeyTimeCounter?.Invoke();
    }
}