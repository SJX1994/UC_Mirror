using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class BuildingSlot : MonoBehaviour
{
    
    public Vector3 slotPos;
    public Building theLastStayBuilding;
    UnityAction<Building> OnBuildingStateChanged;
    private Building building;
    [HideInInspector]
    public BlockDisplay blockDisplay;
    BuildingManager buildingManager;
    public Building Building
    {
          get { return building; }
          set
          {
                if (value != building)
                {
                    building = value;
                    BuildingChanged(building);
                    
                    
                }
          }
    }

      private void BuildingChanged(Building building)
      {
            
            // blockDisplay.Bright(this,false);
            
            if(OnBuildingStateChanged!=null)
            {
                OnBuildingStateChanged(building);
            }
      }

      // Start is called before the first frame update
    void Start()
    {
        blockDisplay = transform.GetComponent<BlockDisplay>();
        building = null;
        slotPos = Vector3.zero;
        Invoke(nameof(LateStart),0.1f);
    }
    void LateStart()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        foreach (var building in buildingManager.buildings)
        {
            building.OnTheLastStayBuilding += OnTheLastStayBuilding;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTheLastStayBuilding(Building build,BuildingSlot slot)
    {
        if(slot == this)
        {
            
            BuildingSlot[] slots = FindObjectsOfType<BuildingSlot>().Where(slot => slot.theLastStayBuilding == build).ToArray();
            foreach (var s in slots)
            {
                // s.blockDisplay.NotBright(s);
            }
            theLastStayBuilding = build;
            // blockDisplay.Bright(this,true);
        }
    }
}
