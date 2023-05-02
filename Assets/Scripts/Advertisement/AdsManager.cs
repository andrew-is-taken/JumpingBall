using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    //private string YOUR_APP_KEY = "19c430e3d";

    private void Start()
    {
#if UNITY_ANDROID
        string appKey = "19c430e3d";
#elif UNITY_IPHONE
        string appKey = "none";
#else
        string appKey = "unexpected_platform";
#endif



        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(appKey);

        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    /// <summary>
    /// deprecated
    /// </summary>
    public void Init()
    {
        //IronSource.Agent.init(YOUR_APP_KEY);
        //IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        //IronSource.Agent.validateIntegration();

        //IronSource.Agent.setMetaData("is_test_suite", "enable");


    }

    private void SdkInitializationCompletedEvent()
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}
