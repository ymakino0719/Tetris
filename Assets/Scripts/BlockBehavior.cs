using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    BlockList bL;
    BlockGenerator bG;

    PanelManager pM;

    // 現在動かしているテトリミノのブロックのリスト
    List<GameObject> currentBlockList = new List<GameObject>();

    // ブロック移動のインターバル時間
    float intervalTime = 1.0f;

    // 横方向キーの累積長押し時間
    float longPressTime_Hor = 0.0f;
    // 縦方向キーの累積長押し時間
    float longPressTime_Ver = 0.0f;
    // 横方向の長押し中の時間係数（加速前）
    float timeCoef_Hor = 1.0f;
    // 縦方向の長押し中の時間係数（加速前）
    float timeCoef_Ver = 1.0f;
    // 長押し中の加速係数（加速後）
    float fastMoveCoef = 15.0f;
    // 加速時間閾値
    float maxPressTime = 0.8f;

    Vector3 rightDir = new Vector3(1.0f, 0, 0);
    Vector3 leftDir = new Vector3(-1.0f, 0, 0);
    Vector3 downDir = new Vector3(0, -1.0f, 0);

    // ゲームオーバー条件
    bool gameOver = false;
    void Awake()
    {
        GameObject bM = GameObject.FindWithTag("BlockManager");
        bL = bM.GetComponent<BlockList>();
        bG = bM.GetComponent<BlockGenerator>();

        GameObject uiM = GameObject.FindWithTag("UIManager");
        pM = uiM.GetComponent<PanelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 現在動かしているテトリミノの各ブロックをリストに追加する
        AddCurrentBlockList();

        if (CheckGameOverCondition())
        {
            pM.DisplayGameOverPanel();
            gameOver = true;
        }
        else
        {
            // ブロックが自動で下に落ちる処理
            StartCoroutine("FallBlockCoroutine");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver)
        {
            // 単押し入力移動の処理
            ShortPressProcess();
            // 長押し入力移動の処理
            LongPressProcess();
        }
    }

    void AddCurrentBlockList()
    {
        // 現在動かしているテトリミノの各ブロックをリストに追加する
        foreach (Transform child in this.gameObject.transform) currentBlockList.Add(child.gameObject);
    }

    bool CheckGameOverCondition()
    {
        for (int j = 0; j < currentBlockList.Count; j++)
        {
            // 各ブロックの出現位置Pos
            Vector3 vec = currentBlockList[j].transform.position;
            int x = Mathf.RoundToInt(vec.x);
            int y = Mathf.RoundToInt(vec.y);

            // 出現位置にすでにブロックがある場合、ゲームオーバー
            if (bL.Blocks[x, y])
            {
                gameOver = true;

                // 各ブロックの移動先Posのブロックの存在をtrueにする
                return true;
            } 
        }
        return false;
    }

    // ブロックが自動で下に落ちる処理
    private IEnumerator FallBlockCoroutine()
    {
        yield return new WaitForSeconds(intervalTime);

        // 下入力中は自動で下に落下しない
        while (Input.GetButton("DownKey")) yield return null;

        // ブロックの移動
        SlideBlock(downDir);

        // 繰り返し
        StartCoroutine("FallBlockCoroutine");
    }

    // ブロックの移動
    void SlideBlock(Vector3 dir)
    {
        foreach (GameObject gao in currentBlockList)
        {
            // vec:ブロックの移動先のPos
            Vector3 vec = gao.transform.position;
            vec += dir;

            // 左右にはみ出した場合、移動を行わず、処理を終了
            if (vec.x <= -1.0f || vec.x >= 10.0f) return;

            // 下記のいずれかを満たす場合、移動を行わず、終了処理を実行する
            // ①最下層に到達した場合
            // ②移動先にブロックが存在する場合
            if (vec.y <= -1.0f || bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)])
            {
                // 終了処理
                EndProcess();
                return;
            }
        }

        // 全てのブロックの移動先が画面外でなくブロックもない場合、移動を実行する
        this.gameObject.transform.Translate(dir, Space.World);
    }

    // 終了処理
    void EndProcess()
    {
        // ブロックのスタック
        StackBlocks();

        // 今のブロックを生成
        bG.GenerateCurrentBlock();

        // 落下地点として表示されていた透明なブロックを、更新のため削除する
        bG.DestroyBlockGameObject(bG.TransparentBlockParent);
        // 今の透明ブロックを生成
        bG.GenerateTransparentBlock();

        // 次のブロックとして表示されていた前のブロックを、更新のため削除する
        bG.DestroyBlockGameObject(bG.NextBlockParent);
        // 次のブロックを生成
        bG.GenerateNextBlock();

        // 消去対象となる行を確認し、消去・落下させる処理
        bL.DeleteAndDropLineProcess();
        
        Destroy(this.gameObject);
    }

    // ブロックのスタック
    void StackBlocks()
    {
        for (int j = 0; j < currentBlockList.Count; j++)
        {
            // 各ブロックの移動先Pos
            Vector3 vec = currentBlockList[j].transform.position;
            int x = Mathf.RoundToInt(vec.x);
            int y = Mathf.RoundToInt(vec.y);

            // 各ブロックの移動先Posのブロックの存在をtrueにする
            bL.Blocks[x, y] = true;

            // 各ブロックを、対応する行のリストの子オブジェクトとして再配置
            currentBlockList[j].transform.parent = bL.StackedLineList[y].transform;
        }
    }

    // 単押し入力移動の処理
    void ShortPressProcess()
    {
        if (Input.GetButtonDown("RightKey"))
        {
            SlideBlock(rightDir);
        }

        if (Input.GetButtonDown("LeftKey"))
        {
            SlideBlock(leftDir);
        }

        if (Input.GetButtonDown("DownKey"))
        {
            SlideBlock(downDir);
        }

        if (Input.GetButtonDown("UpKey"))
        {
            // 回転
            RotateBlock();
        }
    }

    // 長押し入力移動の処理
    void LongPressProcess()
    {
        if (Input.GetButton("RightKey") && !Input.GetButton("LeftKey"))
        {
            // 右矢印だけ押されているときの処理
            LongPressMovement(rightDir, ref longPressTime_Hor, ref timeCoef_Hor);
        }
        else if (!Input.GetButton("RightKey") && Input.GetButton("LeftKey"))
        {
            // 左矢印だけ押されているときの処理
            LongPressMovement(leftDir, ref longPressTime_Hor, ref timeCoef_Hor);
        }
        else
        {
            // それ以外の時（両方同時に押されているか、両方押されていない場合）の初期化処理
            longPressTime_Hor = 0.0f;
            timeCoef_Hor = 1.0f;
        }

        if (Input.GetButton("DownKey"))
        {
            // 下矢印が押されているときの処理
            LongPressMovement(downDir, ref longPressTime_Ver, ref timeCoef_Ver);
        }
        else
        {
            // 押されていないときの初期化処理
            longPressTime_Ver = 0.0f;
            timeCoef_Ver = 1.0f;
        }
    }

    // 長押し入力移動の動作
    void LongPressMovement(Vector3 dir, ref float longPressTime, ref float timeCoef)
    {
        longPressTime += timeCoef * Time.deltaTime;

        if (longPressTime >= maxPressTime)
        {
            // ブロックの移動
            SlideBlock(dir);

            // 初期化
            longPressTime = 0.0f;
            // 係数を増やし、ブロック移動までの時間を短くする
            timeCoef = fastMoveCoef;
        }
    }

    // 回転処理
    void RotateBlock()
    {
        bool normalRotate = false, rightRotate = false, leftRotate = false;

        // ずらさないで回転を試す
        normalRotate = RotatePossibility(Vector3.zero);
        // ずらさないで回転が失敗した場合、１マス右にずらして回転を試す
        if (!normalRotate) rightRotate = RotatePossibility(rightDir);
        // １マス右にずらしての回転も失敗した場合、１マス左にずらして回転を試す
        if (!normalRotate && !rightRotate) leftRotate = RotatePossibility(leftDir);

        // どの場合も回転できない場合、終了
        if (!normalRotate && !rightRotate && !leftRotate) return;
        // 右にずらした時に回転できる場合、ブロック全体を右に１マスずらす
        else if (rightRotate) transform.position += rightDir;
        // 左にずらした時に回転できる場合、ブロック全体を左に１マスずらす
        else if (leftRotate) transform.position += leftDir;

        // 回転
        transform.Rotate(0, 0, 90.0f);
    }

    // 回転できるかどうか
    bool RotatePossibility(Vector3 dir)
    {
        foreach (GameObject gao in currentBlockList)
        {
            // 回転情報
            Quaternion rot = Quaternion.Euler(0, 0, 90.0f);
            // 回転させるブロック（dir分だけずらす）
            Vector3 point = gao.transform.position + dir;
            // 回転の中心となる位置（dir分だけずらす）
            Vector3 center = transform.position + dir;
            // 回転先のブロックの位置
            Vector3 vec = rot * (point - center) + center;
            Vector2 vecInt = new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));

            // 回転先のブロックの位置が画面外となった場合、回転不可能のfalseを返す
            if (vecInt.x <= -1.0f || vecInt.x >= 10.0f || vecInt.y <= -1.0f) return false;
            // 回転先のブロックの位置に他のブロックが存在する場合、回転不可能のfalseを返す
            else if (bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]) return false;
        }

        // 全てのブロックで回転が可能の時、trueを返す
        return true;
    }
}
