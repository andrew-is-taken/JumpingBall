using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private List<GameObject> bulletPool;

    [SerializeField] private Sprite brokenSprite;

    [Header("Time")]
    [SerializeField] private float timeToAim;
    [SerializeField] private float timeBetweenShots;

    [Header("Laser")]
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform laserStart;

    private Transform player;
    private bool broken;
    private bool readyToShoot;
    private bool playerInSight;
    private Coroutine lastCorotine;

    void Start()
    {
        broken = false;
        readyToShoot = true;
        player = MovementManager.instance.transform;
        laser.SetPosition(0, laserStart.position);
    }

    void Update()
    {
        if (!broken && readyToShoot)
        {
            RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
            playerInSight = !hit;

            if (!laser.gameObject.activeSelf)
                laser.gameObject.SetActive(true);

            laser.SetPosition(1, player.position);

            if (playerInSight)
                lastCorotine = StartCoroutine(SpawnBullet());
            else
                StopCoroutine(lastCorotine);
        }
    }

    public void Death()
    {
        broken = true;
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
    }

    private IEnumerator SpawnBullet()
    {
        yield return new WaitForSeconds(timeToAim);
        readyToShoot = false;
        laser.gameObject.SetActive(false);
        // shoot
        yield return new WaitForSeconds(timeBetweenShots);
        if(!broken)
            readyToShoot = true;
    }
}
