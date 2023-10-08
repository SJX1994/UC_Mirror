
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
public class UnitSoul : Unit
{
      #region 数据对象
      public UnitSoulTemplate unitSoulTemplate; 
      public UnityAction<UnitSoul> OnCollection;
      private Vector3 position;
      MeshRenderer meshRenderer;
      public Slider soulSlider;
      private bool collected = false;
      [HideInInspector]
      public bool clicked = false;
      public TrailRenderer trail;
       
      #endregion 数据对象
      #region 数据关系
      override public void Start()
      {
            if(!trail)
            {
                  if(!transform.Find("Spine"))
                  {
                        if(!transform.Find("Spine").transform.Find("Tire"))
                        {
                              transform.Find("Spine").transform.Find("Tire").TryGetComponent<TrailRenderer>(out TrailRenderer trail);
                        }
                  }
                  
            }
            if(!soulSlider)
            {
                  soulSlider = GameObject.Find("UI_SoulSlider_Main").GetComponent<Slider>();
            }
            if (TryGetComponent<Effect>(out Effect m_effect))
            {
                  effect = m_effect;
                  RunEffect(EffectTemplate.EffectType.Start);
            }
            unitSoulTemplate = Instantiate<UnitSoulTemplate>(unitSoulTemplate);
            unitSoul = this;
            position = transform.position;
            meshRenderer = animator.GetComponent<MeshRenderer>();
            Color m_color = meshRenderer.sharedMaterial.color;
            meshRenderer.sharedMaterial.color = new Color(m_color.r,m_color.g,m_color.b,1);
            InvokeRepeating("Wander", 0, 2);
            StartCoroutine(Dissipate(1,0,unitSoulTemplate.duration+ Random.Range(-2,2)));
            // 替代通讯
            if(GameObject.Find("CanvasManager_StayMachine(Clone)"))
            {
                  stateMachineManager = GameObject.Find("CanvasManager_StayMachine(Clone)").GetComponent<StateMachineManager>();
            }
             if(GameObject.Find("CanvasManager_StayMachine"))
            {
                  stateMachineManager = GameObject.Find("CanvasManager_StayMachine").GetComponent<StateMachineManager>();
            }
          
      }
      
      /// <summary>
      /// 鬼魂没有攻击行为
      /// </summary>
      /// <returns></returns>
      public override IEnumerator DealAttack()
      {
            return null;
      }
      /// <summary>
      /// 鬼魂死亡 = 被收集
      /// </summary>
      public override void Die(float destoryTime)
      {
          base.Die(unitTemplate.destoryTime);
          
      }
      /// <summary>
      /// TODO 重写收集时的动画表现
      /// </summary>
      /// <param name="v_start"></param>
      /// <param name="v_end"></param>
      /// <param name="duration"></param>
      /// <returns></returns>
      protected override IEnumerator DieDispaly(float v_start, float v_end, float duration)
      {
            float elapsed = 0.0f;
		float value = v_start;
		while (elapsed < duration )
		{
			value = Mathf.Lerp( v_start, v_end, elapsed / duration );
			elapsed += Time.deltaTime;
			animator.transform.localScale =new Vector3(value,value,value);
		yield return null;
		}
		value = v_end;
      }
      /// <summary>
      /// 鬼魂死亡不会生成鬼魂
      /// </summary>
      public override void CreatSoul()
      {
            // 可能有些特效
      }
      /// <summary>
      /// 可视化 Debug
      /// </summary>
      void OnDrawGizmos()
      {
            Gizmos.color = new Vector4(Color.blue.r,Color.blue.g,Color.blue.b,0.2f) ;
            if (Application.isPlaying)
            {
                  Gizmos.DrawSphere(position, unitSoulTemplate.wanderDistance);
            }else
            {
                  Gizmos.DrawSphere(transform.position, unitSoulTemplate.wanderDistance);
            }
           
            
      }
      #endregion 数据关系
      #region 数据操作
      /// <summary>
      /// 鬼魂消散
      /// </summary>
      IEnumerator Dissipate(float v_start, float v_end, float duration)
      {
            gameObject.tag = unitSoulTemplate.unitType.ToString();
            float elapsed = 0.0f;
            float value = v_start;
            Color m_color = meshRenderer.sharedMaterial.color;
            Color m_colorTrail = Color.black;
            if(trail)
            {
                  m_colorTrail = trail.startColor;
            }
            
            
            Destroy(gameObject,duration+0.5f);
            while (elapsed < duration )
            {
                  value = Mathf.Lerp( v_start, v_end, elapsed / duration );
                  elapsed += Time.deltaTime;
                  meshRenderer.sharedMaterial.color = new Color(m_color.r,m_color.g,m_color.b,value);
                  if(trail)
                  {     
                        trail.startColor = new Color(m_colorTrail.r,m_colorTrail.g,m_colorTrail.b,value);
                  }
                  yield return null;
            }
            
      }
      /// <summary>
      /// 鬼魂活动范围示意
      /// </summary>
      /// <param name="selected"></param>
      protected override void SetSelected(bool selected)
      {
            if(selectionCircle)
		{
                  Color newColor = selectionCircle.color;
                  newColor.a = (selected) ? .3f : .1f;
                  selectionCircle.color = newColor;
                  selectionCircle.gameObject.transform.position = position;
            }
      }
      /// <summary>
      /// 鬼魂游荡
      /// </summary>
      void Wander()
      {
            Vector3 targetPos =  new Vector3(position.x+ Random.insideUnitCircle.x * unitSoulTemplate.wanderDistance,transform.position.y,position.z+ Random.insideUnitCircle.y * unitSoulTemplate.wanderDistance);
		AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndIdle,targetPos);
        	this.ExecuteCommand(newCommand);
      }
      /// <summary>
      /// 鬼魂被收集
      /// </summary>
      void BeenCollected()
      { 
            
            // 收集时 点燃订阅者的事件 以传递信息
            if(OnCollection != null)
            {
                  OnCollection(this);
            }
            
      }
      public IEnumerator CollectingDispaly(Vector2 v_start, Vector2 v_end, float duration,UnitFinder unitFinder)
      {
            float elapsed = 0.0f;
            Vector2 value = v_start;
            while (elapsed < duration )
            {
                  value.x = Mathf.Lerp( v_start.x, v_end.x, elapsed / duration );
                  value.y = Mathf.Lerp( v_start.y, v_end.y, elapsed / duration );
                  elapsed += Time.deltaTime;
                  // TODO 判空
                  transform.position = new Vector3(value.x,transform.position.y,value.y);
                  yield return null;
            }
            BeenCollected();
            Die(unitTemplate.destoryTime);
            state = UnitState.Dead;
      }
      public IEnumerator CollectingDispaly(Vector2 v_start, Vector2 v_end, float duration,GameObject obj)
      {
            
            if(!obj)yield return 0;
            float elapsed = 0.0f;
            Vector2 value = v_start;
            while (elapsed < duration )
            {
                  value.x = Mathf.Lerp( v_start.x, v_end.x, elapsed / duration );
                  value.y = Mathf.Lerp( v_start.y, v_end.y, elapsed / duration );
                  elapsed += Time.deltaTime;
                  // TODO 判空
                  transform.position = new Vector3(value.x,transform.position.y,value.y);
                  yield return null;
            }
            if(!collected)
            {
                  if(unitSoulTemplate.unitType == UnitSoulTemplate.UnitSoulType.Soul_Cell)
                  {
                        soulSlider.value += unitSoulTemplate.value;
                        stateMachineManager.soulTextUI.text = (int.Parse(stateMachineManager.soulTextUI.text)+unitSoulTemplate.value).ToString();
                        string name = SkeletonRenderer.Skeleton.Skin.Name;
                        switch(name)
                        {
                              case "blue":
                                    soulSlider.fillRect.GetComponent<Image>().color = CombineColors(soulSlider.fillRect.GetComponent<Image>().color,Color.blue);
                              break;
                              case "green":
                                    soulSlider.fillRect.GetComponent<Image>().color = CombineColors(soulSlider.fillRect.GetComponent<Image>().color,Color.green);
                              break;
                              case "red":
                                    soulSlider.fillRect.GetComponent<Image>().color = CombineColors(soulSlider.fillRect.GetComponent<Image>().color,Color.red);
                              break;
                              case "purple":
                                    soulSlider.fillRect.GetComponent<Image>().color = CombineColors(soulSlider.fillRect.GetComponent<Image>().color,new Color(0.5f,0,0.5f));
                              break;
                                    
                        }
                        collected = true;
                  }
                  
            }
            
            BeenCollected();
            if(obj)
            {
               Die(duration);
               Destroy(obj);
            }
            
            
            // Die(unitTemplate.destoryTime);
            // state = UnitState.Dead;
      }
      public static Color CombineColors(params Color[] aColors)
      {
            Color result = new Color(0,0,0,0);
            foreach(Color c in aColors)
            {
                  result += c;
            }
            result /= aColors.Length;
            float H, S, V;
            Color.RGBToHSV(result, out H, out S, out V);
            V = 1;
            result = Color.HSVToRGB(H, S, V);
            result.a = 1;
            
            return result;
      }
      #endregion 数据操作
}
