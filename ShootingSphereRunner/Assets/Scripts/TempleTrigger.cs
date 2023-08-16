using System;
using UnityEngine;

public class TempleTrigger : MonoBehaviour
{
    [SerializeField] private Animation doorAnimation;
    [SerializeField] private AnimationClip openDoorClip;
    
    public delegate void TempleTriggerActivate();
    public event TempleTriggerActivate OnTempleTriggerActivated;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Yep");
            doorAnimation.Play(openDoorClip.name);
            
            OnTempleTriggerActivated?.Invoke();
        }
    }
}
