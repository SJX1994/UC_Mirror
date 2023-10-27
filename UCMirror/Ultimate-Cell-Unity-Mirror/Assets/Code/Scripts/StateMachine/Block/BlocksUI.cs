using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UC_PlayerData;
using DG.Tweening;
using Mirror;
public class BlocksUI : NetworkBehaviour
{
#region 数据对象
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
#endregion 数据对象
#region 联网数据对象
    [SyncVar]
    string timerText;
    [SyncVar]
    float progress_player1_SyncVar;
    [SyncVar]
    float progress_player2_SyncVar;
#endregion 联网数据对象
#region 数据关系
    void Start()
    {
        Img_player1.fillAmount = 20;
        Img_player2.fillAmount = 20;
    }
    void LateUpdate()
    {
        Dispaly_Timer();
    }
#endregion 数据关系
#region 数据操作
    void Dispaly_Timer()
    {
        
        if(Local())
        {
            Tex_timer.text = Referee.timerText_ReverseOrder;
        }else
        {
            if(!isServer)return;
            timerText = Referee.timerText_ReverseOrder;
            Tex_timer.text = timerText;
            Client_Dispaly_Timer();
        }
        
    }
    public void Display_Process()
    {
        if(Local())
        {
            progress_player1 = Remap(BlocksData.Player1_numb,0,200,0,1);
            progress_player2 = Remap(BlocksData.Player2_numb,0,200,0,1);
            Img_player1.fillAmount = progress_player1;
            Img_player2.fillAmount = progress_player2;
            Image Img_player1Inner = Img_player1.transform.GetChild(0).GetComponent<Image>();
            Image Img_player2Inner = Img_player2.transform.GetChild(0).GetComponent<Image>();
            Img_player1Inner.fillAmount = Img_player1.fillAmount;
            Img_player2Inner.fillAmount = Img_player2.fillAmount;
            Text_player1.text = BlocksData.Player1_numb.ToString() + "/" + BlocksData.max_numb.ToString();
            Text_player2.text = BlocksData.Player2_numb.ToString()+ "/" + BlocksData.max_numb.ToString();
            // Display_whoBetterAnimat();
        }else
        {
            if(!isServer)return;
            progress_player1_SyncVar = Remap(BlocksData.Player1_numb,0,200,0,1);
            progress_player2_SyncVar = Remap(BlocksData.Player2_numb,0,200,0,1);
            Img_player1.fillAmount = progress_player1_SyncVar;
            Img_player2.fillAmount = progress_player2_SyncVar;
            Image Img_player1Inner = Img_player1.transform.GetChild(0).GetComponent<Image>();
            Image Img_player2Inner = Img_player2.transform.GetChild(0).GetComponent<Image>();
            Img_player1Inner.fillAmount = Img_player1.fillAmount;
            Img_player2Inner.fillAmount = Img_player2.fillAmount;
            Text_player1.text = BlocksData.Player1_numb.ToString() + "/" + BlocksData.max_numb.ToString();
            Text_player2.text = BlocksData.Player2_numb.ToString()+ "/" + BlocksData.max_numb.ToString();
            Client_Display_Process(Text_player1.text,Text_player2.text);
        }
        
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
#endregion 数据操作
#region 联网数据关系
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [ClientRpc]
    void Client_Dispaly_Timer()
    {
        Tex_timer.text = timerText;
    }
    [ClientRpc]
    void Client_Display_Process(string text_player1,string text_player2)
    {
        Img_player1.fillAmount = progress_player1_SyncVar;
        Img_player2.fillAmount = progress_player2_SyncVar;
        Image Img_player1Inner = Img_player1.transform.GetChild(0).GetComponent<Image>();
        Image Img_player2Inner = Img_player2.transform.GetChild(0).GetComponent<Image>();
        Img_player1Inner.fillAmount = Img_player1.fillAmount;
        Img_player2Inner.fillAmount = Img_player2.fillAmount;
        Text_player1.text = text_player1;
        Text_player2.text = text_player2;
    }
#endregion 联网数据关系
}