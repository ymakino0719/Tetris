using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    static readonly int blockNum = 7;

    // テトリミノのパターンのリスト
    [SerializeField] GameObject[] tetriminoPattern = new GameObject[blockNum];

    // 普通のブロックの画像
    [SerializeField] Sprite[] currentBlockImage = new Sprite[blockNum];
    // 透明なブロックの画像
    [SerializeField] Sprite[] transparentBlockImage = new Sprite[blockNum];

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
        // 生成するブロックのタイプを指定
        int blockType = 0;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(4, 19, 0);

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(1, 1, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize);
    }

    // 次のブロックを生成する
    public void GenerateNextBlock()
    {
        // 次のテトリミノの種類をランダムで決定
        nextNum = Random.Range(0, blockNum);

        // 生成するブロックのタイプを指定
        int blockType = 1;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(2f, 20.75f, 0);
        // ※テトリミノの内、IとOは座標を少しずらす
        if (nextNum == 0) { blockPos += new Vector3(-0.4f, 0.5f, 0); }
        else if (nextNum == 3) { blockPos += new Vector3(-0.4f, 0, 0); }

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(0.8f, 0.8f, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize);
    }

    // ブロックの落下予測地点を示す、透明なブロックを生成する
    public void GenerateTransparentBlock()
    {
        // 生成するブロックのタイプを指定
        int blockType = 2;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(-10, -10, 0);

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(1, 1, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize);
    }

    // 次のテトリミノを生成し、親を設定する
    void GenerateAndSetBlock(int bType, Vector3 bPos, Vector3 bSize)
    {
        // 次に生成するテトリミノの種類（nextNum）に対応するテトリミノを生成
        GameObject block = Instantiate(tetriminoPattern[nextNum]) as GameObject;

        // 生成したブロックのタイプに応じたスプライトに変更
        Sprite s = currentBlockImage[nextNum];

        if (bType == 2) s = transparentBlockImage[nextNum];
        else s = currentBlockImage[nextNum];

        foreach (Transform b in block.transform) b.GetComponent<SpriteRenderer>().sprite = s;

        // 生成したブロックの位置を変更
        block.transform.position = bPos;

        // 生成したブロックのサイズを変更
        block.transform.localScale = bSize;

        // CurrentBlockの子オブジェクトに配置
        GameObject parent = null;
        if (bType == 0) parent = currentBlockParent;
        else if (bType == 1) parent = nextBlockParent;
        else if (bType == 2) parent = transparentBlockParent;

        // スクリプトを追加
        if (bType == 0) block.AddComponent<BlockBehavior>();

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

