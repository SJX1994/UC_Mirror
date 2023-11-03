using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace UC_PlayerData
{
    public class LeaderData
    {
        public static WhoTalking whoTalking = WhoTalking.None;
        public enum WhoTalking
        {
            Player1,
            Player2,
            None,
        }
        public LeaderState leaderState = LeaderState.Idle; 
        public enum LeaderState
        {
            Idle,
            Attack,
            Die,
            Move,
        }
        public LeaderType leaderType = LeaderType.NotSet;
        public enum LeaderType
        {
            Bao,
            Zhao,
            Duo,
            Weak,
            NotSet,
        }
        public LeaderDirection leaderDirection = LeaderDirection.NotSet;
        public enum LeaderDirection
        {
            Left,
            Right,
            NotSet,
        }
        public LeaderPosition leaderPosition = LeaderPosition.NotSet;
        public enum LeaderPosition
        {
            Left,
            Right,
            NotSet,
        }
        
    }
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
        public const float MaxOccupyingTime = 6.5f;
        public const float MinOccupyingTime = 1.5f;
    }
    public class Dispaly
    {
        public const int FlowOrder = 1000;
        public const int NotFlowOrder = 14;
        public const int NormalUnitOrder = 101;
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
        public static float BlocksAlpha_watchingFight = 0.0f;
        public static float BlocksAlpha_commandTheBattle = 1.0f;
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
        public const float InIdelbox_CreatFirstCountdown = 9f;
        public const float InIdelbox_CreatCountdown = 9f;
        public const float InIdelbox_UpLevelCountdown = 6f;
        public static UnityAction OnTimeBeforStartFinish_FromKeyTimeCounter;
        public const float TotalTime_ReverseOrder = 180f;
        public static float currentTime_ReverseOrder = 0f;
        public static bool isTimerRunning_ReverseOrder = false;
        public static string timerText_ReverseOrder = "00:00";
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
        public static Player local_palayer = Player.NotReady;
        public static bool isGameStart = false;
        // 道具Id
        // private static int PropsID = 10_0000_0;
        // 俄罗斯砖块组ID
        private static int TetrisGroupID = 10_0000;
        // 俄罗斯砖块预示ID
        private static int TetrisGroupIDTempP1 = 1;
        private static int TetrisGroupIDTempP2 = 2;
        // 俄罗斯砖块ID
        private static int Tetris = 1_0000;
        // 砖块ID
        private static int Block = 1000;
        // public static int GetPropsID()
        // {
        //     return PropsID++;
        // }
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
        public static string[] tetrominoesName = new string[]
        {
            "I Tetromino",
            "J Tetromino",
            "L Tetromino",
            "O Tetromino",
            "S Tetromino",
            "T Tetromino",
            "Z Tetromino",
        };
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
    public static class UIData
    {
        public const float MAX_MORALE = 500f;
        public static float player1MoraleAccumulationAdditionContinuedTime = 12f;
        public static float player2MoraleAccumulationAdditionContinuedTime = 12f;
        public static bool player1isAdditioning = false;
        public static bool player2isAdditioning = false;
        private static float player1MoraleAccumulation = 0;
        public static UnityAction<Player> OnPlayer1MoraleAccumulationMaxed;
        public static UnityAction<Player> OnPlayer1MoraleAccumulationAdditionFinished;

        public static float Player1MoraleAccumulation
        {
            get
            {
                return player1MoraleAccumulation;
            }
            set
            {
                player1MoraleAccumulation = value;
                if(player1MoraleAccumulation <= MAX_MORALE)return;
                player1MoraleAccumulation = MAX_MORALE;
                if(player1isAdditioning)return;
                OnPlayer1MoraleAccumulationMaxed?.Invoke(Player.Player1);
                player1isAdditioning = true;
            }
        }
        public static float Player1MoraleAccumulation_Normalized
        {
            get
            {
                return UC_Tool.Remap(player1MoraleAccumulation,0,MAX_MORALE,0,1);
            }
        }
        private static float player2MoraleAccumulation = 0;
        public static UnityAction<Player> OnPlayer2MoraleAccumulationMaxed;
        public static UnityAction<Player> OnPlayer2MoraleAccumulationAdditionFinished;
        public static float Player2MoraleAccumulation
        {
            get
            {
                return player2MoraleAccumulation;
            }
            set
            {
                player2MoraleAccumulation = value;
                if(player2MoraleAccumulation <= MAX_MORALE)return;
                player2MoraleAccumulation = MAX_MORALE;
                if(player2isAdditioning)return;
                OnPlayer2MoraleAccumulationMaxed?.Invoke(Player.Player2);
                player2isAdditioning = true;
            }
        }
        public static float Player2MoraleAccumulation_Normalized
        {
            get
            {
                return UC_Tool.Remap(player2MoraleAccumulation,0,MAX_MORALE,0,1);
            }
        }

    }
    public static class UserAction
    {
        public enum State
        {
            CommandTheBattle_IdeaBox,
            CommandTheBattle_Buoy,
            WatchingFight,
            Loading,
        }
        public static UnityAction<State> OnPlayer1UserActionStateChanged;
        public static UnityAction<State> OnPlayer2UserActionStateChanged;
        private static State player1UserState = State.WatchingFight;
        private static State player2UserState = State.WatchingFight;
        public static State Player1UserState
        {
            get
            {
                return player1UserState;
            }
            set
            {
                if(player1UserState == value) return;
                player1UserState = value;
                OnPlayer1UserActionStateChanged?.Invoke(player1UserState);
            }
        }
        public static State Player2UserState
        {
            get
            {
                return player2UserState;
            }
            set
            {
                if(player2UserState == value) return;
                player2UserState = value;
                OnPlayer2UserActionStateChanged?.Invoke(player2UserState);
            }
        }
        
    }
    public static class UC_Tool
    {
        public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            float t = Mathf.InverseLerp(oldLow, oldHigh, input);
            return Mathf.Lerp(newLow, newHigh, t);
        }
        
        public static Quaternion Diff(this Quaternion to, Quaternion from)
        {
            return to * Quaternion.Inverse(from);
        }
        public static Quaternion Add(this Quaternion start, Quaternion diff)
        {
            return diff * start;
        }

    }
}