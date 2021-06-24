using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // キー入力をしたとき press に true を、押し続けているときに pressing に true を代入
    bool upKey_Press = false;
    bool upKey_Pressing = false;

    bool downKey_Press = false;
    bool downKey_Pressing = false;

    bool leftKey_Press = false;
    bool leftKey_Pressing = false;

    bool rightKey_Press = false;
    bool rightKey_Pressing = false;

    string upKey = "UpKey";
    string downKey = "DownKey";
    string leftKey = "LeftKey";
    string rightKey = "RightKey";

    // Update is called once per frame
    void Update()
    {
        CheckPressKey(upKey, ref upKey_Press, ref upKey_Pressing);
        CheckPressKey(downKey, ref downKey_Press, ref downKey_Pressing);
        CheckPressKey(leftKey, ref leftKey_Press, ref leftKey_Pressing);
        CheckPressKey(rightKey, ref rightKey_Press, ref rightKey_Pressing);
    }

    // ボタンを押しているかの確認
    // （※単押し時の処理は BlockBehavior で処理を行い、）
    void CheckPressKey(string keyDir, ref bool keyPress, ref bool keyPressing)
    {
        // ボタンを押したとき
        if (Input.GetButtonDown(keyDir))
        {
            keyPress = true;
            keyPressing = true;
        }
        
        // ボタンを離したとき
        if(Input.GetButtonUp(keyDir))
        {
            keyPressing = false;
        }
    }
    // UIボタンからのキー入力
    // ボタンが押されたとき、単押し・長押し双方とも true を代入
    public void PointerDown_UpKey()
    {
        upKey_Press = true;
        upKey_Pressing = true;
    }
    public void PointerDown_DownKey()
    {
        downKey_Press = true;
        downKey_Pressing = true;
    }
    public void PointerDown_LeftKey()
    {
        leftKey_Press = true;
        leftKey_Pressing = true;
    }
    public void PointerDown_RightKey()
    {
        rightKey_Press = true;
        rightKey_Pressing = true;
    }

    // ボタンを離したとき、長押しboolに false を代入
    public void PointerUp_UpKey()
    {
        upKey_Pressing = false;
    }
    public void PointerUp_DownKey()
    {
        downKey_Pressing = false;
    }
    public void PointerUp_LeftKey()
    {
        leftKey_Pressing = false;
    }
    public void PointerUp_RightKey()
    {
        rightKey_Pressing = false;
    }

    public bool UpKey_Press
    {
        set { upKey_Press = value; }
        get { return upKey_Press; }
    }
    public bool DownKey_Press
    {
        set { downKey_Press = value; }
        get { return downKey_Press; }
    }
    public bool LeftKey_Press
    {
        set { leftKey_Press = value; }
        get { return leftKey_Press; }
    }
    public bool RightKey_Press
    {
        set { rightKey_Press = value; }
        get { return rightKey_Press; }
    }
    public bool UpKey_Pressing
    {
        get { return upKey_Pressing; }
    }

    public bool DownKey_Pressing
    {
        get { return downKey_Pressing; }
    }

    public bool LeftKey_Pressing
    {
        get { return leftKey_Pressing; }
    }

    public bool RightKey_Pressing
    {
        get { return rightKey_Pressing; }
    }
}
