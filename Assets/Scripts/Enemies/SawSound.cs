using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SawSound : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
            GetComponent<AudioSource>().Play();
    }
}
