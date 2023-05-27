using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public void UnlockAllLevels()
    {
        FindObjectOfType<LevelManager>().UnlockAllLevelsInSaveData();
    }
}
