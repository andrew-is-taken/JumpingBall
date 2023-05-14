using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    private Animator anim; // animator of the enemy
    private GameObject projection; // projection on other side of playground

    void Start()
    {
        anim = GetComponent<Animator>();
        projection = transform.GetChild(0).gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            anim.enabled = false;
            projection.SetActive(false);
            GetComponent<SpriteRenderer>().color = new Color32(31, 31, 31, 255);
        }
    }

    /// <summary>
    /// Restarts the enemy to default settings when player respawns.
    /// </summary>
    public void RestartEnemy()
    {
        anim.enabled = true;
        projection.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color32(31, 31, 31, 0);
    }
}
