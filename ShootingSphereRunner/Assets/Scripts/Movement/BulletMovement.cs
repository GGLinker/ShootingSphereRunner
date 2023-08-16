using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletMovement : MonoBehaviour
{
    [SerializeField] private float flySpeed;
    [SerializeField] private float triggerExpansionCoefficient;
    [SerializeField] private SphereCollider triggerSphere;

    public delegate void BulletCollision(GameObject self, List<ObstacleDestruction> surroundObstacles);
    public event BulletCollision OnBulletCollision;

    private Rigidbody bulletRigidbody;
    private List<ObstacleDestruction> surroundObstacles;

    private void Start()
    {
        surroundObstacles = new List<ObstacleDestruction>();
        bulletRigidbody = transform.GetComponent<Rigidbody>();
    }

    public void AddScale(Vector3 value)
    {
        transform.localScale += value;
        triggerSphere.radius += value.x * triggerExpansionCoefficient;
    }
    public void Fly()
    {
        bulletRigidbody.velocity = Vector3.forward * flySpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            OnBulletCollision?.Invoke(gameObject, surroundObstacles);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            surroundObstacles.Add(other.transform.GetComponent<ObstacleDestruction>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            surroundObstacles.Remove(other.transform.GetComponent<ObstacleDestruction>());
        }
    }
}