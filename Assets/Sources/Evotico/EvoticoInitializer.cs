using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvoticoInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }
}
