using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletMovement : MonoBehaviour
{
    [SerializeField] private float flySpeed;

    public delegate void BulletCollision(BulletMovement self);
    public event BulletCollision OnBulletCollision;

    private Rigidbody bulletRigidbody;

    private void Start()
    {
        bulletRigidbody = transform.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = Vector3.forward * flySpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            OnBulletCollision?.Invoke(this);
        }
    }
}