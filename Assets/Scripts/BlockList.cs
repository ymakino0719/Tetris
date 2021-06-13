using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    [SerializeField] GameObject UIManager;
    ScoreManager sM;

    static readonly int row = 10; // 列数
    static readonly int column = 25; // 行数

    bool[,] blocks = new bool[row, column]; // 指定の行数・列数にブロックが存在するか（存在する場合true）
    bool[] deleteLineBool = new bool[column]; // 指定行の消去をするかどうか

    [SerializeField] GameObject[] stackedLineList = new GameObject[column]; // 各行にスタックされるブロック（GameObject）を子に持つオブジェクトのリスト

    void Awake()
    {
        sM = UIManager.GetComponent<ScoreManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetAllBlocks(); // （初期化処理）全てのブロックを空にする
    }

    void ResetAllBlocks() // （初期化処理）全てのブロックを空にする
    {
        for (int j = 0; j < column; j++)
        {
            for (int k = 0; k < row; k++)
            {
                blocks[k, j] = false; // 全てのブロックが存在していない状態にする
            }
        }
    }

    public void DeleteAndDropLineProcess() // 消去対象となる行を確認し、消去・落下させる処理
    {
        bool deleteLineExist = CheckDeleteLineExists(); // 消去の対象となる行があるか確認する

        if (deleteLineExist)
        {
            int count = DeleteAndDropLines(); // 行を消し、落下させる処理
            sM.UpdateScore(count); // スコア（内部値）の更新
            sM.DisplayScores(); // スコア（表示画面）の更新
        }
    }

    bool CheckDeleteLineExists() // 消去の対象となる行があるか確認する
    {
        bool exist = false;

        for (int j = 0; j < column; j++)
        {
            bool fullLine = true;

            for (int k = 0; k < row; k++)
            {
                if (!blocks[k, j]) fullLine = false; // 指定行の１列でもブロックが存在しない場合、Falseを代入
            }

            if (fullLine) // 指定行の全てにブロックが存在する場合の処理
            {
                deleteLineBool[j] = true; // 消去対象行を記録
                exist = true; // 消去処理を実行させるためのbool
            } 
        }

        return exist;
    }

    int DeleteAndDropLines() // 行を消し、落下させる処理
    {
        int deleteLineCount = 0; // （下から数えて）落下させる行の数を記録

        for (int cNum = 0; cNum < column; cNum++)
        {
            if (deleteLineBool[cNum]) 
            {
                DeleteLine(cNum); // 消去する行の処理
                deleteLineCount++; // 落下させる行の数を追加
            }
            else 
            {
                if(deleteLineCount >= 1) DropLine(cNum, deleteLineCount); // 落下させる行の処理
            }
        }

        return deleteLineCount;
    }

    void DeleteLine(int cNum) // 消去する行の処理
    {
        foreach (Transform n in stackedLineList[cNum].transform) Destroy(n.gameObject); // 消去する行のブロックをすべて消去する

        deleteLineBool[cNum] = false; // 消去判定行のboolの初期化
    }

    void DropLine(int cNum, int dLC) // 落下させる行の処理
    {
        for (int rNum = 0; rNum < row; rNum++) // 配列情報（blocks[, ]）の更新
        {
            if (cNum + dLC < column) // 現在の行数と落下させる行の数の合計が行の総数を越えない場合
            {
                blocks[rNum, cNum - dLC] = blocks[rNum, cNum]; // 現在の行にあるブロックの有無情報を、落下させる行数分下のブロック情報に上書きする
            }
            else // 行の総数を越える場合
            {
                blocks[rNum, cNum] = false; // 現在の行の全てのブロック情報をfalseにする
            }
        }

        if (cNum + dLC < column) // オブジェクト情報（stackedLineList[]）の更新
        {
            List<GameObject> bList = new List<GameObject>();
            foreach (Transform n in stackedLineList[cNum].transform)
            {
                bList.Add(n.gameObject); // 移動元の全てのブロックを一度リスト化する
            }

            foreach (GameObject gao in bList)
            {
                gao.transform.position += new Vector3(0, -dLC, 0); // 全てのブロックのY座標を落下させる行数分落とす
                gao.transform.parent = stackedLineList[cNum - dLC].transform; // 全てのブロックを移動先となる行オブジェクトの子オブジェクトとして再配置
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
}
