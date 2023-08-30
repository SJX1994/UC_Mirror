using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class UnitGroup : Unit
{
      public int unitGroupID;
      private int numberOfCells = 1;
      public Vector3 targetPosition;
      public override void Start()
      {
            base.Start();
            numberOfCells = unitTemplate.health;
      }
      public override void Update()
      {
            base.Update();
      }

}