using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeJoystickArea : MonoBehaviour
{
    void Awake()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        float anchorX = 0.5f;
#else
        float anchorX = 1.0f;
#endif
        
        ((RectTransform)this.transform).anchorMin = new Vector2(anchorX, 0);
    }
}
