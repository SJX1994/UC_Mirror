using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace com.cygnusprojects.TalentTree
{
	public static class TalentTreeConnectionUtils  
	{
        #region Variables
        #endregion

        #region Implementation
#if UNITY_EDITOR
        public static TalentTreeConnection CreateConnection(TalentTreeGraph curGraph, TalentTreeNodeBase fromNode, TalentTreeNodeBase toNode)
        {
            if (curGraph != null)
            {
                TalentTreeConnection connection = (TalentTreeConnection)ScriptableObject.CreateInstance<TalentTreeConnection>();
                if (connection != null)
                {
                    connection.Init(curGraph, fromNode, toNode);
                    connection.isSelected = true;
                    curGraph.connections.Add(connection);

                    AssetDatabase.AddObjectToAsset(connection, curGraph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return connection;
                }
                else return null;
            }
            else
                return null;
        }

        public static void DeleteConnection(TalentTreeGraph curTree, TalentTreeConnectionBase connection)
        {
            if (curTree != null && connection != null)
            {
                connection.DeleteAllCondItems(curTree, connection);
                curTree.connections.Remove(connection);
                GameObject.DestroyImmediate(connection, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void DeleteConnectionsForNode(TalentTreeGraph curTree, int nodeID)
        {
            if (curTree != null)
            {
                if (curTree.talents.Count >= nodeID)
                {
                    TalentTreeNodeBase deleteNode = curTree.talents[nodeID];
                    if (deleteNode != null)
                    {
                        if (curTree.connections.Count > 0)
                        {
                            int i = curTree.connections.Count - 1;
                            while (i >= 0)
                            {
                                TalentTreeConnectionBase connection = curTree.connections[i];
                                if (connection != null)
                                {
                                    if (connection.fromNode == deleteNode || connection.toNode == deleteNode)
                                    {
                                        curTree.connections.RemoveAt(i);
                                        GameObject.DestroyImmediate(connection, true);
                                    }
                                }
                                i--;
                            }
                        }
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
#endif
        #endregion
    }
}
