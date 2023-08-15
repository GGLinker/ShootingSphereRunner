using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallHandlerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody handlerRigidbody;
    private Vector3 targetVector;
    private Coroutine movementCoroutineHandler;

    private void Start()
    {
        handlerRigidbody = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MoveTo(transform.position + new Vector3(0, 0, 3));
        }*/
    }

    public void MoveTo(Vector3 target)
    {
        if (movementCoroutineHandler != null)
        {
            StopCoroutine(movementCoroutineHandler);
        }
        targetVector = target;
        movementCoroutineHandler = StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        handlerRigidbody.velocity = Vector3.forward * movementSpeed;
        while (Vector3.Distance(transform.position, targetVector) > .5f)
        {
            yield return null;
        }
        handlerRigidbody.velocity = Vector3.zero;
    }
}
