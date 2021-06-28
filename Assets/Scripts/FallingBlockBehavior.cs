using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockBehavior : MonoBehaviour
{
    static readonly int blockNum = 7;

    [SerializeField] SpriteRenderer sP;

    // 落ちるブロックの画像のリスト
    [SerializeField] Sprite[] fallingBlockImage = new Sprite[blockNum];

    // 生成位置
    // X軸下限
    float minRange_X = -14.0f;
    // X軸上限
    float maxRange_X = 14.0f;
    // Y軸生成位置
    float generateLine_Y = 30.0f;

    // ブロックの下限、上限サイズ
    float minSize = 0.5f;
    float maxSize = 0.9f;

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

    void Awake()
    {
        InitialDeterminationOfEachValue();
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

    // FallingBlockの各値の決定
    void InitialDeterminationOfEachValue()
    {
        // 生成したブロックの位置を変更（X軸はランダムで決定）
        float pos_X = Random.Range(minRange_X, maxRange_X);
        transform.position = new Vector3(pos_X, generateLine_Y, 0);

        // 生成したブロックの角度をランダムで決定
        float angle = Random.Range(0, 360.0f);
        transform.eulerAngles = new Vector3(0, 0, angle);

        // 生成したブロックのサイズをランダムで決定
        float size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, 1);

        // 生成したブロックのテトリミノをランダムで決定
        int num = Random.Range(0, blockNum);
        sP.sprite = fallingBlockImage[num];

        // ブロックの回転速度をランダムで決定
        rotateAmount = Random.Range(-rotateAmount_Max, rotateAmount_Max);
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
