using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{    
    static readonly int blockNum = 7;
    [SerializeField] GameObject[] currentBlocks = new GameObject[blockNum]; // 今動かすブロックのリスト
    [SerializeField] GameObject[] nextBlocks = new GameObject[blockNum]; // 次のブロックのリスト（動かない）

    int nextNum;
    [SerializeField] GameObject nextBlockParent; // NextBlockのPrefabを子に持たせる親オブジェクト

    void Start()
    {
        nextNum = Random.Range(0, blockNum);
        GenerateCurrentBlock(); // （開幕処理）最初のブロックを生成
        GenerateNextBlock(); // （開幕処理）次のブロックを生成
    }

    public void GenerateCurrentBlock() // 今のブロックを生成する
    {
        GameObject currentBlock = Instantiate(currentBlocks[nextNum]) as GameObject;// 次に生成するテトリミノの種類（nextNum）に対応するテトリミノを生成
    }
    public void GenerateNextBlock() // 次のブロックを生成する
    {
        foreach (Transform n in nextBlockParent.transform) GameObject.Destroy(n.gameObject); // 次のブロックとして表示されていたブロックを削除する

        nextNum = Random.Range(0, blockNum); // 次のテトリミノの種類をランダムで決定
        GameObject nextBlock = Instantiate(nextBlocks[nextNum]) as GameObject;// 次のテトリミノを生成
        nextBlock.transform.parent = nextBlockParent.transform;// NextBlockの子オブジェクトに配置
    }
}
