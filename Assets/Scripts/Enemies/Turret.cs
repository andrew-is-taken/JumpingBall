using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
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
        laser.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!broken && readyToShoot)
        {
            RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
            float dist = Vector2.Distance(transform.position, player.position);
            playerInSight = !hit && dist > minDistance && dist < maxDistance;

            if (playerInSight)
            {
                lastCorotine = StartCoroutine(SpawnBullet());
                if (!laser.gameObject.activeSelf)
                    laser.gameObject.SetActive(true);
            }
            else
                if(lastCorotine != null)
                    StopCoroutine(lastCorotine);
        }

        if (aiming)
        {
            Gun.LookAt(player);
            laser.SetPosition(1, player.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            Death();
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
    private IEnumerator SpawnBullet()
    {
        readyToShoot = false;
        aiming = true;
        yield return new WaitForSeconds(timeToAim);
        aiming = false;
        if (!broken)
        {
            laser.gameObject.SetActive(false);
            var lastBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            lastBullet.GetComponent<Bullet>().speed = bulletSpeed;
            soundSource.PlayOneShot(shotSound);
        }
        yield return new WaitForSeconds(timeBetweenShots);
        if (!broken)
            readyToShoot = true;
    }
}
