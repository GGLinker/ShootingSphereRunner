using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform bouncingBall;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    [SerializeField] private float scalingThreshold;
    [SerializeField] private float scaleSpeed;

    private bool bTouching;
    private Coroutine touchActionHandler;

    private void Update()
    {
        if (!bTouching && Input.GetKeyDown(KeyCode.Mouse0))
        {
            bTouching = true;
            touchActionHandler = StartCoroutine(TouchAction());
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
            bulletMovement.OnBulletCollision += MakeExplosion;

            while (bTouching)
            {
                float scaleDiff = bouncingBall.localScale.x - Mathf.Lerp(bouncingBall.localScale.x, 0, scaleSpeed * Time.deltaTime);
                var scaleDiffVector = new Vector3(scaleDiff, scaleDiff, scaleDiff);
                bouncingBall.localScale -= scaleDiffVector;
                bulletMovement.transform.localScale += scaleDiffVector;
                
                yield return null;
            }

            if (bouncingBall.localScale.x <= scalingThreshold)
            {
                GameOver();
                yield break;
            }
            
            bulletMovement.Fly();
        }
    }

    private void MakeExplosion(Vector3 bulletPosition)
    {
        
    }

    private void GameOver()
    {
        
    }
}
