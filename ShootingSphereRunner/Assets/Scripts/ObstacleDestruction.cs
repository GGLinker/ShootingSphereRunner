using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ObstacleDestruction : MonoBehaviour
{
    [SerializeField] private AnimationClip destructionClip;

    private Animation obstacleAnimation;

    private void Start()
    {
        obstacleAnimation = transform.GetComponent<Animation>();
    }

    public void DestroyObstacle()
    {
        StartCoroutine(Destruction());
    }

    private IEnumerator Destruction()
    {
        obstacleAnimation.Play(destructionClip.name);

        yield return new WaitForSecondsRealtime(destructionClip.length);
        
        Destroy(gameObject);
    }
}
