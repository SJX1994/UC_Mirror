using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueSeat : MonoBehaviour
{
    public Vector3 Position
    {
        get{return position;}
    }
    private Vector3 position => transform.position;
    public int SeatIndex;
    
    void OnDrawGizmos()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Vector4( Color.blue.r, Color.blue.g, Color.blue.b,0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
    
}
