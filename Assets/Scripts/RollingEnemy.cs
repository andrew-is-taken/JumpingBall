using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour
{
    public Vector2 rollingDirection;
    public float speed;
    public float delay;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            Death();
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        AddForce();
    }

    private void AddForce()
    {
        GetComponent<Rigidbody2D>().AddForce(rollingDirection * 100f * speed, ForceMode2D.Impulse);
    }

    private void Death()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().enabled = true;
    }
}
