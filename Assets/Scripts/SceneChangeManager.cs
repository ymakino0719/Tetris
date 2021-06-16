using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
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

    // シーンチェンジ（タイトルへ）
    public void ChangeSceneToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
