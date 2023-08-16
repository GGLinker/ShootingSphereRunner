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

    [SerializeField] private float ballInitialScale;
    
    [SerializeField] private float bulletInitialScale;
    [SerializeField] private float scalingThreshold;
    [SerializeField] private float scaleSpeed;

    [SerializeField] private TextMeshProUGUI goalLabel;
    [SerializeField] private Animation goalLabelAnimation;
    [SerializeField] private AnimationClip goalClip;

    private bool bTouching;
    private bool bInputAllowed = true;

    private void Start()
    {
        bouncingBall.localScale = new Vector3(ballInitialScale, ballInitialScale, ballInitialScale);
        roadPlane.localScale = new Vector3(bouncingBall.localScale.x / 10, roadPlane.localScale.y, roadPlane.localScale.z);
        
        handlerMovement.OnMovementTargetReached += () =>
        {
            if (Vector3.Distance(bulletSpawn.position, temple.position) < spawner.templeFreeDistance)
            {
                handlerMovement.MoveTo( Vector3.forward * temple.position.z);
                return;
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
        BulletMovement bulletMovement = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity).transform.GetComponent<BulletMovement>();
        if (bulletMovement)
        {
            bulletMovement.transform.localScale = Vector3.zero;
            bulletMovement.OnBulletCollision += (bullet, surroundObstacles) =>
            {
                StartCoroutine(BulletCollisionProcess(bullet, surroundObstacles));
            };

            {
                var initialScaleVector = new Vector3(bulletInitialScale, bulletInitialScale, bulletInitialScale);
                DistractScale(initialScaleVector);
                bulletMovement.AddScale(initialScaleVector);
            }


            while (bTouching)
            {
                float scaleDiff = bouncingBall.localScale.x - Mathf.Lerp(bouncingBall.localScale.x, 0, scaleSpeed * Time.deltaTime);
                var scaleDiffVector = new Vector3(scaleDiff, scaleDiff, scaleDiff);
                DistractScale(scaleDiffVector);
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
    private void DistractScale(Vector3 scaleVector)
    {
        bouncingBall.localScale -= scaleVector;
        roadPlane.localScale = new Vector3(bouncingBall.localScale.z / 10, roadPlane.localScale.y, roadPlane.localScale.z);
    }

    private IEnumerator BulletCollisionProcess(GameObject bullet, List<ObstacleDestruction> surroundObstacles)
    {
        MakeExplosion(bullet, surroundObstacles);
        if (surroundObstacles.Count > 0)
        {
            yield return new WaitForSecondsRealtime(surroundObstacles[0].destructionClip.length + .1f);
        }
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

        float closestPosition = float.MaxValue;

        float roadPlaneWidth = roadPlane.localScale.x * 10;
        float shiftFraction = roadPlaneWidth / (obstacleDetectionAccuracy - 1);

        bool bObstaclesFound = false;
        for (int i = 0; i < obstacleDetectionAccuracy; i++)
        {
            if (Physics.Raycast(
                    new Vector3(shiftFraction * i - roadPlaneWidth / 2, 1f, bulletSpawn.position.z),
                    Vector3.forward,
                    out hit,
                    spawner.DistanceToTemple))
            {
                if (hit.collider.gameObject.CompareTag("Obstacle")) {
                    bObstaclesFound = true;
                    if (hit.transform.position.z < closestPosition)
                    {
                        closestPosition = hit.transform.position.z;
                    }
                }
            }
        }

        handlerMovement.MoveTo(Vector3.forward * (bObstaclesFound ? 
            (closestPosition - (bulletSpawn.localPosition.z + spawner.minSpawnDistance)) 
            : spawner.DistanceToTemple));
    }

    private IEnumerator ReachGoal(bool bWin)
    {
        goalLabel.SetText(bWin ? "Victory!" : "Game Over");
        goalLabelAnimation.Play(goalClip.name);
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene("Level");
    }
}
