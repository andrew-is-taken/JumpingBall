using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DisappearingPlatform : MonoBehaviour
{
    public bool disappearing = false; // if the platform can become invisible
    public float visiblePeriod = 1f; // time when platform is visible
    public float invisiblePeriod = 1f; // time when platform is invisible

    void Start()
    {
        if(disappearing)
            StartCoroutine(disappear());
        else
            GetComponent<Animator>().enabled = false;
    }

    private IEnumerator disappear()
    {
        GetComponent<Animator>().SetBool("Disappear", true);
        yield return new WaitForSeconds(invisiblePeriod + 2.5f);
        GetComponent<Animator>().SetBool("Disappear", false);
        yield return new WaitForSeconds(visiblePeriod);
        StartCoroutine(disappear());
    }
}
