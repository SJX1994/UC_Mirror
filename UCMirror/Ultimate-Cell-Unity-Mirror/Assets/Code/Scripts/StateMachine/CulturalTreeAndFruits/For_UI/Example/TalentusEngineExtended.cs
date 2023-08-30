using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cygnusprojects.TalentTree;
using UnityEngine.UI;
using System.IO;

namespace com.cygnusprojects.TalentTree
{
    public class TalentusEngineExtended : TalentusEngine
    {
        #region 数据对象
        public Text AvailableSkillPointsUI;
        // public string Filename = "Assets/Talentus/Example/Resources/SavedTalentTree.TT";
        public string Filename = "Assets/Code/Scripts/StateMachine/CulturalTreeAndFruits/For_UI/Example/Resources/SavedTalentTree.TT";
        #endregion

        #region 数据关系
        // Use this for initialization
        public override void Start()
        {
            base.Start();
            // Your code here    
        }

        private void OnEnable()
        {            
            // TalentBought is triggered when the talents get applied
            TalentTree.TalentBought += TalentTree_TalentBought;
            // TalentSelected is triggered when you buy a talent but you still did not applied it (so it still can be reverted)
            TalentTree.TalentSelected += TalentTree_TalentSelected;
            // TalentReverted is triggered when you did buy a talent and does revert it back.
            TalentTree.TalentReverted += TalentTree_TalentReverted;
        }

        private void OnDisable()
        {
            // Clear the event listening
            TalentTree.TalentBought -= TalentTree_TalentBought;
            TalentTree.TalentSelected -= TalentTree_TalentSelected;            
            TalentTree.TalentReverted -= TalentTree_TalentReverted;
        }
        /// <summary>
        /// 回退购买的经验值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalentTree_TalentReverted(object sender, TalentTreeGraph.TalentRevertedEventArgs e)
        {
            Debug.LogWarning(string.Format("Talent {0} reverted, added {1} back to the available skill points.", e.Talent.Name, e.Cost.Cost));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalentTree_TalentSelected(object sender, TalentTreeGraph.TalentSelectedEventArgs e)
        {
            Debug.LogWarning(string.Format("Talent {0} selected.", e.Talent.Name));
        }

        private void TalentTree_TalentBought(object sender, TalentTreeGraph.TalentBoughtEventArgs e)
        {
            Debug.LogWarning(string.Format("Talent {0} bought.", e.Talent.Name));
        }

        /// <summary>
        /// Unity update event
        /// In this example used to update the available skill points within the UI 
        /// Pressing S will also save the current state of the tree to disk.
        /// Pressing L will load the state of the tree from disk.
        /// </summary>
        void Update()
        {
            if (AvailableSkillPointsUI != null)
                AvailableSkillPointsUI.text = TalentTree.PointsToAssign.ToString().Trim();           
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveGraph();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadGraph();
            }
            if (Input.GetKeyDown(KeyCode.R)) // Reset the tree to it's initial values
            {
                foreach (TalentTreeNodeBase talent in TalentTree.talents)
                {
                    talent.Reset(); 
                }
                // Don't forget to reevaluate the tree after you changed some data:
                TalentTree.Evaluate();

                /* Or you can just reset the Bought flag of the Talent
                foreach (TalentTreeNodeBase talent in TalentTree.talents)
                {
                    foreach (TalentTreeCost cost in talent.Cost)
                    {
                        cost.Bought = false;
                    }
                }
                TalentTree.Evaluate();*/
            }
        }
        #endregion

        #region 数据方法
        public void SaveGraph()
        {
            string path = Filename;
            //Write the graph to a file
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(SaveToString());
            writer.Close();

            // Player prefs
            //PlayerPrefs.SetString("MyTree", SaveToString());
        }

        public void LoadGraph()
        {
            string path = Filename;
            StreamReader reader = new StreamReader(path);
            string statuses = reader.ReadToEnd();
            reader.Close();
            LoadFromString(statuses);

            // Player prefs
            //string importedData = PlayerPrefs.GetString("MyTree");
            //LoadFromString(importedData);
        }

        #endregion

    }
}
