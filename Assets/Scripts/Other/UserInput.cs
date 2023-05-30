using UnityEngine;

public class UserInput : MonoBehaviour
{
    private MovementManager movementManager;

    /// <summary>
    /// Updates the movement manager instance.
    /// </summary>
    /// <param name="player"></param>
    public void UpdatePlayerInstance(MovementManager player)
    {
        movementManager = player;
    }

    /// <summary>
    /// When user touches the screen.
    /// </summary>
    public void TapOnScreen()
    {
        movementManager.TapOnScreen();
    }
}
