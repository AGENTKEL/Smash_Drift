using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private Collider roadCollider;
    public Vector3 RoadSize { get; private set; }

    private void Awake()
    {
        roadCollider = GetComponent<Collider>();
        if (roadCollider != null)
        {
            RoadSize = roadCollider.bounds.size;
        }
        else
        {
            Debug.LogError("No Collider found on the Road prefab!");
        }
    }
}
