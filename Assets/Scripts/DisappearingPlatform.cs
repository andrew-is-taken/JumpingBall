using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DisappearingPlatform : MonoBehaviour
{
    public bool disappearing = false; // if the platform can become invisible
    public bool visibleBeforeInvisible; // if we start with visible platform
    public float visiblePeriod = 1f; // time when platform is visible
    public float invisiblePeriod = 1f; // time when platform is invisible
    public float delayTime;

    void Start()
    {
        if(disappearing)
            StartCoroutine(Delay());
        else
            GetComponent<Animator>().enabled = false;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        if (visibleBeforeInvisible)
        {
            GetComponent<Animator>().SetBool("Disappear", false);
            yield return new WaitForSeconds(visiblePeriod);
            GetComponent<Animator>().SetBool("Disappear", true);
            yield return new WaitForSeconds(invisiblePeriod + 2.5f);
        }
        else
        {
            GetComponent<Animator>().SetBool("Disappear", true);
            yield return new WaitForSeconds(invisiblePeriod + 2.5f);
            GetComponent<Animator>().SetBool("Disappear", false);
            yield return new WaitForSeconds(visiblePeriod);
        }
        StartCoroutine(Disappear());
    }
}
