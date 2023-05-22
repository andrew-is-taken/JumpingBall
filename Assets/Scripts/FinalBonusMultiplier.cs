using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBonusMultiplier : MonoBehaviour
{
    public float BonusMultiplier; // bonus that will be applied to player's money
    public bool TrueFinish; // if trigger indicates the real finish where level ends

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            MovementManager player = MovementManager.instance;
            player.SetEndLevelBonus(BonusMultiplier);
            if (TrueFinish)
            {
                player.Finish();
            }
        }
    }
}
