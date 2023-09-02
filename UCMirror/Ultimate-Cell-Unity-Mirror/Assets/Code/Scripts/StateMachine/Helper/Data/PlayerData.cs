using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
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
    }
    
}