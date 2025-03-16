using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndWall : MonoBehaviour
{
    public Collider colliderToEnable; // Assign the collider to enable in the Inspector

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car")) // Check if exited object has tag "EndWall"
        {
            colliderToEnable.enabled = true;
        }
    }

    public void RestartColliders()
    {
        colliderToEnable.enabled = false;
    }
}
