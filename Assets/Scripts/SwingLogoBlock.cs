using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingLogoBlock : MonoBehaviour
{
    // ブロックを揺らし始めるスイッチとなるbool
    bool startSwing = false;

    // ブロックを揺らしている最中の時
    bool swinging = false;

    // 最初に揺らす向き（ランダムで決める）（ +1 か -1 が代入される）
    int dir;

    // 現在傾いている角度
    float currentTiltAngle = 0;
    // 傾ける角度
    float tiltAngle = 5.0f;
    // 傾ける速度
    float tiltSpeed = 20.0f;

    // ①往路
    bool step01 = false;
    // ②復路
    bool step02 = false;
    // ③元に戻す
    bool step03 = false;

    void Update()
    {
        // ブロックを揺らし始める合図を受け取ったときに、既に揺らしている最中でないとき
        if (startSwing && !swinging)
        {
            // 最初に揺らす向きをランダムで決める
            DecidePositiveOrNegative();

            step01 = true;
            swinging = true;
        }

        startSwing = false;
    }

    // 正負のランダム決定
    void DecidePositiveOrNegative()
    {
        float rnd = Random.Range(0, 2);
        dir = (rnd - 1 >= 0) ? 1 : -1;
    }

    void FixedUpdate()
    {
        if (swinging)
        {
            // ブロックを揺らす
            SwingProcess();
        }
    }

    // ブロックを揺らす
    void SwingProcess()
    {
        // ステップごとに関数を変える
        if (step01) SwingProcess_Step01();
        else if (step02) SwingProcess_Step02();
        else if (step03) SwingProcess_Step03();
        else swinging = false;
    }

    // ①往路
    void SwingProcess_Step01()
    {
        float rotAngle = tiltSpeed * Time.deltaTime;
        currentTiltAngle += rotAngle;

        // 現在の回転角度と上限値を調べる
        if (currentTiltAngle < tiltAngle)
        {
            // 上限に満たない場合、回転処理を続ける
            transform.Rotate(0, 0, rotAngle * dir);
        }
        else
        {
            // 上限を超過した場合、目標回転角度までの差分の分回転させる
            transform.Rotate(0, 0, (currentTiltAngle - tiltAngle) * dir);

            // Step01の終了、Step02へ
            currentTiltAngle = 0;
            step01 = false;
            step02 = true;
        }
    }

    // ②復路
    void SwingProcess_Step02()
    {
        // step02の回転速度及び目標回転量は他の2倍
        float rotAngle = tiltSpeed * Time.deltaTime * 2.0f;
        currentTiltAngle += rotAngle;

        // 現在の回転角度と上限値を調べる
        if (currentTiltAngle < tiltAngle * 2.0f)
        {
            // 上限に満たない場合、回転処理を続ける
            // 反対方向への回転なので、-1 を掛ける
            transform.Rotate(0, 0, rotAngle * (dir * -1.0f));
        }
        else
        {
            // 上限を超過した場合、目標回転角度までの差分の分回転させる
            transform.Rotate(0, 0, (currentTiltAngle - tiltAngle * 2.0f) * (dir * -1.0f));

            // Step02の終了、Step03へ
            currentTiltAngle = 0;
            step02 = false;
            step03 = true;
        }
    }

    // ③元に戻す
    void SwingProcess_Step03()
    {
        float rotAngle = tiltSpeed * Time.deltaTime;
        currentTiltAngle += rotAngle;

        // 現在の回転角度と上限値を調べる
        if (currentTiltAngle < tiltAngle)
        {
            // 上限に満たない場合、回転処理を続ける
            transform.Rotate(0, 0, rotAngle * dir);
        }
        else
        {
            // 上限を超過した場合、目標回転角度までの差分の分回転させる
            transform.Rotate(0, 0, (currentTiltAngle - tiltAngle) * dir);

            // Step03の終了、Swing処理の終了
            currentTiltAngle = 0;
            step03 = true;
            swinging = false;
        }
    }

    public bool StartSwing
    {
        set { startSwing = value; }
    }
}
