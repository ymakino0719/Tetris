using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField] ScoreManager sM;

    // シーンチェンジ（指定されたシーンへ）
    public void ChangeSceneToSceneName(string sceneName)
    {
        if(sceneName == "Result") SceneManager.sceneLoaded += ResultSceneLoaded;

        SceneManager.LoadScene(sceneName);
    }

    // シーンチェンジ（ステージへ）
    public void ChangeSceneToStage()
    {
        SceneManager.LoadScene("Stage");
    }

    // ゲームのリトライ
    public void RetryStage()
    {
        // ゲーム内時間を再度動かす
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // シーンチェンジ（リザルト画面へ）
    public void ChangeSceneToResult()
    {
        // イベントに登録
        SceneManager.sceneLoaded += ResultSceneLoaded;

        SceneManager.LoadScene("Result");
    }

    void ResultSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後にスコアを表示させるスクリプトを取得
        var rD = GameObject.FindWithTag("UIManager").GetComponent<ResultDisplayer>();

        // リザルトデータを渡す
        rD.ResultScore = sM.Score;
        rD.ResultLevel = sM.Level;
        rD.ResultLine = sM.Line;

        // イベントから削除
        SceneManager.sceneLoaded -= ResultSceneLoaded;
    }

    // シーンチェンジ（タイトルへ）
    public void ChangeSceneToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
