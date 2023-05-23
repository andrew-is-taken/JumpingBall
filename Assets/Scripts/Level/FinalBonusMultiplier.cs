using UnityEngine;

public class FinalBonusMultiplier : MonoBehaviour
{
    [SerializeField] private float BonusMultiplier; // bonus that will be applied to player's money
    [SerializeField] private bool TrueFinish; // if trigger indicates the real finish where level ends

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
