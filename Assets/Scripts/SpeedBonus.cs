using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBonus : MonoBehaviour
{
    public bool emitTrails = false; // if trails spawn after the player picks up bonus
    public float speedBonus = -1f; // bonus to player's speed

    public GameObject InnerPart; // inner part of the bonus
    public GameObject Trail; // trail prefab
    public Transform trailSpawn; // spawn point of trails

    public AudioClip speedUp; // sound when player accelerates
    public AudioClip slowDown; // sound when player slows down

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
