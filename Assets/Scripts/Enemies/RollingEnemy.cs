using System.Collections;
using UnityEngine;

public class RollingEnemy : MonoBehaviour
{
    [SerializeField] private Vector2 rollingDirection;
    [SerializeField] private float speed;
    [SerializeField] private float delay;

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

    /// <summary>
    /// Waits delay time to start the movement.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        AddForce();
    }

    /// <summary>
    /// Pushes enemy in direction.
    /// </summary>
    private void AddForce()
    {
        GetComponent<Rigidbody2D>().AddForce(rollingDirection * 100f * speed, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Turns on death animation and stops the enemy.
    /// </summary>
    private void Death()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().enabled = true;
    }
}
