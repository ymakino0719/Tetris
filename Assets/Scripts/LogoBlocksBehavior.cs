using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoBlocksBehavior : MonoBehaviour
{
    static readonly int blockNum = 7;

    // テトリミノごとのロゴブロックのリスト
    [SerializeField] GameObject[] logoBlocks = new GameObject[blockNum];

    // SwingLogoBlockがアタッチされている子オブジェクト（ロゴブロック）のコンポーネントリスト
    List<SwingLogoBlock> swingTargetList = new List<SwingLogoBlock>();

    // ロゴブロックを動かすランダムな時間間隔の上限と下限
    float intervalTime_Min = 0.02f;
    float intervalTime_Max = 0.1f;

    void Awake()
    {
        // ロゴブロックにアタッチされている SwingLogoBlock コンポーネントをすべて取得する
        GetAllSwingComponentList();
    }

    void Start()
    {
        // ランダムな間隔でロゴブロックを動かすためのコルーチンの開始
        StartCoroutine("SwingLogoBlocksCoroutine");
    }

    // SwingLogoBlockコンポーネントをすべて取得する
    void GetAllSwingComponentList()
    {
        for(int i = 0; i < logoBlocks.Length; i++)
        {
            foreach(Transform child in logoBlocks[i].transform)
            {
                swingTargetList.Add(child.GetComponent<SwingLogoBlock>());
            }
        }
    }

    // ランダムな間隔でロゴブロックを動かすためのコルーチン
    public IEnumerator SwingLogoBlocksCoroutine()
    {
        // 揺らすブロックの決定
        SelectLogoBlockToSwing();

        // ランダムな時間間隔を生成し、その時間分だけ待機する
        float rnd = Random.Range(intervalTime_Min, intervalTime_Max);
        yield return new WaitForSeconds(rnd);

        // 繰り返し
        StartCoroutine("SwingLogoBlocksCoroutine");
    }

    // 揺らすブロックの決定
    void SelectLogoBlockToSwing()
    {
        // ランダムで決定
        int rnd = Random.Range(0, swingTargetList.Count);

        swingTargetList[rnd].StartSwing = true;
    }
}
