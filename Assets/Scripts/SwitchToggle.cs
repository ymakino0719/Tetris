using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] InputManager iM;
    [SerializeField] Image image01;
    [SerializeField] Image image02;
    [SerializeField] Sprite sprite01;
    [SerializeField] Sprite sprite02;

    [SerializeField] GameObject touch;
    [SerializeField] GameObject button;

    [SerializeField] GameObject controlButtons;

    void Awake()
    {
        if (iM.TouchInputControl)
        {
            button.SetActive(false);
            controlButtons.SetActive(false);

            // トグルのオン時、オフ時の背景を設定する
            image01.sprite = sprite01;
            image02.sprite = sprite02;
        }
        else
        {
            touch.SetActive(false);

            // トグルのオン時、オフ時の背景を設定する（sprite01⇔sprite02）
            image01.sprite = sprite02;
            image02.sprite = sprite01;
        }
    }

    public void SwitchControlText()
    {
        iM.TouchInputControl = !iM.TouchInputControl;

        if (iM.TouchInputControl)
        {
            touch.SetActive(true);
            button.SetActive(false);
            controlButtons.SetActive(false);
        }
        else
        {
            touch.SetActive(false);
            button.SetActive(true);
            controlButtons.SetActive(true);
        }
    }
}
