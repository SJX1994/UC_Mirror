using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace UC_PlayerData
{
    public enum Player
    {
        Player1,
        Player2,
        NotReady,
    }
    public class UnitData
    {
        public enum Color
        {
            notReady,
            red,
            green,
            blue,
            purple,
            yellow,
        }
        public const string Temp = "_Temp";
    }
    public class Dispaly
    {
        // 心流
        public const int FlowOrder = 1000;
        public const int NotFlowOrder = 14;
        // 棋盘
        public const int MaxWidth = 20;
        public const int MaxHeight = 10;
        public static bool IsInCheckerboard(Vector2 posId)
        {
            bool isInWidthRange = (posId.x >= 0) && (posId.x <= MaxWidth);
            bool isInHeightRange = (posId.y >= 0) && (posId.y <= MaxHeight);
            bool isIntegerWidth = Mathf.Approximately(Mathf.Round(posId.x), posId.x);
            bool isIntegerHeight = Mathf.Approximately(Mathf.Round(posId.y), posId.y);
            return  isInWidthRange && isInHeightRange && isIntegerWidth && isIntegerHeight;
        }
        // 颜色
        public static Color Player1Color = new(Color.red.r + 0.3f,Color.red.g+ 0.3f,Color.red.b+ 0.3f,0.6f);
        public static Color Player2Color = new(Color.blue.r+ 0.3f,Color.blue.g+ 0.3f,Color.blue.b+ 0.3f,0.6f);
        // PVP
        public static float HidenAlpha = 0.1f;
    }
    public enum RunMode
    {
        Host,
        Local,
    }
    public static class RunModeData
    {
        public static RunMode CurrentRunMode { get; set; }

        public static void ChangeRunMode(RunMode newState)
        {
            CurrentRunMode = newState;
        }
    }
    public static class BlocksData
    {
        public static bool stopEventSend = false; 
        public static UnityAction<int> OnPlayer1BlocksNumbChange;
        public static int player1_numb = 20;
        public static int Player1_numb
        {
            get { return player1_numb; }
            set
            {
                if (value == player1_numb || value == 0 ) return;
                player1_numb = value;
                if(stopEventSend) return;
                OnPlayer1BlocksNumbChange?.Invoke(player1_numb);
            }
        }
        public static UnityAction<int> OnPlayer2BlocksNumbChange;
        public static int player2_numb = 20;
        public static int Player2_numb
        {
            get { return player2_numb; }
            set
            {
                if (value == player2_numb || value == 0) return;
                player2_numb = value;
                if(stopEventSend) return;
                OnPlayer2BlocksNumbChange?.Invoke(player2_numb);
            }
        }
        public static int peace_numb = 0;
        public static int max_numb = 200;
        public enum BlocksMechanismType
        {
            NoneType,
            WeakAssociation,
            ReachBottomLine,
            ReachBottomLineGain,
            FullRows,
        }
    }
    public static class Referee
    {
        public static float currentTime = 0f; // 当前计时时间
        public static float totalTime = 180f; // 当前计时时间
        public static bool isTimerRunning = false; // 计时器是否正在运行
        public static string timerText = "00:00"; // 计时器UI显示的文本
        private static Player winner = Player.NotReady;
        public static Player Winner
        {
            get
            {
                return winner;
            }
            set
            {
                winner = value;
                Debug.Log("Winner is " + winner);
            }
        }
        public static bool gameover = false;
    }
    public static class ServerLogic
    {
        // 俄罗斯砖块组ID
        private static int TetrisGroupID = 10_0000;
        // 俄罗斯砖块预示ID
        private static int TetrisGroupIDTempP1 = 1;
        private static int TetrisGroupIDTempP2 = 2;
        // 俄罗斯砖块ID
        private static int Tetris = 1_0000;
        // 砖块ID
        private static int Block = 1000;
        public static int GetTetrisGroupID()
        {
            return TetrisGroupID++;
        }
        public static int GetTetrisID()
        {
            return Tetris++;
        }
        public static int GetBlockID()
        {
            return Block++;
        }
        public static int GetTetrisGroupIDTemp(int player = 1)
        {
            if(player == 2)
            {
                return TetrisGroupIDTempP2;
            }else
            {
                return TetrisGroupIDTempP1;
            }
            
        }
    }
    public class PropsData
    {
        public enum PropsState
        {
            None,
            ChainBall,
            MoveDirectionChanger,
            Obstacle,
            HealthAdder,
        }
        public enum MoveDirection
        {
            Up,
            Down,
            NotReady,
        }
    }
}