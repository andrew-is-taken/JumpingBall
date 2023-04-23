using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBonusMultiplier : MonoBehaviour
{
    public float BonusMultiplier;
    public bool TrueFinish;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Movement>().endLevelBonus = BonusMultiplier;
            if (TrueFinish)
            {
                collision.GetComponent<Movement>().Finish();
            }
        }
    }
}
