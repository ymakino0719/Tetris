using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockBehavior : MonoBehaviour
{
    // ブロックの移動距離
    float moveAmountY = 0.1f;
    // ブロックの加速度
    float accelerationCoef = 1.003f;

    // ブロックの回転速度（ランダム）
    float rotateAmount;

    // ブロックの回転速度の上限
    float rotateAmount_Max = 1.0f;

    // Y軸削除位置
    float deadLine_Y = -30.0f;

    void Start()
    {
        // ブロックの回転速度をランダムで決定
        rotateAmount = Random.Range(-rotateAmount_Max, rotateAmount_Max);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ブロックの降下
        DropBlock();

        // ブロックの回転
        RotateBlock();

        // ブロックの削除判定
        CheckDeadLine();
    }

    // ブロックの降下
    void DropBlock()
    {
        Vector3 pos = transform.position;

        // ブロックのサイズに反比例して、落下速度を遅くさせる
        pos.y -= moveAmountY / transform.localScale.x;

        // ブロックの移動速度の更新
        moveAmountY *= accelerationCoef;

        transform.position = pos;
    }

    // ブロックの回転
    void RotateBlock()
    {
        // ブロックのサイズに反比例して、回転速度を遅くさせる
        transform.Rotate(0, 0, rotateAmount / transform.localScale.x);
    }

    // ブロックの削除判定
    void CheckDeadLine()
    {
        // デッドラインに到達したらこのオブジェクトを削除する
        if (transform.position.y <= deadLine_Y) Destroy(this.gameObject);
    }
}
