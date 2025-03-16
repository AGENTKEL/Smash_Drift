using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform arrowTransform; // Reference to the arrow UI
    public string obstacleTag = "Target"; // Tag for spawned obstacles

    void Update()
    {
        Transform nearestObstacle = FindNearestObstacle();
        if (nearestObstacle != null)
        {
            Vector3 direction = nearestObstacle.position - arrowTransform.position;
            direction.y = 0; // Ignore height difference

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Set only the Y-axis dynamically while keeping X at 90° and Z at 0°
            arrowTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }

    Transform FindNearestObstacle()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obstacle in obstacles)
        {
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obstacle.transform;
            }
        }

        return nearest;
    }
}
