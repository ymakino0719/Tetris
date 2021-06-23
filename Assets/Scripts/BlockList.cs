using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    [SerializeField] ScoreManager sM;
    [SerializeField] SoundManager soundM;
    [SerializeField] PerformanceManager_Stage perM;
    [SerializeField] BlockGenerator bG;

    // 列数
    static readonly int row = 10;
    // 行数
    static readonly int column = 25;

    // 指定の行数・列数にブロックが存在するか（存在する場合true）
    bool[,] blocks = new bool[row, column];
    // 指定行の消去をするかどうか
    bool[] deleteLineBool = new bool[column];

    // 各行にスタックされるブロック（GameObject）を子に持つオブジェクトのリスト
    [SerializeField] GameObject[] stackedLineList = new GameObject[column];

    // 消去する行のリスト
    List<GameObject> deleteLineList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // （初期化処理）全てのブロックを空にする
        ResetAllBlocks();
    }

    // （初期化処理）全てのブロックを空にする
    void ResetAllBlocks()
    {
        // 全てのブロックが存在していない状態（false）にする
        for (int j = 0; j < column; j++)
        {
            for (int k = 0; k < row; k++)
            {
                blocks[k, j] = false;
            }
        }
    }

    // 消去対象となる行を確認し、消去・落下させる処理
    public void DeleteAndDropLineProcess()
    {
        // 消去の対象となる行があるか確認する
        bool deleteLineExist = CheckDeleteLineExists();

        if (deleteLineExist)
        {
            // ブロックを消した時の効果音を流す
            soundM.PlaySFX(5);

            perM.StartCoroutine("Coroutine_DeleteLinePerformance");
        }
        else
        {
            // ブロックが落下した時の効果音を流す
            soundM.PlaySFX(4);

            // 終了処理02へ移行
            bG.EndProcess02();
        }
    }

    // 消去する行のリストを作成
    public void CreateDeleteLineList()
    {
        for (int cNum = 0; cNum < column; cNum++)
        {
            // 消去対象の行であった場合、リストに追加
            if (deleteLineBool[cNum])
            {
                deleteLineList.Add(stackedLineList[cNum]);
            }
        }
    }

    public void DeleteLineProcess()
    {
        // 行を消し、落下させる処理
        int count = DeleteAndDropLines();
        // 消去する行のリストを全削除
        deleteLineList.Clear();

        // スコア（内部値）の更新
        sM.UpdateScore(count);
        // スコア（表示画面）の更新
        sM.DisplayScores();
        // ラインを消した時、またはレベルが上がった時、背景スピードや画像を変更する
        sM.ChangeBackground_WhenLineOrLevelIncreased(count);

        // 終了処理02へ移行
        bG.EndProcess02();
    }

    // 消去の対象となる行があるか確認する
    bool CheckDeleteLineExists()
    {
        bool exist = false;

        for (int j = 0; j < column; j++)
        {
            bool fullLine = true;

            for (int k = 0; k < row; k++)
            {
                // 指定行の１列でもブロックが存在しない場合、Falseを代入
                if (!blocks[k, j]) fullLine = false;
            }

            // 指定行の全てにブロックが存在する場合の処理
            if (fullLine)
            {
                // 消去対象行を記録
                deleteLineBool[j] = true;
                // 消去処理を実行させるためのbool
                exist = true;
            } 
        }

        return exist;
    }

    // 行を消し、落下させる処理
    int DeleteAndDropLines()
    {
        // （下から数えて）落下させる行の数を記録
        int deleteLineCount = 0;

        for (int cNum = 0; cNum < column; cNum++)
        {
            if (deleteLineBool[cNum]) 
            {
                // 消去する行の処理
                DeleteLine(cNum);
                // 落下させる行の数を追加
                deleteLineCount++;
            }
            else 
            {
                // 落下させる行の処理
                if (deleteLineCount >= 1) DropLine(cNum, deleteLineCount);
            }
        }

        return deleteLineCount;
    }

    // 消去する行の処理
    void DeleteLine(int cNum)
    {
        // 消去する行のブロックをすべて消去する
        foreach (Transform n in stackedLineList[cNum].transform) Destroy(n.gameObject);

        // 消去判定行のboolの初期化
        deleteLineBool[cNum] = false;
    }

    // 落下させる行の処理
    void DropLine(int cNum, int dLC)
    {
        // 配列情報（blocks[, ]）の更新
        for (int rNum = 0; rNum < row; rNum++)
        {
            // 現在の行数と落下させる行の数の合計が行の総数を越えるかどうか
            if (cNum + dLC < column)
            {
                // 越えない場合
                // 現在の行にあるブロックの有無情報を、落下させる行数分下のブロック情報に上書きする
                blocks[rNum, cNum - dLC] = blocks[rNum, cNum];
            }
            else
            {
                // 越える場合
                // 現在の行の全てのブロック情報をfalseにする
                blocks[rNum, cNum] = false;
            }
        }

        // オブジェクト情報（stackedLineList[]）の更新
        if (cNum + dLC < column)
        {
            List<GameObject> bList = new List<GameObject>();
            foreach (Transform n in stackedLineList[cNum].transform)
            {
                // 移動元の全てのブロックを一度リスト化する
                bList.Add(n.gameObject);
            }

            foreach (GameObject gao in bList)
            {
                // 全てのブロックのY座標を落下させる行数分落とす
                gao.transform.position += new Vector3(0, -dLC, 0);
                // 全てのブロックを移動先となる行オブジェクトの子オブジェクトとして再配置
                gao.transform.parent = stackedLineList[cNum - dLC].transform;
            }
        }
    }

    public bool[,] Blocks
    {
        set { blocks = value; }
        get { return blocks; }
    }
    public GameObject[] StackedLineList
    {
        set { stackedLineList = value; }
        get { return stackedLineList; }
    }

    public List<GameObject> DeleteLineList
    {
        get { return deleteLineList; }
    }
}
