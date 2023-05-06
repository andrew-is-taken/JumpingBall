using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBonus : MonoBehaviour
{
    public bool emitTrails = false;
    public float speedBonus = -1f;

    public GameObject InnerPart;
    public GameObject Trail;
    public Transform trailSpawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(spawnTrails());
        }
    }

    IEnumerator spawnTrails()
    {
        GetComponent<AudioSource>().Play();
        FindObjectOfType<SpeedUI>().AddSpeed(speedBonus);
        InnerPart.SetActive(false);
        yield return new WaitForSeconds(.5f);
        if(emitTrails)
            Instantiate(Trail, trailSpawn.position, trailSpawn.rotation);
        gameObject.SetActive(false);
    }
}
