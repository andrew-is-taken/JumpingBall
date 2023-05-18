using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Sprite brokenSprite;
    public Transform laserStart;

    private Transform player;
    private bool broken;
    private bool playerInSight;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!broken)
        {
            RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
            playerInSight = hit.collider.tag == "Player";
            if (playerInSight)
            {

            }
        }
    }

    public void Death()
    {
        broken = true;
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
    }

    private IEnumerator checkPlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(laserStart.position, player.position);
        playerInSight = hit.collider.tag == "Player";

        yield return new WaitForSeconds(0.2f);
        if(!broken)
            StartCoroutine(checkPlayer());
    }
}
