using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UC_PlayerData;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class BlocksReferee : MonoBehaviour
{
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
            if(!gameoverPage)gameoverPage = Resources.Load<RectTransform>("UI/VictoryPage");
            return gameoverPage;
        }
    }
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
        if(Referee.gameover)return;
        Referee.gameover = true;
        gameoverPage = Instantiate(GameoverPage);
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
}