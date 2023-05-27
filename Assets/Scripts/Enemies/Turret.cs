using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Shot")]
    [SerializeField] private bool locatedOnHorizontal;
    [SerializeField] private bool canShootInBothDirections;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private AudioClip shotSound;
    [Tooltip("Minimal distance to player, recommended > 3")]
    [SerializeField] private float minDistance = 5f;

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
    private bool readyToShoot;
    private bool aiming;
    private bool playerInSight;
    private AudioSource soundSource;
    private Coroutine lastCorotine;

    private void Start()
    {
        broken = false;
        readyToShoot = true;
        player = MovementManager.instance.transform;
        soundSource = GetComponent<AudioSource>();
        laser.SetPosition(0, laserStart.position);
    }

    private void Update()
    {
        if (!broken && readyToShoot)
        {
            RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
            playerInSight = !hit && Vector2.Distance(transform.position, player.position) > minDistance;

            if (!laser.gameObject.activeSelf)
                laser.gameObject.SetActive(true);

            if (playerInSight)
                lastCorotine = StartCoroutine(SpawnBullet());
            else
                if(lastCorotine != null)
                    StopCoroutine(lastCorotine);
        }

        if(aiming)
            laser.SetPosition(1, player.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            Death();
    }

    public void Death()
    {
        broken = true;
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
        soundSource.PlayOneShot(deathSound);
        sparks.Play();
    }

    private IEnumerator SpawnBullet()
    {
        readyToShoot = false;
        aiming = true;
        yield return new WaitForSeconds(timeToAim);
        aiming = false;
        laser.gameObject.SetActive(false);
        bulletSpawn.LookAt(player.position);

        if (locatedOnHorizontal)
        {
            if (canShootInBothDirections)
            {
                
            }
        }
        // shoot
        var lastBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        lastBullet.GetComponent<Bullet>().speed = bulletSpeed;
        soundSource.PlayOneShot(shotSound);
        yield return new WaitForSeconds(timeBetweenShots);
        if(!broken)
            readyToShoot = true;
    }
}
