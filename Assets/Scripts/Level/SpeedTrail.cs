using System;
using UnityEngine;

public class SpeedTrail : MonoBehaviour
{
    private Transform Player; // player
    private float t = 0f; // time for lerp

    private void Start()
    {
        try
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    { 
        LerpTrails();
    }

    /// <summary>
    /// Moves the trails to player's position.
    /// </summary>
    private void LerpTrails()
    {
        t += Time.deltaTime / 3;
        if (Player != null)
        {
            transform.position = Vector3.Lerp(transform.position, Player.position, t);
            if (Vector3.Distance(transform.position, Player.position) < .3f)
            {
                Destroy(gameObject);
            }
        }
    }
}
