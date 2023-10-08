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
    public void Active()
    {
        ResetTimer();
    }
    // 计时器胜负判定
    private void Update()
    {
        if(!Referee.isTimerRunning)return;
        UpdateTimerString(); // 更新UI上的计时器显示
        Referee.currentTime -= Time.deltaTime; // 更新当前计时时间
        if (Referee.currentTime > 0) return;
        TimerComplete();
    }
    public void StopTimer()
    {
        Referee.isTimerRunning = false; // 停止计时
    }
    public void ResetTimer()
    {
        Referee.currentTime = Referee.totalTime; // 重置当前计时时间
        Referee.isTimerRunning = true; // 开始计时
        Referee.gameover = false; // 游戏未结束
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
    private void UpdateTimerString()
    {
        // 将当前计时时间转换为分秒格式，并显示在UI上
        int minutes = Mathf.FloorToInt(Referee.currentTime / 60f);
        int seconds = Mathf.FloorToInt(Referee.currentTime % 60f);
        Referee.timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // 砖块数胜负判定
    public void CheckLose(Vector2 posId = default(Vector2), int state = 0)
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