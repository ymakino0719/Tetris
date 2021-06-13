using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    static readonly int blockNum = 7;
    [SerializeField] GameObject[] currentBlocks = new GameObject[blockNum]; // 今動かすブロックのリスト
    [SerializeField] GameObject[] nextBlocks = new GameObject[blockNum]; // 次のブロックのリスト（動かない）
    [SerializeField] GameObject[] transparentBlocks = new GameObject[blockNum]; // ブロックの落下予測地点を示す、透明なブロックのリスト

    int nextNum;
    [SerializeField] GameObject currentBlockParent; // CurrentBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject nextBlockParent; // NextBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject transparentBlockParent; // TransparentBlockのPrefabを子に持たせる親オブジェクト

    void Start()
    {
        nextNum = Random.Range(0, blockNum);
        GenerateCurrentBlock(); // （開幕処理）最初のブロックを生成
        GenerateTransparentBlock(); // （開幕処理）最初の透明ブロックを生成
        GenerateNextBlock(); // （開幕処理）次のブロックを生成
    }

    public void GenerateCurrentBlock() // 今のブロックを生成する
    {
        GenerateAndSetParent(currentBlocks, currentBlockParent);
    }
    public void GenerateTransparentBlock() // ブロックの落下予測地点を示す、透明なブロックを生成する
    {
        GenerateAndSetParent(transparentBlocks, transparentBlockParent);
    }
    public void GenerateNextBlock() // 次のブロックを生成する
    {
        nextNum = Random.Range(0, blockNum); // 次のテトリミノの種類をランダムで決定
        GenerateAndSetParent(nextBlocks, nextBlockParent);
    }

    public void GenerateAndSetParent(GameObject[] blocks, GameObject parent) // 次のテトリミノを生成し、親を設定する
    {
        GameObject block = Instantiate(blocks[nextNum]) as GameObject;// 次に生成するテトリミノの種類（nextNum）に対応するテトリミノを生成
        block.transform.parent = parent.transform;// CurrentBlockの子オブジェクトに配置
    }

    public void DestroyBlockGameObject(GameObject gao)
    {
        foreach (Transform n in gao.transform) GameObject.Destroy(n.gameObject); // 次のブロックとして表示されていたブロックを削除する
    }
    public GameObject NextBlockParent
    {
        get { return nextBlockParent; }
    }
    public GameObject TransparentBlockParent
    {
        get { return transparentBlockParent; }
    }
}

