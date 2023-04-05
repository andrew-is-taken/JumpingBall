using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrail : MonoBehaviour
{
    private Transform Player;
    private float t = 0f;

    private void Start()
    {
        try
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        catch
        {
            
        }
    }

    private void Update()
    {
        t += Time.deltaTime / 3;
        if(Player != null)
        {
            transform.position = Vector3.Lerp(transform.position, Player.position, t);
            if (Vector3.Distance(transform.position, Player.position) < .3f)
            {
                Destroy(gameObject);
            }
        }
    }
}
