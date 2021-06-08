using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{    
    static readonly int blockNum = 7;
    public GameObject[] blocks = new GameObject[blockNum];

    void Start()
    {
        GenerateNextBlock(); // （開幕処理）最初のブロックを生成
    }

    public void GenerateNextBlock() // 次のブロックを生成する
    {
        GameObject nextBlock = Instantiate(blocks[Random.Range(0, blockNum)]) as GameObject;// ランダムなテトリミノを生成
    }
}
