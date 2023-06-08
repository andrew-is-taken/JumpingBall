using UnityEngine;

public class UnlockLevelsButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }

    public void OnClick()
    {
        FindObjectOfType<DebugLevelData>().UnlockAllLevelsInSaveData();
    }
}
