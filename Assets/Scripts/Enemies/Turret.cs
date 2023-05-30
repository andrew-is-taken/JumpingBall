using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, ILevelObject
{
    [Header("Shot")]
    [SerializeField] private Transform Gun;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private AudioClip shotSound;
    [Tooltip("Minimal distance to be able to shoot player, recommended > 3")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("Maximal distance to see player")]
    [SerializeField] private float maxDistance = 20f;

    [Header("Time")]
    [SerializeField] private float timeToAim;
    [SerializeField] private float timeBetweenShots;

    [Header("Death")]
    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private AudioClip deathSound;

    [Header("Laser")]
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform laserStart;

    private Transform player;

    private bool broken;
    public bool readyToShoot;
    public bool aiming;
    public bool playerInSight;

    private AudioSource soundSource;
    public List<GameObject> bulletsPool;
    private Coroutine lastCorotine;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        laser.SetPosition(0, laserStart.position);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        broken = false;
        readyToShoot = true;
        laser.gameObject.SetActive(false);
        foreach(GameObject bullet in bulletsPool)
            Destroy(bullet);
        bulletsPool.Clear();
    }

    private void Update()
    {
        if (!broken)
        {
            RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
            float dist = Vector2.Distance(transform.position, player.position);
            playerInSight = !hit && dist > minDistance && dist < maxDistance;

            if (playerInSight && readyToShoot)
                lastCorotine = StartCoroutine(StartAiming());
            //else
            //{
            //    if (lastCorotine != null)
            //        StopCoroutine(lastCorotine);
            //}

            if (aiming)
            {
                Gun.LookAt(player);
                laser.SetPosition(1, player.position);
                if (!playerInSight)
                    StopAiming();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            Death();
    }

    /// <summary>
    /// Sets default state of the turret when player isn't in sight.
    /// </summary>
    private void StopAiming()
    {
        readyToShoot = true;
        aiming = false;
        if (laser.gameObject.activeSelf)
            laser.gameObject.SetActive(false);
        if (lastCorotine != null)
            StopCoroutine(lastCorotine);
    }

    /// <summary>
    /// Disables the ability to shoot and stops shooting if turret is aiming.
    /// </summary>
    public void Death()
    {
        broken = true;
        aiming = false;
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
        laser.gameObject.SetActive(false);
        soundSource.PlayOneShot(deathSound);
        sparks.Play();
    }

    /// <summary>
    /// Spawns bullet in the player's direction.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartAiming()
    {
        readyToShoot = false;
        aiming = true;
        if (!laser.gameObject.activeSelf)
            laser.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeToAim);
        aiming = false;
        if (!broken)
        {
            laser.gameObject.SetActive(false);
            var lastBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            lastBullet.GetComponent<Bullet>().speed = bulletSpeed;
            bulletsPool.Add(lastBullet);
            soundSource.PlayOneShot(shotSound);
        }
        yield return new WaitForSeconds(timeBetweenShots);
        if (!broken)
            readyToShoot = true;
    }

    public void restartObject()
    {
        enabled = true;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void turnOffObject()
    {
        enabled = false;
    }
}
