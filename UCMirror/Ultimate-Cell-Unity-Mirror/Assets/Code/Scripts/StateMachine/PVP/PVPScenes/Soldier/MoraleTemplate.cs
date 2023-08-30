
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Morale_", menuName = "士气", order = 1)]

public class MoraleTemplate : ScriptableObject{
      // 士气值
      public float morale;
      
      public float maxMorale;
      public float minMorale;
      [HideInInspector]
      public float baseminMorale;
      // 兵种
      public SoldierType soldierType;
      public enum SoldierType
      {
            Red,
            Blue,
            Green,
            Purple
      }
      // 形式因：波及范围是一个圆形的圈
      // 波及范围(个体对群体的影响)
      public float affectedRange;
      public Color affectedRangeColor;
      public bool isAffectedRangeVisible = true;
      // 每个单位自身的基础属性都要乘以这个系数
      public void EffectByMorale( Soldier soldier,ref float value )
      {
            value = soldier.maxStrength * soldier.morale.morale;
      }
      // 基于心理学的 自我赋能感
      public void AddMorale(Soldier soldier, float value, bool source = false)
      {
            if(source)
            {
                  Soldier[] soldiers = FindObjectsOfType<Soldier>();
                  Transform circleCenter = soldier.transform;
                  float circleRadius = soldier.morale.affectedRange;
                  int soldiersInCircleCount = 0;
                  foreach (Soldier s in soldiers)
                  {
                        if(s==soldier)continue;
                        // 获取单位的坐标
                        Vector3 sPosition = s.transform.position;

                        // 计算单位与圆心的距离
                        float distance = Vector3.Distance(sPosition, circleCenter.position);
                        
                        // 检查距离是否小于或等于圆的半径
                        if (distance <= circleRadius)
                        {
                              // 将距离映射到大小的范围
                              float size = 1/distance;
                              // Debug.Log("距离：" + distance + "，强度：" + size);
                              s.morale.AddMorale(s,value * size,false);
                              s.morale.EffectByMorale(s,ref s.strength);
                              soldiersInCircleCount++;
                        }
                  }
                  // Debug.Log("在半径内的单位数量：" + soldiersInCircleCount);
            }
            soldier.morale.morale += value;
            
      }
      public void ReduceMorale(Soldier soldier, float value, bool source = false)
      {
            
            if(soldier.morale.morale>0.6f)
            {
                  soldier.morale.morale -= value;
            }
            
      }
      public static void DrawMoraleRange(Vector3 center, float radius, Color color, float duration = 0)
      {
            // #if UNITY_EDITOR
            //       Handles.color = color;
            //       Handles.DrawWireCube(center, Vector3.one * radius * 2);
            // #endif
            Vector3 prevPoint = Vector3.zero;
            float segmentAngle = 10; // 控制圆形的分段数

            for (float angle = 0; angle <= 360; angle += segmentAngle)
            {
                  float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + center.x;
                  float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + center.z;
                  Vector3 currPoint = new Vector3(x, center.y, z);

                  if (prevPoint != Vector3.zero)
                  {
                        #if UNITY_EDITOR
                              Handles.color = color;
                              Handles.DrawLine(prevPoint, currPoint);
                        #else
                              Debug.DrawLine(prevPoint, currPoint, color, duration);
                        #endif
                  }

                  prevPoint = currPoint;
            }
      }
      public void ModifyBaseMinMorale(Soldier soldier,float value)
      {
            soldier.morale.minMorale += value;
      }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Soldier))]
public class SoldierEditor : Editor
{
    Soldier myScript;
    SoldierEditor()
    {
        // 注册场景视图回调函数
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private  void OnSceneGUI(SceneView sceneView)
    {
        if (target == null)
        {
            return;
        }
        myScript = (Soldier)target;
        if(myScript.morale.isAffectedRangeVisible == false)
        {
            return;
        }
        // 绘制您的Handles内容
        MoraleTemplate.DrawMoraleRange(myScript.transform.position, myScript.morale.affectedRange, myScript.morale.affectedRangeColor);
        // 刷新Scene视图
        sceneView.Repaint();
    }
    private  void OnSceneGUI()
    {
        if (target == null)
        {
            return;
        }
        myScript = (Soldier)target;
        MoraleTemplate.DrawMoraleRange(myScript.transform.position, myScript.morale.affectedRange, myScript.morale.affectedRangeColor);
    }
}
#endif