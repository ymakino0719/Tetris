using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PerformanceManager_Stage : MonoBehaviour
{
    [SerializeField] FadeInAndOut fIAO;
    [SerializeField] SceneChangeManager sCM;
    [SerializeField] SoundManager soundM;
    [SerializeField] PanelManager pM;
    [SerializeField] BlockList bL;

    // カットイン
    [SerializeField] GameObject ready;
    [SerializeField] GameObject go;
    [SerializeField] GameObject levelUp;
    [SerializeField] GameObject gameOver01;
    [SerializeField] GameObject gameOver02;

    // パーティクル
    // ブロックを消したときに生成する星のパーティクル
    [SerializeField] GameObject starParticle;

    // シーン名のstring
    string title = "Title";
    string stage = "Stage";
    string result = "Result";

    // 開幕にフェードアウトするかどうか
    public bool initialFadeOut = false;

    // カットインの最中かどうか（開幕はカットインがあるためtrue）
    bool playingCutIn = true;

    // 消去する行の全てのブロックを縮める
    bool shrinkBlocks = false;
    // 縮小係数
    float shrinkCoef = 0.83f;
    // 縮小最小サイズ（Y軸）
    float minSizeY = 0.02f;
    // 行のデフォルトサイズ（Y軸）
    float defaultRowSizeY = 1.0f;
    void Start()
    {
        // 開幕のフェードアウトの開始
        if (initialFadeOut) fIAO.StartFadeOut();

        // （開幕処理）全カットインを非表示にしておく
        ready.SetActive(false);
        go.SetActive(false);
        levelUp.SetActive(false);
        gameOver01.SetActive(false);
        gameOver02.SetActive(false);

        // ReadyとGoのカットインを出す
        StartCoroutine("StartReadyAndGoCutIn");
    }
    void FixedUpdate()
    {
        // 消去する行が持つ全ての子オブジェクトの縦幅を縮める
        if(shrinkBlocks) ShrinkDeleteLineBlocks();
    }

    public void SceneChangeToTitleAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(title));
    }

    public void SceneChangeToStageAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(stage));
    }

    public void SceneChangeToResultAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(result));
    }

    // ステージを再度読み込む
    public void RetryStageAfterFadeIn()
    {
        StartCoroutine("Coroutine_RetryStageAfterFadeIn");
    }

    IEnumerator Coroutine_SceneChangeToStageAfterFadeIn(string sceneName)
    {
        // フェードインの開始
        fIAO.StartFadeIn();

        yield return new WaitForSeconds(1.0f);

        // シーンチェンジ
        sCM.ChangeSceneToSceneName(sceneName);
    }

    IEnumerator Coroutine_RetryStageAfterFadeIn()
    {
        // 時間を戻す
        Time.timeScale = 1f;

        // フェードインの開始
        fIAO.StartFadeIn();

        yield return new WaitForSeconds(1.0f);

        // Stageシーンの再読み込み
        sCM.RetryStage();
    }

    // カットイン処理；Readyの文字の後にGoを出す。Goが出たらブロックが操作できるように
    IEnumerator StartReadyAndGoCutIn()
    {
        yield return new WaitForSeconds(0.5f);

        ready.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        ready.SetActive(false);
        go.SetActive(true);

        // Goの効果音を流す
        soundM.PlaySFX(0);

        yield return new WaitForSeconds(0.9f);

        go.SetActive(false);

        // 開幕の操作不能パネルを非表示にする
        pM.HideInoperablePanel();

        // BGMを流す
        soundM.PlayBGM();

        // ブロック操作を可能にする
        playingCutIn = false;
    }

    // カットイン処理；レベルアップ時にLevelUpの文字を出す（※このカットイン中はブロック操作が可能）
    public IEnumerator StartLevelUpCutIn()
    {
        levelUp.SetActive(true);

        // LevelUpの効果音を流す
        soundM.PlaySFX(1);

        yield return new WaitForSeconds(1.4f);

        levelUp.SetActive(false);
    }

    // カットイン処理；GameOverの文字を出す
    public IEnumerator StartGameOverCutIn()
    {
        playingCutIn = true;

        gameOver01.SetActive(true);

        // GameOverの効果音を流す
        soundM.PlaySFX(2);

        // BGMを徐々に小さくする処理を開始する
        soundM.DecreaseBGMVolume = true;

        yield return new WaitForSeconds(1.0f);

        gameOver01.SetActive(false);
        gameOver02.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // 何らかのキーが押されたらリザルト画面へ移動する
        while (!Input.anyKeyDown) yield return null;

        // リザルト画面へシーンチェンジする
        SceneChangeToResultAfterFadeIn();
    }

    public IEnumerator Coroutine_DeleteLinePerformance()
    {
        bL.CreateDeleteLineList();
        shrinkBlocks = true;

        while(shrinkBlocks) yield return null;

        bL.DeleteLineProcess();
    }

    // 消去する行が持つ全ての子オブジェクトの縦幅を縮める
    void ShrinkDeleteLineBlocks()
    {
        foreach(GameObject gao in bL.DeleteLineList)
        {
            Vector3 scale = gao.transform.localScale;

            scale.y *= shrinkCoef;

            if (scale.y >= minSizeY)
            {
                // 最小サイズを下回らない場合、処理を続行
                gao.transform.localScale = scale;
            }
            else
            {
                // 最小サイズを下回っている場合、縮小ループ処理を終了
                shrinkBlocks = false;
            }
        }

        // 縮小処理が終了したとき
        if(!shrinkBlocks)
        {
            foreach (GameObject gao in bL.DeleteLineList)
            {
                // 行のGameObjectのサイズを元に戻す
                Vector3 defaultSize = new Vector3(1, defaultRowSizeY, 1);
                gao.transform.localScale = defaultSize;
            }
        }
    }

    // ラインを消したときにパーティクルを生成する
    public void GenerateParticlesWhenLineDeletes()
    {
        foreach (GameObject gao in bL.DeleteLineList)
        {
            // 1行につきパーティクルを左右それぞれ2つ生成
            GameObject leftStar  = Instantiate(starParticle) as GameObject;
            GameObject rightStar = Instantiate(starParticle) as GameObject;

            // パーティクルの位置を補正
            leftStar.transform.position  = gao.transform.position + new Vector3(-0.5f, -0.5f, -9.5f);
            rightStar.transform.position = gao.transform.position + new Vector3(9.5f, -0.5f, -9.5f);

            // パーティクルの向きを補正
            leftStar.transform.eulerAngles  = new Vector3(0, 0, 135.0f);
            rightStar.transform.eulerAngles = new Vector3(0, 0, -45.0f);
        }
    }

    public bool PlayingCutIn
    {
        set { playingCutIn = value; }
        get { return playingCutIn; }
    }
}
