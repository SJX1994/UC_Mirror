
using UnityEngine;

[CreateAssetMenu(fileName = "Idel", menuName = "状态机/创建Idel模板", order = 1)]

public class IdelTemplate: ScriptableObject
{
    [Tooltip("idea类型")]
    public IdelType idelType = IdelType.Blocks;

    [Tooltip("精灵样式")]
    public Sprite blue , red , green , purple;
   
    public enum IdelType
    {
      Blocks,
      Skills
    }
   
    
}
