using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ObstaclesSpawner spawner;
    [SerializeField] private BallHandlerMovement handlerMovement;
    
    [SerializeField] private Transform bouncingBall;
    [SerializeField] private Transform roadPlane;
    
    [SerializeField] private Transform temple;
    [SerializeField] private TempleTrigger templeTrigger;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [SerializeField, Range(2, 10)] private int obstacleDetectionAccuracy;
    
    [SerializeField] private float scalingThreshold;
    [SerializeField] private float scaleSpeed;

    [SerializeField] private TextMeshProUGUI goalLabel;
    [SerializeField] private Animation goalLabelAnimation;
    [SerializeField] private AnimationClip goalClip;

    private bool bTouching;
    private bool bInputAllowed = true;

    private void Start()
    {
        handlerMovement.OnMovementTargetReached += () =>
        {
            if (Vector3.Distance(bulletSpawn.position, temple.position) < spawner.templeFreeDistance)
            {
                handlerMovement.MoveTo( Vector3.forward * temple.position.z);
            }
            
            bInputAllowed = true;
        };
        templeTrigger.OnTempleTriggerActivated += () =>
        {
            StartCoroutine(ReachGoal(true));
        };
    }

    private void Update()
    {
        if (bInputAllowed && !bTouching && Input.GetKeyDown(KeyCode.Mouse0))
        {
            bTouching = true;
            StartCoroutine(TouchAction());
        }

        if (bTouching && Input.GetKeyUp(KeyCode.Mouse0))
        {
            bTouching = false;
        }
    }

    private IEnumerator TouchAction()
    {
        BulletMovement bulletMovement = Instantiate(bulletPrefab, bulletSpawn).transform.GetComponent<BulletMovement>();
        if (bulletMovement)
        {
            bulletMovement.transform.localScale = new Vector3(scalingThreshold, scalingThreshold, scalingThreshold);
            bulletMovement.OnBulletCollision += (GameObject bullet, List<ObstacleDestruction> surroundObstacles) =>
            {
                StartCoroutine(BulletCollisionProcess(bullet, surroundObstacles));
            };

            while (bTouching)
            {
                float scaleDiff = bouncingBall.localScale.x - Mathf.Lerp(bouncingBall.localScale.x, 0, scaleSpeed * Time.deltaTime);
                var scaleDiffVector = new Vector3(scaleDiff, scaleDiff, scaleDiff);
                bouncingBall.localScale -= scaleDiffVector;
                roadPlane.localScale = new Vector3(bouncingBall.localScale.z / 10, roadPlane.localScale.y, roadPlane.localScale.z);
                bulletMovement.AddScale(scaleDiffVector);

                yield return null;
            }

            bInputAllowed = false;

            if (bouncingBall.localScale.x <= scalingThreshold)
            {
                StartCoroutine(ReachGoal(false));
                yield break;
            }
            
            bulletMovement.Fly();
        }
    }

    private IEnumerator BulletCollisionProcess(GameObject bullet, List<ObstacleDestruction> surroundObstacles)
    {
        MakeExplosion(bullet, surroundObstacles);
        yield return new WaitForSecondsRealtime(surroundObstacles[0].destructionClip.length + .1f);
        MoveCloserToObstacles();
    }

    private void MakeExplosion(GameObject bullet, List<ObstacleDestruction> surroundObstacles)
    {
        foreach (var obstacle in surroundObstacles)
        {
            obstacle.DestroyObstacle();
        }
        Destroy(bullet);
    }

    private void MoveCloserToObstacles()
    {
        RaycastHit hit;
        
        float closestPosition = spawner.DistanceToTemple;
        
        float roadPlaneWidth = roadPlane.localScale.x * 10;
        float shiftFraction = roadPlaneWidth / (obstacleDetectionAccuracy - 1);
        
        for (int i = 0; i < obstacleDetectionAccuracy; i++)
        {
            if (Physics.Raycast(
                    new Vector3(shiftFraction * i - roadPlaneWidth / 2, 1f, bulletSpawn.position.z),
                    Vector3.forward, 
                    out hit, 
                    spawner.DistanceToTemple))
            {
                //Debug.Log(i + ":// " + -roadPlaneWidth / 2 + "; "  + shiftFraction * i + " = " + (shiftFraction * i - roadPlaneWidth / 2));
                if (hit.collider.gameObject.CompareTag("Obstacle") && hit.transform.position.z < closestPosition)
                {
                    closestPosition = hit.transform.position.z;
                }
            }
        }
        handlerMovement.MoveTo( Vector3.forward * (closestPosition - (bulletSpawn.localPosition.z + spawner.minSpawnDistance)));
    }

    private IEnumerator ReachGoal(bool bWin)
    {
        goalLabel.SetText(bWin ? "Victory!" : "Game Over");
        goalLabelAnimation.Play(goalClip.name);
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene("Level");
    }
}
