using UnityEngine;

public class UnlockLevelsButton : MonoBehaviour
{
    public void OnClick()
    {
        FindObjectOfType<DebugLevelData>().UnlockAllLevelsInSaveData();
    }
}
