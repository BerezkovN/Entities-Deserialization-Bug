using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeJoystickArea : MonoBehaviour
{
    void Awake()
    {
        ((RectTransform)this.transform).sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}
