
using UnityEngine;

public class PVP_faction: MonoBehaviour
{
    // 静态字段
    private static faction myFaction = faction.Environmentalism;
    private static faction enemyFaction = faction.None;
    public enum faction
    {
        None,
        Environmentalism, // 环保主义
        Socialism, // 社会主义
        Capitalism, // 自由市场
        Technological,// 科技乐观主义
        Cthulhuism,// 克苏鲁主义
    }
    public static faction MyFaction
    {
          get { return myFaction; }
          set
          {
                if (value != myFaction)
                {
                    myFaction = value;
                    SaveMyFaction();
                }
          }
    }
    public static faction EnemyFaction
    {
          get { return enemyFaction; }
          set
          {
                if (value != enemyFaction)
                {
                    enemyFaction = value;
                    SaveEnemyFaction();
                }
          }
    }
    public static void SaveMyFaction()
    {
        // 实现保存分数的逻辑
    }
    public static void SaveEnemyFaction()
    {
        // 实现保存分数的逻辑
    }

    public static void LoadMyFaction()
    {
        // 实现加载分数的逻辑
    }
}
