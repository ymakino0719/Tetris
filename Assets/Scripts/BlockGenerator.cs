using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    static readonly int blockNum = 7;

    // 今動かすブロックのリスト
    [SerializeField] GameObject[] currentBlocks = new GameObject[blockNum];
    // 次のブロックのリスト（動かない）
    [SerializeField] GameObject[] nextBlocks = new GameObject[blockNum];
    // ブロックの落下予測地点を示す、透明なブロックのリスト
    [SerializeField] GameObject[] transparentBlocks = new GameObject[blockNum];

    int nextNum;

    // CurrentBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject currentBlockParent;
    // NextBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject nextBlockParent;
    // TransparentBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject transparentBlockParent;

    void Start()
    {
        nextNum = Random.Range(0, blockNum);

        // （開幕処理）最初のブロックを生成
        GenerateCurrentBlock();
        // （開幕処理）最初の透明ブロックを生成
        GenerateTransparentBlock();
        // （開幕処理）次のブロックを生成
        GenerateNextBlock();
    }

    // 今のブロックを生成する
    public void GenerateCurrentBlock()
    {
        GenerateAndSetParent(currentBlocks, currentBlockParent);
    }

    // ブロックの落下予測地点を示す、透明なブロックを生成する
    public void GenerateTransparentBlock()
    {
        GenerateAndSetParent(transparentBlocks, transparentBlockParent);
    }

    // 次のブロックを生成する
    public void GenerateNextBlock()
    {
        // 次のテトリミノの種類をランダムで決定
        nextNum = Random.Range(0, blockNum);

        GenerateAndSetParent(nextBlocks, nextBlockParent);
    }

    // 次のテトリミノを生成し、親を設定する
    public void GenerateAndSetParent(GameObject[] blocks, GameObject parent)
    {
        // 次に生成するテトリミノの種類（nextNum）に対応するテトリミノを生成
        GameObject block = Instantiate(blocks[nextNum]) as GameObject;
        // CurrentBlockの子オブジェクトに配置
        block.transform.parent = parent.transform;
    }

    public void DestroyBlockGameObject(GameObject gao)
    {
        // 次のブロックとして表示されていたブロックを削除する
        foreach (Transform n in gao.transform) GameObject.Destroy(n.gameObject);
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

