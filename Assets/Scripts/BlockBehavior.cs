using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    BlockList bL;
    BlockGenerator bG;

    GameOverManager gOM;

    List<GameObject> currentBlockList = new List<GameObject>(); // 現在動かしているテトリミノのブロックのリスト

    float intervalTime = 1.0f; // ブロック移動のインターバル時間

    float longPressTime_Hor = 0.0f; // 横方向キーの累積長押し時間
    float longPressTime_Ver = 0.0f; // 縦方向キーの累積長押し時間
    float timeCoef_Hor = 1.0f; // 横方向の長押し中の時間係数（加速前）
    float timeCoef_Ver = 1.0f; // 縦方向の長押し中の時間係数（加速前）
    float fastMoveCoef = 15.0f; // 長押し中の加速係数（加速後）
    float maxPressTime = 0.8f; // 加速時間閾値

    Vector3 rightDir = new Vector3(1.0f, 0, 0);
    Vector3 leftDir = new Vector3(-1.0f, 0, 0);
    Vector3 downDir = new Vector3(0, -1.0f, 0);

    bool gameOver = false; // ゲームオーバー条件
    void Awake()
    {
        GameObject bM = GameObject.FindWithTag("BlockManager");
        bL = bM.GetComponent<BlockList>();
        bG = bM.GetComponent<BlockGenerator>();

        GameObject uiM = GameObject.FindWithTag("UIManager");
        gOM = uiM.GetComponent<GameOverManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddCurrentBlockList(); // 現在動かしているテトリミノの各ブロックをリストに追加する
        if(CheckGameOverCondition())
        {
            gOM.DisplayGameOverUI();
            gameOver = true;
        }
        else StartCoroutine("FallBlockCoroutine"); // ブロックが自動で下に落ちる処理
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver)
        {
            ShortPressProcess(); // 単押し入力移動の処理
            LongPressProcess(); // 長押し入力移動の処理
        }
    }

    void AddCurrentBlockList() // 現在動かしているテトリミノの各ブロックをリストに追加する
    {
        foreach (Transform child in this.gameObject.transform) currentBlockList.Add(child.gameObject);
    }

    bool CheckGameOverCondition()
    {
        for (int j = 0; j < currentBlockList.Count; j++)
        {
            Vector3 vec = currentBlockList[j].transform.position; // 各ブロックの出現位置Pos
            int x = Mathf.RoundToInt(vec.x);
            int y = Mathf.RoundToInt(vec.y);

            if (bL.Blocks[x, y]) // 出現位置にすでにブロックがある場合、ゲームオーバー
            {
                gameOver = true;
                return true; // 各ブロックの移動先Posのブロックの存在をtrueにする
            } 
        }
        return false;
    }
    private IEnumerator FallBlockCoroutine() // ブロックが自動で下に落ちる処理
    {
        yield return new WaitForSeconds(intervalTime);

        while (Input.GetButton("DownKey")) yield return null; // 下入力中は自動で下に落下しない

        SlideBlock(downDir, true); // ブロックの移動
        StartCoroutine("FallBlockCoroutine"); // 繰り返し
    }

    void SlideBlock(Vector3 dir, bool down) // ブロックの移動
    {
        foreach (GameObject gao in currentBlockList)
        {
            Vector3 vec = gao.transform.position;
            vec += dir; // vec:ブロックの移動先のPos

            if (vec.x <= -1.0f || vec.x >= 10.0f) return; // 左右にはみ出した場合、移動を行わず、処理を終了

            if (vec.y <= -1.0f) // 最下層に到達した場合、移動を行わず、スタック処理を実行する
            {
                EndProcess(); // 終了処理
                return;
            }

            if (bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) // 下入力中で且つ移動先にブロックが存在する場合、移動を行わず、スタック処理を実行する
            {
                if (down) EndProcess(); // 終了処理
                return;
            }
        }

        this.gameObject.transform.Translate(dir, Space.World); // 全てのブロックの移動先が画面外でなくブロックもない場合、移動を実行する
    }

    void EndProcess() // 終了処理
    {
        StackBlocks(); // ブロックのスタック

        bG.GenerateCurrentBlock(); // 今のブロックを生成
        bG.GenerateNextBlock(); // 次のブロックを生成
        bL.DeleteAndDropLineProcess(); // 消去対象となる行を確認し、消去・落下させる処理

        Destroy(this.gameObject);
    }

    void StackBlocks() // ブロックのスタック
    {
        for (int j = 0; j < currentBlockList.Count; j++)
        {
            Vector3 vec = currentBlockList[j].transform.position; // 各ブロックの移動先Pos
            int x = Mathf.RoundToInt(vec.x);
            int y = Mathf.RoundToInt(vec.y);

            bL.Blocks[x, y] = true; // 各ブロックの移動先Posのブロックの存在をtrueにする

            currentBlockList[j].transform.parent = bL.StackedLineList[y].transform; // 各ブロックを、対応する行のリストの子オブジェクトとして再配置
        }
    }

    void ShortPressProcess() // 単押し入力移動の処理
    {
        if (Input.GetButtonDown("RightKey"))
        {
            SlideBlock(rightDir, false);
        }

        if (Input.GetButtonDown("LeftKey"))
        {
            SlideBlock(leftDir, false);
        }

        if (Input.GetButtonDown("DownKey"))
        {
            SlideBlock(downDir, true);
        }

        if (Input.GetButtonDown("UpKey"))
        {
            RotateBlock(); // 回転
        }
    }
    void LongPressProcess() // 長押し入力移動の処理
    {
        if (Input.GetButton("RightKey") && !Input.GetButton("LeftKey")) // 右矢印だけ押されているとき
        {
            LongPressMovement(rightDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else if (!Input.GetButton("RightKey") && Input.GetButton("LeftKey")) // 左矢印だけ押されているとき
        {
            LongPressMovement(leftDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else // それ以外の時（両方同時に押されているか、両方押されていない場合）、初期化
        {
            longPressTime_Hor = 0.0f;
            timeCoef_Hor = 1.0f;
        }

        if (Input.GetButton("DownKey")) // 下矢印が押されているとき
        {
            LongPressMovement(downDir, ref longPressTime_Ver, ref timeCoef_Ver, true);
        }
        else // 押されていないとき、初期化
        {
            longPressTime_Ver = 0.0f;
            timeCoef_Ver = 1.0f;
        }
    }
    void LongPressMovement(Vector3 dir, ref float longPressTime, ref float timeCoef, bool down) // 長押し入力移動の動作
    {
        longPressTime += timeCoef * Time.deltaTime;

        if (longPressTime >= maxPressTime)
        {
            SlideBlock(dir, down); // ブロックの移動

            longPressTime = 0.0f; // 初期化
            timeCoef = fastMoveCoef; // 係数を増やし、ブロック移動までの時間を短くする
        }
    }
    void RotateBlock() // 回転処理
    {
        bool normalRotate = false, rightRotate = false, leftRotate = false;

        normalRotate = RotatePossibility(Vector3.zero); // ずらさないで回転を試す
        if (!normalRotate) rightRotate = RotatePossibility(rightDir); // ずらさないで回転が失敗した場合、１マス右にずらして回転を試す
        if (!normalRotate && !rightRotate) leftRotate = RotatePossibility(leftDir); // １マス右にずらしての回転も失敗した場合、１マス左にずらして回転を試す

        if (!normalRotate && !rightRotate && !leftRotate) return; // どの場合も回転できない場合、終了
        else if (rightRotate) transform.position += rightDir; // 右にずらした時に回転できる場合、ブロック全体を右に１マスずらす
        else if (leftRotate) transform.position += leftDir; // 左にずらした時に回転できる場合、ブロック全体を左に１マスずらす

        transform.Rotate(0, 0, 90.0f); // 回転
    }

    bool RotatePossibility(Vector3 dir) // 回転できるかどうか
    {
        foreach (GameObject gao in currentBlockList)
        {
            Quaternion rot = Quaternion.Euler(0, 0, 90.0f); // 回転情報
            Vector3 point = gao.transform.position + dir; // 回転させるブロック（dir分だけずらす）
            Vector3 center = transform.position + dir; // 回転の中心となる位置（dir分だけずらす）
            Vector3 vec = rot * (point - center) + center; // 回転先のブロックの位置
            Vector2 vecInt = new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));

            if (vecInt.x <= -1.0f || vecInt.x >= 10.0f || vecInt.y <= -1.0f) return false; // 回転先のブロックの位置が画面外となった場合、回転不可能のfalseを返す
            else if (bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) return false; // 回転先のブロックの位置に他のブロックが存在する場合、回転不可能のfalseを返す
        }

        return true; // 全てのブロックで回転が可能の時、trueを返す
    }
}
