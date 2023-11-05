using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UC_PlayerData;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Mirror;
public class BlocksReferee : NetworkBehaviour
{
#region 数据对象
    BlocksCreator_Main blocksCreator_Main;
    public BlocksCreator_Main BlocksCreator_Main
    {
        get
        {
            if(!blocksCreator_Main)blocksCreator_Main = GetComponent<BlocksCreator_Main>();
            return blocksCreator_Main;
        }
    }
    public UnityAction OnTimerComplete;
    RectTransform gameoverPage;
    public RectTransform GameoverPage
    {
        get
        {
            if(!gameoverPage)gameoverPage = Resources.Load<RectTransform>("UI/FinishPage");
            return gameoverPage;
        }
    }
   
#endregion 数据对象
#region 联网数据对象
    RectTransform victoryPage;
    public RectTransform VictoryPage
    {
        get
        {
            if(!victoryPage)victoryPage = Resources.Load<RectTransform>("UI/VictoryPage");
            return victoryPage;
        }
    }
    RectTransform defeatedPage;
    public RectTransform DefeatedPage
    {
        get
        {
            if(!defeatedPage)defeatedPage = Resources.Load<RectTransform>("UI/DefeatedPage");
            return defeatedPage;
        }
    }
    RectTransform equivalentPage;
    public RectTransform EquivalentPage
    {
        get
        {
            if(!equivalentPage)equivalentPage = Resources.Load<RectTransform>("UI/EquivalentPage");
            return equivalentPage;
        }
    }
#endregion 联网数据对象
#region 数据关系
    public void Awake()
    {
        DOTween.Clear();
    }
    public void Active()
    {
        ResetTimer();
    }
    private void Update()
    {
        if(!Referee.isTimerRunning_ReverseOrder)return;
        UpdateStaticTimerString();
        Referee.currentTime_ReverseOrder -= Time.deltaTime;
        if (Referee.currentTime_ReverseOrder > 0) return;
        TimerComplete();
    }
#endregion 数据关系
#region 数据操作
    public void StopTimer()
    {
        bool stopCounting = false;
        Referee.isTimerRunning_ReverseOrder = stopCounting;
    }
    public void ResetTimer()
    {
        bool startCounting = true;
        Referee.currentTime_ReverseOrder = Referee.TotalTime_ReverseOrder;
        Referee.isTimerRunning_ReverseOrder = startCounting;
        Referee.gameover = false;
    }
    void TimerComplete()
    {
        StopTimer();
        OnTimerComplete?.Invoke();
        if(BlocksData.Player1_numb > BlocksData.Player2_numb)
        {
            Referee.Winner = Player.Player1;
        }
        else if(BlocksData.Player1_numb < BlocksData.Player2_numb)
        {
            Referee.Winner = Player.Player2;
        }
        else
        {
            Referee.Winner = Player.NotReady;
        }
        GameOver();
        
    }
    void GameOver()
    {
        if(Local())
        {
            if(Referee.gameover)return;
            Referee.gameover = true;
            gameoverPage = Instantiate(GameoverPage);
        }else
        {
            if(!isServer)return;
            if(Referee.gameover)return;
            Referee.gameover = true;
            gameoverPage = Instantiate(GameoverPage);
            Client_GameOver(BlocksData.Player1_numb,BlocksData.Player2_numb);
        }
        
    }
    public void Restart()
    {
        DOTween.Clear(true);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    private void UpdateStaticTimerString()
    {
        int minutes = Mathf.FloorToInt(Referee.currentTime_ReverseOrder / 60f);
        int seconds = Mathf.FloorToInt(Referee.currentTime_ReverseOrder % 60f);
        Referee.timerText_ReverseOrder = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // 砖块数胜负判定
    public void CheckLoseByBloksLess(Vector2 posId = default(Vector2), int state = 0)
    {
        BlockTetriHandler.BlockTetriState blockTetriState = (BlockTetriHandler.BlockTetriState)state;
        if(blockTetriState == BlockTetriHandler.BlockTetriState.Occupying || blockTetriState == BlockTetriHandler.BlockTetriState.Peace_Player1 || blockTetriState == BlockTetriHandler.BlockTetriState.Peace_Player2 || blockTetriState == BlockTetriHandler.BlockTetriState.Peace)return;
        if(BlocksData.Player1_numb <= 4 ) Referee.Winner = Player.Player2;
        if(BlocksData.Player2_numb <= 4 ) Referee.Winner = Player.Player1;
        if(Referee.Winner == Player.NotReady)return;
        StopTimer();
        GameOver();
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [ClientRpc]
    void Client_GameOver(int Player1_numb,int Player2_numb)
    {
        Player localPlayer = ServerLogic.Local_palayer;
        if(Player1_numb == Player2_numb)
        {
            equivalentPage = Instantiate(EquivalentPage);
            return;
        }
        else if(localPlayer == Player.Player1 && Player1_numb > Player2_numb)
        {
            victoryPage = Instantiate(VictoryPage);
        }
        else if(localPlayer == Player.Player2 && Player1_numb < Player2_numb)
        {
            victoryPage = Instantiate(VictoryPage);
        }
        else
        {
            defeatedPage = Instantiate(DefeatedPage);
        }
    }

#endregion 联网数据操作
}