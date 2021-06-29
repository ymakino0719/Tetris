using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    [SerializeField] BlockList bL;

    static readonly int blockNum = 7;

    // テトリミノのパターンのリスト
    [SerializeField] GameObject[] tetriminoPattern = new GameObject[blockNum];

    // 普通のブロックの画像
    [SerializeField] Sprite[] currentBlockImage = new Sprite[blockNum];
    // 透明なブロックの画像
    [SerializeField] Sprite[] transparentBlockImage = new Sprite[blockNum];

    int nextNum;
    // 次に生成可能なテトリミノの、残りの種類のリスト
    // （テトリミノは1セット7種類が重複なしでランダムで生成され、1セットが終了次第、次のセットに移行する）
    List<int> tetrimino_RemainingTypes = new List<int>();

    // CurrentBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject currentBlockParent;
    // NextBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject nextBlockParent;
    // TransparentBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject transparentBlockParent;
    // HoldBlockのPrefabを子に持たせる親オブジェクト
    [SerializeField] GameObject holdingBlockParent;

    // NextとHoldブロックのサイズ
    float nextAndHoldBlockSize = 0.55f;

    // Spriteの描写順序（透明なブロックだけ OrderInLayer の値を下げる）
    int transparentSortingOrder = 1;
    int defaultSortingOrder = 2;

    void Start()
    {
        // 次のテトリミノの種類をランダムで決定
        nextNum = DecideNextBlock();

        // （開幕処理）最初のブロックを生成
        GenerateCurrentBlock(nextNum);
        // （開幕処理）最初の透明ブロックを生成
        GenerateTransparentBlock(nextNum);
        // （開幕処理）次のブロックを生成
        GenerateNextBlock();
    }

    // 次に生成するテトリミノの種類を決定する
    int DecideNextBlock()
    {
        // 1セット7種類の生成が終わっている場合は、次のリストを生成する（0～blockNum を順番に入れるだけ）
        if (tetrimino_RemainingTypes.Count == 0) for (int i = 0; i < blockNum; i++) tetrimino_RemainingTypes.Add(i);

        // 次に生成するテトリミノの種類を、生成可能な残りのリストを参考に決定する
        int rnd = Random.Range(0, tetrimino_RemainingTypes.Count);
        int next = tetrimino_RemainingTypes[rnd];

        // 生成したテトリミノをリストから除外
        tetrimino_RemainingTypes.RemoveAt(rnd);

        return next;
    }

    // 今のブロックを生成する
    public void GenerateCurrentBlock(int num)
    {
        // 生成するブロックのタイプを指定
        // blockType 0: 今落下させるブロック
        int blockType = 0;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(4, 19, 0);

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(1, 1, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize, num);
    }

    // 次のブロックを生成する
    public void GenerateNextBlock()
    {
        // 次のテトリミノの種類をランダムで決定
        nextNum = DecideNextBlock();

        // 生成するブロックのタイプを指定
        // blockType 1: 次に落下させるブロック（Nextに表示する）
        int blockType = 1;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(0.6f, 21.25f, 0);
        // ※テトリミノの内、IとOは座標を少しずらす
        if (nextNum == 0) { blockPos += new Vector3(0.4f, 0.5f, 0); }
        else if (nextNum == 3) { blockPos += new Vector3(-0.2f, 0, 0); }

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(nextAndHoldBlockSize, nextAndHoldBlockSize, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize, nextNum);
    }

    // ブロックの落下予測地点を示す、透明なブロックを生成する
    public void GenerateTransparentBlock(int num)
    {
        // 生成するブロックのタイプを指定
        // blockType 2: 今落下させるブロックの落下地点表示となる透明なブロック
        int blockType = 2;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(-10, -10, 0);

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(1, 1, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize, num);
    }

    // ホールドブロックを生成する
    public void GenerateHoldingBlock(int num)
    {
        // 生成するブロックのタイプを指定
        // blockType 3: ホールドブロック
        int blockType = 3;

        // 生成するブロックの位置を指定
        Vector3 blockPos = new Vector3(8.2f, 21.25f, 0);
        // ※テトリミノの内、IとOは座標を少しずらす
        if (num == 0) { blockPos += new Vector3(0.4f, 0.25f, 0); }
        else if (num == 3) { blockPos += new Vector3(-0.2f, 0, 0); }

        // 生成するブロックの大きさを指定
        Vector3 blockSize = new Vector3(nextAndHoldBlockSize, nextAndHoldBlockSize, 1);

        GenerateAndSetBlock(blockType, blockPos, blockSize, num);
    }

    // 次のテトリミノを生成し、親を設定する
    void GenerateAndSetBlock(int bType, Vector3 bPos, Vector3 bSize, int num)
    {
        // 次に生成するテトリミノの種類（num）に対応するテトリミノを生成
        GameObject block = Instantiate(tetriminoPattern[num]) as GameObject;

        // 生成したブロックのタイプに応じたスプライトと描写順に変更
        Sprite s = currentBlockImage[num];

        if (bType == 2) s = transparentBlockImage[num];
        else s = currentBlockImage[num];

        foreach (Transform b in block.transform)
        {
            SpriteRenderer sR = b.GetComponent<SpriteRenderer>();
            // スプライトを変更する
            sR.sprite = s;
            // スプライトの描写順を修正する（TransparentBlock が CurrentBlock の前に来ないようにする）
            if (bType == 2) sR.sortingOrder = transparentSortingOrder;
            //else sR.sortingOrder = defaultSortingOrder;
        }

        // 生成したブロックの位置を変更
        block.transform.position = bPos;

        // 生成したブロックのサイズを変更
        block.transform.localScale = bSize;

        // CurrentBlockの子オブジェクトに配置
        GameObject parent = null;
        if (bType == 0) parent = currentBlockParent;
        else if (bType == 1) parent = nextBlockParent;
        else if (bType == 2) parent = transparentBlockParent;
        else if (bType == 3) parent = holdingBlockParent;

        // CurrentBlockのとき
        if (bType == 0)
        {
            // スクリプトを追加
            block.AddComponent<BlockBehavior>();
            // テトリミノの種類を渡す
            block.GetComponent<BlockBehavior>().TetriminoType = num;
        } 

        block.transform.parent = parent.transform;
    }

    public void DestroyBlockGameObject(GameObject gao)
    {
        // 次のブロックとして表示されていたブロックを削除する
        foreach (Transform n in gao.transform) GameObject.Destroy(n.gameObject);
    }

    // 終了処理（BlockBehavior:EndProcess01 の続き）
    public void EndProcess02()
    {
        // 今のブロックを生成
        GenerateCurrentBlock(nextNum);

        // 今の透明ブロックを生成
        GenerateTransparentBlock(nextNum);

        // 次のブロックとして表示されていた前のブロックを、更新のため削除する
        DestroyBlockGameObject(nextBlockParent);

        // 次のブロックを生成
        GenerateNextBlock();
    }

    // 1回目のホールド入力があった時の操作（Holdブロックがないとき）
    public void HoldProcess_FirstTime(int num)
    {
        // 落下地点として表示されていた透明なブロックを、更新のため削除する
        DestroyBlockGameObject(TransparentBlockParent);

        // 今回ホールドされるHoldブロックをUIに表示する
        GenerateHoldingBlock(num);

        // 終了処理02へ移行
        EndProcess02();
    }

    // 2回目以降のホールド入力があった時の操作（Holdブロックがあるとき）
    public void HoldProcess_AfterTheSecondTime(int num)
    {
        // 前のホールドとして表示されていたHoldブロックを、更新のため削除する
        DestroyBlockGameObject(HoldingBlockParent);

        // 落下地点として表示されていた透明なブロックを、更新のため削除する
        DestroyBlockGameObject(TransparentBlockParent);

        // HoldされていたブロックをCurrentBlockとして生成する
        GenerateCurrentBlock(bL.HoldBlockNum);

        // Holdされていたブロックの透明ブロックを生成
        GenerateTransparentBlock(bL.HoldBlockNum);

        // 今回ホールドされるHoldブロックをUIに表示する
        GenerateHoldingBlock(num);
    }

    public GameObject NextBlockParent
    {
        get { return nextBlockParent; }
    }
    public GameObject TransparentBlockParent
    {
        get { return transparentBlockParent; }
    }
    public GameObject HoldingBlockParent
    {
        get { return holdingBlockParent; }
    }
}

