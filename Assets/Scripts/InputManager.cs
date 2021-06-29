using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // キー入力操作を有効にするか（無効の場合、タッチ操作になる）
    bool keyInputControl = true;

    // キー入力をしたとき press に true を、押し続けているときに pressing に true を代入
    bool upKey_Press = false;
    bool upKey_Pressing = false;

    bool downKey_Press = false;
    bool downKey_Pressing = false;

    bool leftKey_Press = false;
    bool leftKey_Pressing = false;

    bool rightKey_Press = false;
    bool rightKey_Pressing = false;

    bool holdKey_Press = false;

    bool hardDropKey_Press = false;

    string upKey = "UpKey";
    string downKey = "DownKey";
    string leftKey = "LeftKey";
    string rightKey = "RightKey";

    /*
    // 【タッチ操作】
    // 画面にタッチされた指の中でもっとも古いタッチのFingerIDが操作権を持つようにする（初期値はタッチがないため -1）
    int controlFingerId = -1;

    // 操作権を新しく持った有効なタッチの、最初の座標
    Vector3 startPos = Vector3.zero;

    // タッチ入力があるかどうか
    bool touching = false;

    // タッチ入力中に移動（操作）があったかどうか
    bool moved = false;

    // フリック操作を検知する閾値（フリックされた瞬間の距離で判定）
    float flickOperationThreshold = 0.5f;

    // スワイプ操作を検知する閾値（スワイプされた総距離で判定）
    float swipeOperationThreshold = 1.0f;
    */

    // 【クリック操作】
    // クリック押し込みの最初の座標
    Vector3 startPos = Vector3.zero;

    // クリックが押し込まれているかどうか
    bool clicking = false;

    // クリック入力中に移動（操作）があったかどうか
    bool moved = false;

    // フリック操作を検知する閾値（フリックされた瞬間の距離で判定）
    float flickOperationThreshold = 20.0f;

    // スワイプ操作を検知する閾値（スワイプされた総距離で判定）
    float swipeOperationThreshold = 10.0f;

    // Update is called once per frame
    void Update()
    {
        if(keyInputControl)
        {
            //【キー入力操作】
            // 上下左右キー入力受付
            CheckPressArrowKey(upKey, ref upKey_Press, ref upKey_Pressing);
            CheckPressArrowKey(downKey, ref downKey_Press, ref downKey_Pressing);
            CheckPressArrowKey(leftKey, ref leftKey_Press, ref leftKey_Pressing);
            CheckPressArrowKey(rightKey, ref rightKey_Press, ref rightKey_Pressing);

            // ホールドキー入力受付
            CheckPressHoldKey();

            // ハードドロップキー入力受付
            CheckPressHardDropKey();
        }
        else
        {
            //【マウス操作】
            CheckInputFromMouse();
        }

        // 【Touch】
        // 操作権限のあるタッチによる操作の場合、タッチ操作を開始する
        //if (CheckThisTouchIsAuthorizedToOperate()) DetectTouchingOperation();
    }

    // ボタンを押しているかの確認
    void CheckPressArrowKey(string keyDir, ref bool keyPress, ref bool keyPressing)
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

    void CheckPressHoldKey()
    {
        if(Input.GetButtonDown("HoldKey")) holdKey_Press = true;
    }
    void CheckPressHardDropKey()
    {
        if (Input.GetButtonDown("HardDropKey")) hardDropKey_Press = true;
    }

    /*
    // 操作権限のあるタッチがあるかどうか確認する（最も最初に触れた指がまだ画面上にあるかどうか）
    bool CheckThisTouchIsAuthorizedToOperate()
    {
        bool authority = false;

        // タッチ入力があるとき
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 最初の指が触れた時
            if (!touching)
            {
                // 最初に触れた指の fingerId を取得
                controlFingerId = touch.fingerId;
                // 最初に触れた指の最初の座標を取得
                startPos = touch.position;
                touching = true;
            }

            // 今画面に触れている指の中で最も最初に触れた指が、操作権を持つ指かどうか
            if (controlFingerId == touch.fingerId)
            {
                // 操作開始
                authority = true;
            }
            else
            {
                // 2本以上のタッチが検出されている間に、操作権を持つ指が離れてしまった場合
                // 今画面上にある最も最初に触れた指に操作権を移行させ、次フレームから操作可能とする
                controlFingerId = touch.fingerId;

                // 指が離れた時、移動がなかった場合はタップ操作とみなし、回転入力として受け付ける
                if (!moved) upKey_Press = true;

                // 最初の座標を更新
                startPos = touch.position;
            }
        }
        else if(touching)
        {
            // すべての指が画面から離れた時
            touching = false;

            // 指が離れた時、移動がなかった場合はタップ操作とみなし、回転入力として受け付ける
            if (!moved) upKey_Press = true;

            controlFingerId = -1;
            startPos = Vector3.zero;
        }

        return authority;
    }

    // タッチ操作を検知する
    void DetectTouchingOperation()
    {
        // フリック操作時の処理
        bool flick = FlickOperationProcess();
        if (flick)
        {
            // 移動したので true
            moved = true;
            // フリック有効時、以下の入力をスキップ
            return;
        } 

        // スワイプ操作時の処理
        bool swipe = SwipeOperationProcess();
        if (swipe)
        {
            // 移動したので true
            moved = true;
            // スワイプ有効時、以下の入力をスキップ
            return;
        }
    }

    // フリック操作時の処理
    bool FlickOperationProcess()
    {
        bool flick = true;

        Touch touch = Input.GetTouch(0);

        Vector2 dis = (touch.deltaPosition / touch.deltaTime) * Time.deltaTime;

        if (dis.y >= flickOperationThreshold)
        {
            // 上向きにフリック操作をしたとき
            holdKey_Press = true;
        }
        else if(dis.y <= -1.0f * flickOperationThreshold)
        {
            // 下向きにフリック操作をしたとき
            hardDropKey_Press = true;
        }
        else
        {
            // フリック操作がされていない場合
            flick = false;
        }

        return flick;
    }

    // スワイプ操作時の処理
    bool SwipeOperationProcess()
    {
        bool swipe = true;

        Touch touch = Input.GetTouch(0);

        // 現在いる座標と移動前の最初の座標の差をとる
        Vector2 vec = new Vector2(touch.position.x - startPos.x, touch.position.y - startPos.y);

        if (vec.x >= swipeOperationThreshold)
        {
            // 右にスワイプされた場合、右入力を有効にする
            rightKey_Press = true;
        }
        else if (vec.x <= -1.0f * swipeOperationThreshold)
        {
            // 左にスワイプされた場合、左入力を有効にする
            leftKey_Press = true;
        }
        else if (vec.y <= -1.0f * swipeOperationThreshold)
        {
            // 下にスワイプされた場合、下入力を有効にする
            downKey_Press = true;
        }
        else
        {
            // スワイプ操作がされていない場合
            swipe = false;
        }

        // スワイプ操作がされたとき、startPosを更新する
        if(swipe) startPos = touch.position;

        return swipe;
    }
    */

    // マウスからの入力を確認する
    void CheckInputFromMouse()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clicking = true;
            startPos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            clicking = false;
            // 指が離れた時、移動がなかった場合はタップ操作とみなし、回転入力として受け付ける
            if (!moved) upKey_Press = true;
            moved = false;
        }

        if(clicking)
        {
            bool flick = FlickOperationProcess();
            if (flick)
            {
                // 移動したので true
                moved = true;
                // 次のクリックまでフリックは無効
                clicking = false;
                // フリック有効時、以下の入力をスキップ
                return;
            }
            bool swipe = SwipeOperationProcess();
            if (swipe)
            {
                // 移動したので true
                moved = true;
                // スワイプ有効時、以下の入力をスキップ
                return;
            }
        }
    }

    // フリック操作時の処理
    bool FlickOperationProcess()
    {
        bool flick = true;

        Vector2 dis = Input.mousePosition - startPos;

        if (dis.y >= flickOperationThreshold)
        {
            // 上向きにフリック操作をしたとき
            holdKey_Press = true;
        }
        else if (dis.y <= -1.0f * flickOperationThreshold)
        {
            // 下向きにフリック操作をしたとき
            hardDropKey_Press = true;
        }
        else
        {
            // フリック操作がされていない場合
            flick = false;
        }

        return flick;
    }

    // スワイプ操作時の処理
    bool SwipeOperationProcess()
    {
        bool swipe = true;

        // 現在いる座標と移動前の最初の座標の差をとる
        Vector2 vec = new Vector2(Input.mousePosition.x - startPos.x, Input.mousePosition.y - startPos.y);

        if (vec.x >= swipeOperationThreshold)
        {
            // 右にスワイプされた場合、右入力を有効にする
            rightKey_Press = true;
        }
        else if (vec.x <= -1.0f * swipeOperationThreshold)
        {
            // 左にスワイプされた場合、左入力を有効にする
            leftKey_Press = true;
        }
        else if (vec.y <= -1.0f * swipeOperationThreshold)
        {
            // 下にスワイプされた場合、下入力を有効にする
            downKey_Press = true;
        }
        else
        {
            // スワイプ操作がされていない場合
            swipe = false;
        }

        // スワイプ操作がされたとき、startPosを更新する
        if (swipe) startPos = Input.mousePosition;

        return swipe;
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

    // キー入力かタッチ操作かを切り替える
    public void ToggleKeyOrTouch()
    {
        keyInputControl = !keyInputControl;
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
    public bool HoldKey_Press
    {
        set { holdKey_Press = value; }
        get { return holdKey_Press; }
    }
    public bool HardDropKey_Press
    {
        set { hardDropKey_Press = value; }
        get { return hardDropKey_Press; }
    }
    public bool KeyInputControl
    {
        set { keyInputControl = value; }
        get { return keyInputControl; }
    }
}
