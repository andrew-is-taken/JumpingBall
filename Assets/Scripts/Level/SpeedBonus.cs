using System.Collections;
using UnityEngine;

public class SpeedBonus : MonoBehaviour
{
    [SerializeField] private bool emitTrails = false; // if trails spawn after the player picks up bonus
    [SerializeField] private float speedBonus = -1f; // bonus to player's speed

    [SerializeField] private GameObject InnerPart; // inner part of the bonus
    [SerializeField] private GameObject Trail; // trail prefab
    [SerializeField] private Transform trailSpawn; // spawn point of trails

    [SerializeField] private AudioClip speedUp; // sound when player accelerates
    [SerializeField] private AudioClip slowDown; // sound when player slows down

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(spawnTrails());
        }
    }

    /// <summary>
    /// Spawns trails that follow player.
    /// </summary>
    /// <returns></returns>
    IEnumerator spawnTrails()
    {
        GetComponent<AudioSource>().PlayOneShot(speedBonus > 0 ? speedUp : slowDown);
        FindObjectOfType<SpeedUI>().AddSpeed(speedBonus);
        InnerPart.SetActive(false);
        yield return new WaitForSeconds(.5f);
        if(emitTrails)
            Instantiate(Trail, trailSpawn.position, trailSpawn.rotation);
        gameObject.SetActive(false);
    }
}
