using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UC_PlayerData;
using DG.Tweening;

public class BlocksUI : MonoBehaviour
{
    Image img_player1;
    public Image Img_player1
    {
        get
        {
            if(!img_player1)img_player1 = transform.Find("Player1CaptureProgressBar").GetComponent<Image>();
            return img_player1;
        }
    }
    TextMeshProUGUI text_player1;
    public TextMeshProUGUI Text_player1
    {
        get
        {
            if(!text_player1)text_player1 = Img_player1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            return text_player1;
        }
    }
    float progress_player1 = 0f;
    Image img_player2;
    public Image Img_player2
    {
        get
        {
            if(!img_player2)img_player2 = transform.Find("Player2CaptureProgressBar").GetComponent<Image>();
            return img_player2;
        }
    }
    TextMeshProUGUI text_player2;
    public TextMeshProUGUI Text_player2
    {
        get
        {
            if(!text_player2)text_player2 = Img_player2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            return text_player2;
        }
    }
    float progress_player2 = 0f;
    TextMeshProUGUI tex_timer;
    public TextMeshProUGUI Tex_timer
    {
        get
        {
            if(!tex_timer)tex_timer = transform.Find("TimeCounter").Find("Text").GetComponent<TextMeshProUGUI>();
            return tex_timer;
        }
    }
    void Update()
    {
        Dispaly_Timer();
    }
    void Dispaly_Timer()
    {
        Tex_timer.text = Referee.timerText;
    }
    public void Display_Process()
    {
        progress_player1 = Remap(BlocksData.Player1_numb,0,200,0,1);
        progress_player2 = Remap(BlocksData.Player2_numb,0,200,0,1);
        Img_player1.fillAmount = progress_player1;
        Img_player2.fillAmount = progress_player2;
        Text_player1.text = BlocksData.Player1_numb.ToString() + "/" + BlocksData.max_numb.ToString();
        Text_player2.text = BlocksData.Player2_numb.ToString()+ "/" + BlocksData.max_numb.ToString();
        Display_whoBetterAnimat();
    }
    void Display_whoBetterAnimat()
    {
        if(BlocksData.Player1_numb > BlocksData.Player2_numb)
        {
            Img_player1.transform.DOScaleY(1.2f,0.5f).SetEase(Ease.OutBounce);
            Img_player2.transform.DOScaleY(0.8f,0.5f).SetEase(Ease.OutBounce);
        }else
        {
            Img_player2.transform.DOScaleY(1.2f,0.5f).SetEase(Ease.OutBounce);
            Img_player1.transform.DOScaleY(0.8f,0.5f).SetEase(Ease.OutBounce);
        }
    }
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
}