using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PerformanceManager : MonoBehaviour
{
    [SerializeField] FadeInAndOut fIAO;
    [SerializeField] SceneChangeManager sCM;

    // カットイン
    [SerializeField] GameObject ready;
    [SerializeField] GameObject go;
    [SerializeField] GameObject levelUp;
    [SerializeField] GameObject gameOver01;
    [SerializeField] GameObject gameOver02;

    // シーン名のstring
    string title = "Title";
    string stage = "Stage";
    string result = "Result";

    // 開幕にフェードアウトするかどうか
    public bool initialFadeOut = false;

    // カットインの最中かどうか（開幕はカットインがあるためtrue）
    bool playingCutIn = true;

    void Start()
    {
        // 開幕のフェードアウトの開始
        if (initialFadeOut) fIAO.StartFadeOut();
        
        // 現在のシーンが"Stage"のとき
        if (SceneManager.GetActiveScene().name == stage)
        {
            // （開幕処理）全カットインを非表示にしておく
            ready.SetActive(false);
            go.SetActive(false);
            levelUp.SetActive(false);
            gameOver01.SetActive(false);
            gameOver02.SetActive(false);

            // ReadyとGoのカットインを出す
            StartCoroutine("StartReadyAndGoCutIn");
        }
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

    IEnumerator Coroutine_SceneChangeToStageAfterFadeIn(string sceneName)
    {
        // フェードインの開始
        fIAO.StartFadeIn();

        yield return new WaitForSeconds(1.0f);

        // シーンチェンジ
        sCM.ChangeSceneToStage(sceneName);
    }

    // カットイン処理；Readyの文字の後にGoを出す。Goが出たらブロックが操作できるように
    IEnumerator StartReadyAndGoCutIn()
    {
        yield return new WaitForSeconds(0.3f);

        ready.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        ready.SetActive(false);
        go.SetActive(true);

        yield return new WaitForSeconds(0.9f);

        go.SetActive(false);

        // ブロック操作を可能にする
        playingCutIn = false;
    }

    // カットイン処理；レベルアップ時にLevelUpの文字を出す（※このカットイン中はブロック操作が可能）
    public IEnumerator StartLevelUpCutIn()
    {
        levelUp.SetActive(true);

        yield return new WaitForSeconds(1.4f);

        levelUp.SetActive(false);
    }

    // カットイン処理；GameOverの文字を出す
    public IEnumerator StartGameOverCutIn()
    {
        playingCutIn = true;

        gameOver01.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        gameOver01.SetActive(false);
        gameOver02.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        // 何らかのキーが押されたらリザルト画面へ移動する
        while (!Input.anyKeyDown) yield return null;

        Debug.Log("end");

        // リザルト画面へシーンチェンジする
        SceneChangeToResultAfterFadeIn();
    }

    public bool PlayingCutIn
    {
        set { playingCutIn = value; }
        get { return playingCutIn; }
    }
}
