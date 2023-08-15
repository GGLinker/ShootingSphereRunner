using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallBounceMovement : MonoBehaviour
{
    [SerializeField] private Collider surfacePlaneCollider;
    [SerializeField] private float bounceIntensity;

    private Rigidbody ballRigidbody;

    private void Start()
    {
        ballRigidbody = transform.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == surfacePlaneCollider)
        {
            ballRigidbody.AddForce(Vector3.up * bounceIntensity);
        }
    }
}