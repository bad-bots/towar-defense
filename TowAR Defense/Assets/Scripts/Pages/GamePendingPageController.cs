using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePendingPageController : MonoBehaviour
{
    IEnumerator Load()
    {
        #if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(iOS.ActivityIndicatorStyle.Gray);
        #elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
        #endif

        Handheld.StartActivityIndicator();
        yield return new WaitForSeconds(0);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 200), "Load!"))
            StartCoroutine(Load());
    }

}