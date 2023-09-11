using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UC_PlayerData
{
    
    public enum Player
    {
        Player1,
        Player2,
        NotReady,
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
    public static class ServerLogic
    {
        // 俄罗斯砖块组ID
        private static int TetrisGroupID = 10_0000;
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
    }
}