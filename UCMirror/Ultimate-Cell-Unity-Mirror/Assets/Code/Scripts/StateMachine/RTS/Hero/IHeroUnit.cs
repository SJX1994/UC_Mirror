using UnityEngine;
using System.Collections;
public interface IHeroUnit
{
  
    public UnitHeroTemplate OnCreating()
    {
        // 生成时，让人充满希望的口号和救世主的感觉
        UnitHeroTemplate unitHeroTemplate = new UnitHeroTemplate();
        return unitHeroTemplate;
    }
    public UnitHeroTemplate OnDefeated()
    {
        // 战败后需要令人铭记 虽败犹荣的感觉
        UnitHeroTemplate unitHeroTemplate = new UnitHeroTemplate();
        return unitHeroTemplate;
    }
    public IEnumerator WhenCreatMoveTo(Vector3 position)
    {
        // 生成后的移动位置
        yield return null;
    }
   
}
