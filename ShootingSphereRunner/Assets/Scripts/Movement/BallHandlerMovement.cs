using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallHandlerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody handlerRigidbody;
    private Vector3 targetVector;
    private Coroutine movementCoroutineHandler;
    
    public delegate void MovementTargetReached();
    public event MovementTargetReached OnMovementTargetReached;

    private void Start()
    {
        handlerRigidbody = transform.GetComponent<Rigidbody>();
    }

    public void MoveTo(Vector3 target)
    {
        if (movementCoroutineHandler != null)
        {
            StopCoroutine(movementCoroutineHandler);
        }
        targetVector = target;
        Debug.Log("Target: " + target);
        movementCoroutineHandler = StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        handlerRigidbody.velocity = Vector3.forward * movementSpeed;
        while (Vector3.Distance(transform.position, targetVector) > .1f)
        {
            yield return null;
        }
        handlerRigidbody.velocity = Vector3.zero;
        OnMovementTargetReached?.Invoke();
    }
}
