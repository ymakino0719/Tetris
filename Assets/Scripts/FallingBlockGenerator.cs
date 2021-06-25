using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockGenerator : MonoBehaviour
{
    static readonly int blockNum = 7;

    // 落ちるブロック
    [SerializeField] GameObject fallingBlock;

    // 落ちるブロックの画像のリスト
    [SerializeField] Sprite[] fallingBlockImage = new Sprite[blockNum];

    // ブロック生成間隔
    float generateInterval = 0.25f;

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

    void Start()
    {
        StartCoroutine("GenerateFallBlockCoroutine");
    }

    // 落ちるブロックを一定の間隔で生成するコルーチン
    public IEnumerator GenerateFallBlockCoroutine()
    {
        // ブロックの生成
        GenerateFallingBlock();

        yield return new WaitForSeconds(generateInterval);

        // 繰り返し
        StartCoroutine("GenerateFallBlockCoroutine");
    }

    // 落ちるブロックの生成
    void GenerateFallingBlock()
    {
        // 次に生成するテトリミノの種類（nextNum）に対応するテトリミノを生成
        GameObject fallB = Instantiate(fallingBlock) as GameObject;

        // 生成したブロックのテトリミノをランダムで決定
        int num = Random.Range(0, blockNum);
        fallB.GetComponent<SpriteRenderer>().sprite = fallingBlockImage[num];

        // 生成したブロックの位置を変更（X軸はランダムで決定）
        float pos_X = Random.Range(minRange_X, maxRange_X);
        fallB.transform.position = new Vector3(pos_X, generateLine_Y, 0);

        // 生成したブロックの角度をランダムで決定
        float angle = Random.Range(0, 360.0f);
        fallB.transform.eulerAngles = new Vector3(0, 0, angle);

        // 生成したブロックのサイズをランダムで決定
        float size = Random.Range(minSize, maxSize);
        fallB.transform.localScale = new Vector3(size, size, 1);

        // 親オブジェクトへ移動
        fallB.transform.parent = this.transform;
    }
}
