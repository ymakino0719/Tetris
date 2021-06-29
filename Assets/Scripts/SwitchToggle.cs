using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] InputManager iM;

    [SerializeField] GameObject touch;
    [SerializeField] GameObject button;

    void Awake()
    {
        if (iM.KeyInputControl) touch.SetActive(false);
        else button.SetActive(false);
    }

    public void SwitchControlText()
    {
        iM.KeyInputControl = !iM.KeyInputControl;

        if (iM.KeyInputControl)
        {
            touch.SetActive(false);
            button.SetActive(true);
        }
        else
        {
            touch.SetActive(true);
            button.SetActive(false);
        }
    }
}
