
using UnityEngine;
using UnityEditor;
using UC_PlayerData;

[CreateAssetMenu(fileName = "Morale_", menuName = "士气", order = 1)]

public class MoraleTemplate : ScriptableObject{
      public bool successCreated = false;
      public Player player;
      public float morale;
      public float Morale
      {
            get
            {
                  return morale;
            }
            set
            {
                  morale = value;
                  
                  if(morale >= maxMorale)
                  {
                        morale = maxMorale;
                        if(player == Player.Player1 && successCreated)
                        {
                              UIData.Player1MoraleAccumulation += 1;
                              // Debug.Log("P1 士气值累计：" + UIData.Player1MoraleAccumulation);
                        }else if(player == Player.Player2 && successCreated)
                        {
                              UIData.Player2MoraleAccumulation += 1;
                              // Debug.Log("P2 士气值累计：" + UIData.Player2MoraleAccumulation);
                        }
                  }
                  if(morale <= minMorale)
                  {
                        morale = minMorale;
                  }
            }
      }
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
      private float previousValue;
      // 每个单位自身的基础属性都要乘以这个系数
      public void EffectByMorale( SoldierBehaviors soldier,ref float value )
      {
            value = soldier.maxStrength * soldier.morale.Morale;
      }
      // 基于心理学的 自我赋能感
      public void AddMorale(SoldierBehaviors soldier, float value, bool source = false)
      {
            soldier.morale.Morale += value;
            soldier.morale.Morale = soldier.morale.Morale>=soldier.morale.maxMorale?soldier.morale.maxMorale : soldier.morale.Morale;
            if(!source)return;
            SoldierBehaviors[] soldiers = FindObjectsOfType<SoldierBehaviors>();
            Transform circleCenter = soldier.transform;
            float circleRadius = soldier.morale.affectedRange;
            int soldiersInCircleCount = 0;
            foreach (SoldierBehaviors s in soldiers)
            {
                  if(s==soldier)continue;
                  // 获取单位的坐标
                  Vector3 sPosition = s.transform.position;
                  // 计算单位与圆心的距离
                  float distance = Vector3.Distance(sPosition, circleCenter.position);
                  // 检查距离是否小于或等于圆的半径
                  if (distance > circleRadius)return;
                  // 将距离映射到大小的范围
                  float size = 1/distance;
                  // Debug.Log("距离：" + distance + "，强度：" + size);
                  s.morale.AddMorale(s,value * size,false);
                  s.morale.EffectByMorale(s,ref s.strength);
                  soldiersInCircleCount++;
            }
            // Debug.Log("在半径内的单位数量：" + soldiersInCircleCount);
            
            
            
      }
      public void ReduceMorale(SoldierBehaviors soldier, float value, bool source = false)
      {
            soldier.morale.Morale -= value;
            soldier.morale.Morale = soldier.morale.Morale <= soldier.morale.minMorale? soldier.morale.minMorale : soldier.morale.Morale;
            if(!source)return;
            SoldierBehaviors[] soldiers = FindObjectsOfType<SoldierBehaviors>();
            Transform circleCenter = soldier.transform;
            float circleRadius = soldier.morale.affectedRange;
            int soldiersInCircleCount = 0;
            foreach (SoldierBehaviors s in soldiers)
            {
                  if(s==soldier)continue;
                  // 获取单位的坐标
                  Vector3 sPosition = s.transform.position;
                  // 计算单位与圆心的距离
                  float distance = Vector3.Distance(sPosition, circleCenter.position);
                  // 检查距离是否小于或等于圆的半径
                  if (distance > circleRadius)return;
                  // 将距离映射到大小的范围
                  float size = 1/distance;
                  // Debug.Log("距离：" + distance + "，强度：" + size);
                  s.morale.ReduceMorale(s,value * size,false);
                  s.morale.EffectByMorale(s,ref s.strength);
                  soldiersInCircleCount++;
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
                        #endif
                        Debug.DrawLine(prevPoint, currPoint, color, duration);
                        
                  }

                  prevPoint = currPoint;
            }
      }
      public static void DrawMoraleRange(Vector3 center, float radius, LineRenderer lineRenderer)
      {
            // #if UNITY_EDITOR
            //       Handles.color = color;
            //       Handles.DrawWireCube(center, Vector3.one * radius * 2);
            // #endif
            Vector3 prevPoint = Vector3.zero;
            float segmentAngle = 10; // 控制圆形的分段数
            lineRenderer.positionCount = (int)(360 + segmentAngle);
            for (float angle = 0; angle <= 360; angle += segmentAngle)
            {
                  float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + center.x;
                  float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + center.z;
                  Vector3 currPoint = new Vector3(x, center.y, z);

                  if (prevPoint != Vector3.zero)
                  {
                        lineRenderer.SetPosition((int)(angle / segmentAngle), currPoint);
                        // Debug.DrawLine(prevPoint, currPoint);
                  }

                  prevPoint = currPoint;
            }
      }
      public void ModifyBaseMinMorale(SoldierBehaviors soldier,float value)
      {
            soldier.morale.minMorale += value;
      }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SoldierBehaviors))]
public class SoldierEditor : Editor
{
    SoldierBehaviors myScript;
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
        myScript = (SoldierBehaviors)target;
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
        myScript = (SoldierBehaviors)target;
        MoraleTemplate.DrawMoraleRange(myScript.transform.position, myScript.morale.affectedRange, myScript.morale.affectedRangeColor);
    }
}
#endif