using System.Collections;
using System.Collections.Generic;
using SBS.ME;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private MeshExploder meshExploder;
    [SerializeField] private ObstGenerator generator; // Reference to the generator

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            meshExploder.explodeNOW = true;
            CinemachineShake.Instance.ShakeCamera(2, 0.5f);
            DestroyObstacle();
        }
    }

    public void SetGenerator(ObstGenerator gen)
    {
        generator = gen;
    }

    void DestroyObstacle()
    {
        if (generator != null)
        {
            generator.ObstacleDestroyed(); // Notify the generator
            gameObject.tag = "Untagged";
        }
        
    }
    
}
