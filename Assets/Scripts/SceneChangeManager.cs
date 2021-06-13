using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public void ChangeSceneToStage() // シーンチェンジ（ステージへ）
    {
        SceneManager.LoadScene("Stage");
    }
    public void RetryStage() // ゲームのリトライ
    {
        Time.timeScale = 1f; // ゲーム内時間を再度動かす
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ChangeSceneToTitle() // シーンチェンジ（タイトルへ）
    {
        SceneManager.LoadScene("Title");
    }
}
