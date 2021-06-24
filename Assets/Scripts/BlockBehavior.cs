using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    BlockList bL;
    BlockGenerator bG;

    DetectCurrentBlockMovement dCBM;

    PanelManager pM;
    SceneChangeManager sCM;
    ScoreManager sM;
    PerformanceManager_Stage perM;
    InputManager iM;

    SoundManager soundM;

    // 現在動かしているテトリミノのブロックのリスト
    List<GameObject> currentBlockList = new List<GameObject>();

    // ブロック移動のインターバル時間
    float intervalTime;
    // ブロック移動のインターバル時間（初期値）
    float intervalTime_Initial = 1.0f;

    // 横方向キーの累積長押し時間
    float longPressTime_Hor = 0.0f;
    // 縦方向キーの累積長押し時間
    float longPressTime_Ver = 0.0f;
    // 横方向の長押し中の時間係数（加速前）
    float timeCoef_Hor = 0.75f;
    // 縦方向の長押し中の時間係数（加速前）
    float timeCoef_Ver;
    // 縦方向の長押し中の時間係数（加速前）の初期値
    float timeCoef_Ver_Initial = 1.25f;
    // 長押し中の加速係数（加速後）
    float fastMoveCoef = 60.0f;
    // 加速時間閾値
    float maxPressTime = 0.8f;

    Vector3 rightDir = new Vector3(1.0f, 0, 0);
    Vector3 leftDir = new Vector3(-1.0f, 0, 0);
    Vector3 downDir = new Vector3(0, -1.0f, 0);

    // ゲームオーバー条件
    bool gameOver = false;

    // このブロックがテトリミノ「I」のとき
    bool tetrimino_I = false;
    // このブロックがテトリミノ「O」のとき
    bool tetrimino_O = false;

    // ブロックが移動したかどうか（移動先のエリアの確認が必要なため）
    bool moveCurrentBlock = false;

    // 範囲外のブロックが存在し、非表示にしているかどうか
    bool outOfRangeAreaBlocksExist = false;

    void Awake()
    {
        GameObject bM = GameObject.FindWithTag("BlockManager");
        bL = bM.GetComponent<BlockList>();
        bG = bM.GetComponent<BlockGenerator>();

        dCBM = GameObject.FindWithTag("CurrentBlock").GetComponent<DetectCurrentBlockMovement>();

        GameObject uiM = GameObject.FindWithTag("UIManager");
        pM = uiM.GetComponent<PanelManager>();
        sCM = uiM.GetComponent<SceneChangeManager>();
        sM = uiM.GetComponent<ScoreManager>();
        perM = uiM.GetComponent<PerformanceManager_Stage>();
        iM = uiM.GetComponent<InputManager>();

        soundM = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

        // このブロックがテトリミノの「I」あるいは「O」に該当するか確認
        if (this.transform.tag == "Tetrimino_I") tetrimino_I = true;
        if (this.transform.tag == "Tetrimino_O") tetrimino_O = true;

        // インターバル時間の更新
        intervalTime = sM.IntervalTime;
        // 初期値の代入（ブロック移動のインターバル時間が短くなればなるほど、下入力時に一瞬発生するブロック停止時間が短くなる）
        timeCoef_Ver = timeCoef_Ver_Initial * (intervalTime_Initial / intervalTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        // ブロック生成時は「移動した」判定として扱う
        dCBM.MoveCurrentBlock = true;

        // 現在動かしているテトリミノの各ブロックをリストに追加する
        AddCurrentBlockList();

        // ブロックの場所が（上方向の）範囲外エリアかどうか確認
        CheckAreasBlocksExist();

        if (CheckGameOverCondition())
        {
            // ゲームオーバーパネルを表示
            pM.DisplayGameOverPanel();

            // ゲームオーバーのカットインを出す
            perM.StartCoroutine("StartGameOverCutIn");
        }
        else if(!perM.PlayingCutIn)
        {
            // ブロックが自動で下に落ちる処理を実行
            StartCoroutine("FallBlockCoroutine");
        }
        else
        {
            // 開幕のカットイン演出が終わるまで、ブロックを降下させない
            StartCoroutine("WaitForCutInToEndCoroutine");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver && !pM.Paused && !perM.PlayingCutIn)
        {
            // 単押し入力移動の処理
            ShortPressProcess();
            // 長押し入力移動の処理
            LongPressProcess();
        }

        // ブロックが動いたときの処理;
        // ブロックの移動先が（上方向の）範囲外エリアかどうか確認
        if (moveCurrentBlock)
        {
            CheckAreasBlocksExist();
            moveCurrentBlock = false;
        }
    }

    // 開幕のカットイン演出が終わるまで、ブロックを降下させない
    public IEnumerator WaitForCutInToEndCoroutine()
    {
        // カットイン演出が終わるまで待つ
        while (perM.PlayingCutIn) yield return null;

        // ブロックが自動で下に落ちる処理を実行
        StartCoroutine("FallBlockCoroutine");
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
    public IEnumerator FallBlockCoroutine()
    {
        yield return new WaitForSeconds(intervalTime);

        // 下入力中、またはポーズ中は自動で下に落下しない
        while (iM.DownKey_Pressing || pM.Paused) yield return null;

        // ブロックの移動
        SlideBlock(downDir, true);

        // 繰り返し
        StartCoroutine("FallBlockCoroutine");
    }

    // ブロックの移動（下移動中の時のみ bool down は true を受ける）
    void SlideBlock(Vector3 dir, bool down)
    {
        foreach (GameObject gao in currentBlockList)
        {
            Vector3 vec = gao.transform.position;
            // vec + dir : ブロックの移動先のPos
            vec += dir;

            // 下記のいずれかを満たす場合、移動を行わず、処理を終了する
            // ①左右のフィールド外にはみ出した場合
            // ②左右移動時に限り、移動先にブロックが存在する場合
            if (vec.x <= -1.0f || vec.x >= 10.0f || (!down && bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)])) return;

            // 下記のいずれかを満たす場合、移動を行わず、終了処理を実行する
            // ①最下層に到達した場合
            // ②下方向移動時に限り、移動先にブロックが存在する場合
            if (vec.y <= -1.0f || (down && bL.Blocks[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)]))
            {
                // 終了処理
                EndProcess01();
                return;
            }
        }

        // 全てのブロックの移動先が画面外でなくブロックもない場合、移動を実行する
        this.gameObject.transform.Translate(dir, Space.World);

        moveCurrentBlock = true;
        dCBM.MoveCurrentBlock = true;
    }

    // 終了処理 ; 操作中のブロックを落下させ、現在のブロックを削除し次のブロックを生成する処理
    // EndProcess01_消去する行がある場合は、ライン消去演出を挟んでからEndProcess02に移行する（ない場合はノータイムで02に移行）
    void EndProcess01()
    {
        // ブロックのスタック
        StackBlocks();

        // 落下地点として表示されていた透明なブロックを、更新のため削除する
        bG.DestroyBlockGameObject(bG.TransparentBlockParent);

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

            // 各ブロックの回転方向を修正しておく
            currentBlockList[j].transform.eulerAngles = Vector3.up;
        }
    }

    // ブロックが表示されていい場所かを確認する
    void CheckAreasBlocksExist()
    {
        // ①範囲外ブロックがあるかどうか確認
        foreach (GameObject gao in currentBlockList)
        {
            // ブロックの場所が上方向の範囲外エリアだった場合、SpriteRenderer を非表示にする
            if (gao.transform.position.y >= 19.5f)
            {
                gao.GetComponent<SpriteRenderer>().enabled = false;
                outOfRangeAreaBlocksExist = true;
            }
        }

        // ②範囲外ブロックが範囲内に戻ったかどうか確認
        if (outOfRangeAreaBlocksExist)
        {
            bool exist = false;

            foreach (GameObject gao in currentBlockList)
            {
                // ブロックの場所が範囲内に戻った場合、SpriteRenderer を非表示にする
                if (gao.transform.position.y < 19.5f)
                {
                    gao.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    // 範囲外ブロックが1つでもあった場合、trueを返す
                    exist = true;
                }
            }

            // 範囲外ブロックが1つもなかった場合、次から②の処理は（一旦）終了となる
            if (!exist) outOfRangeAreaBlocksExist = false;
        }
    }

    // 単押し入力移動の処理
    void ShortPressProcess()
    {
        // 右ボタンが1回押されたとき
        if (iM.RightKey_Press)
        {
            SlideBlock(rightDir, false);
            iM.RightKey_Press = false;
        }

        // 左ボタンが1回押されたとき
        if (iM.LeftKey_Press)
        {
            SlideBlock(leftDir, false);
            iM.LeftKey_Press = false;
        }

        // 下ボタンが1回押されたとき
        if (iM.DownKey_Press)
        {
            SlideBlock(downDir, true);
            iM.DownKey_Press = false;
        }

        // 上ボタンが1回押されたとき
        if (iM.UpKey_Press)
        {
            // 回転
            RotateBlock();

            // 回転時の効果音を流す
            soundM.PlaySFX(3);

            iM.UpKey_Press = false;
        }
    }

    // 長押し入力移動の処理
    void LongPressProcess()
    {
        if (iM.RightKey_Pressing && !iM.LeftKey_Pressing)
        {
            // 右矢印だけ押されているときの処理
            LongPressMovement(rightDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else if (!iM.RightKey_Pressing && iM.LeftKey_Pressing)
        {
            // 左矢印だけ押されているときの処理
            LongPressMovement(leftDir, ref longPressTime_Hor, ref timeCoef_Hor, false);
        }
        else
        {
            // それ以外の時（両方同時に押されているか、両方押されていない場合）の初期化処理
            longPressTime_Hor = 0.0f;
            timeCoef_Hor = 1.0f;
        }

        if (iM.DownKey_Pressing)
        {
            // 下矢印が押されているときの処理
            LongPressMovement(downDir, ref longPressTime_Ver, ref timeCoef_Ver, true);
        }
        else
        {
            // 押されていないときの初期化処理
            longPressTime_Ver = 0.0f;
            // ブロック移動のインターバル時間が短くなるにつれ、下入力時に一瞬発生するブロック停止時間を短くする
            timeCoef_Ver = timeCoef_Ver_Initial * (intervalTime_Initial / intervalTime);
        }
    }

    // 長押し入力移動の動作
    void LongPressMovement(Vector3 dir, ref float longPressTime, ref float timeCoef, bool down)
    {
        longPressTime += timeCoef * Time.deltaTime;

        if (longPressTime >= maxPressTime)
        {
            // ブロックの移動
            SlideBlock(dir, down);

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

        // テトリミノの「O」の場合回転させないため、終了
        if (tetrimino_O) return;

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
        // テトリミノの「I」は回転を制限
        if(tetrimino_I)
        {
            // テトリミノの「I」は1パターンの回転しかしないようにする
            // 既に回転していたら元に戻す（-90度）
            if (Vector3.Angle(Vector3.up, transform.up) >= 45) transform.Rotate(0, 0, -90.0f);
            else transform.Rotate(0, 0, 90.0f);
        }
        else
        {
            // その他の場合は普通に90度ずつ、計4パターンの回転を実施
            transform.Rotate(0, 0, 90.0f);
        }

        moveCurrentBlock = true;
        dCBM.MoveCurrentBlock = true;
    }

    // 回転できるかどうか
    bool RotatePossibility(Vector3 dir)
    {
        foreach (GameObject gao in currentBlockList)
        {
            // 回転情報
            // ※テトリミノの「I」に限り、回転を制限（もう回転していたら反対方向に回転）
            Quaternion rot = Quaternion.Euler(0, 0, 90.0f);
            if (tetrimino_I && Vector3.Angle(Vector3.up, transform.up) >= 45) rot = Quaternion.Euler(0, 0, -90.0f);
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
