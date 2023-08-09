using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeJoystickArea : MonoBehaviour
{
    public GameObject JoystickBase;
    
    // Start is called before the first frame update
    void Awake()
    {
        ((RectTransform)JoystickBase.transform).sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}
