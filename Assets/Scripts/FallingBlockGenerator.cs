using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockGenerator : MonoBehaviour
{
    static readonly int blockNum = 7;

    // 落ちるブロック
    [SerializeField] GameObject fallingBlock;

    // ブロック生成間隔
    float generateInterval = 0.25f;

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

        // 親オブジェクトへ移動
        fallB.transform.parent = this.transform;
    }
}
