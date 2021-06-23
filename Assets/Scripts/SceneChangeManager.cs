using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    ScoreManager sM;

    // シーンチェンジ（指定されたシーンへ）
    public void ChangeSceneToSceneName(string sceneName)
    {
        if (sceneName == "Result")
        {
            sM = GetComponent<ScoreManager>();
            SceneManager.sceneLoaded += ResultSceneLoaded;
        }

        SceneManager.LoadScene(sceneName);
    }
    // ゲームのリトライ
    public void RetryStage()
    {
        // ゲーム内時間を再度動かす
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
