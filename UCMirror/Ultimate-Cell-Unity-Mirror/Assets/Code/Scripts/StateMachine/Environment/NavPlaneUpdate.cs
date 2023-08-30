using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavPlaneUpdate : MonoBehaviour
{
    private NavMeshSurface m_surface;
    // Start is called before the first frame update
    void Start()
    {
        m_surface = GetComponent<NavMeshSurface>();
        InvokeRepeating("UpdateNavMesh", 0, 0.5f);
    }

   
    void UpdateNavMesh()
    {
        m_surface.BuildNavMesh();
    }
}
