using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UC_PlayerData;
using DG.Tweening;

public class BlocksUI : MonoBehaviour
{
    Image img_player1;
    TextMeshProUGUI text_player1;
    float progress_player1 = 0f;
    Image img_player2;
    TextMeshProUGUI text_player2;
    float progress_player2 = 0f;
    void Start()
    {
        if(!img_player1)img_player1 = transform.Find("Player1").GetComponent<Image>();
        if(!img_player2)img_player2 = transform.Find("Player2").GetComponent<Image>();
        if(!text_player1)text_player1 = img_player1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if(!text_player2)text_player2 = img_player2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }
    public void Display_Process()
    {
        progress_player1 = Remap(BlocksData.Player1_numb,0,200,0,1);
        progress_player2 = Remap(BlocksData.Player2_numb,0,200,0,1);
        if(!img_player1)Start();
        if(!img_player2)Start();
        img_player1.fillAmount = progress_player1;
        img_player2.fillAmount = progress_player2;
        if(!text_player1)Start();
        if(!text_player2)Start();
        text_player1.text = BlocksData.Player1_numb.ToString() + "/" + BlocksData.max_numb.ToString();
        text_player2.text = BlocksData.Player2_numb.ToString()+ "/" + BlocksData.max_numb.ToString();
        Display_whoBetterAnimat();
    }
    void Display_whoBetterAnimat()
    {
        if(BlocksData.Player1_numb > BlocksData.Player2_numb)
        {
            img_player1.transform.DOScaleY(1.2f,0.5f).SetEase(Ease.OutBounce);
            img_player2.transform.DOScaleY(0.8f,0.5f).SetEase(Ease.OutBounce);
        }else
        {
            img_player2.transform.DOScaleY(1.2f,0.5f).SetEase(Ease.OutBounce);
            img_player1.transform.DOScaleY(0.8f,0.5f).SetEase(Ease.OutBounce);
        }
    }
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
}