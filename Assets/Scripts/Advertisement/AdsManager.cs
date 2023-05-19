using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        string appKey = "19c430e3d";
#elif UNITY_IPHONE
        string appKey = "none";
#else
        string appKey = "unexpected_platform";
#endif

#if UNITY_EDITOR
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(appKey);
#else
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);
#endif

        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    /// <summary>
    /// Event of completed initialization.
    /// </summary>
    private void SdkInitializationCompletedEvent()
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
    }
}
