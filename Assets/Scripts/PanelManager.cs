using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    // リザルト画面
    [SerializeField] GameObject gameOverPanel;
    // ポーズ画面
    [SerializeField] GameObject pausedPanel;

    // リザルト時の各スコアを表示する各Textコンポーネント
    [SerializeField] Text resultScoreNum;
    [SerializeField] Text resultLevelNum;
    [SerializeField] Text resultLineNum;

    // 各表示上限値
    int scoreLimit = 99999999;
    int levelLimit = 999;
    int lineLimit = 999;

    // スコアマネージャー
    [SerializeField] ScoreManager sM;
    void Awake()
    {
        // リザルト・ポーズ画面の非表示化
        gameOverPanel.SetActive(false);
        pausedPanel.SetActive(false);
    }

    // ゲームオーバー時に表示するUI
    public void DisplayGameOverPanel()
    {
        // ゲームオーバー時のスコアの更新
        int resultScore = sM.CheckDisplayLimit(sM.Score, scoreLimit);
        resultScoreNum.text = resultScore.ToString();

        // ゲームオーバー時のレベル数の更新
        int resultLevel = sM.CheckDisplayLimit(sM.Level, levelLimit);
        resultLevelNum.text = resultLevel.ToString();

        // ゲームオーバー時のライン数の更新
        int resultLine = sM.CheckDisplayLimit(sM.Line, lineLimit);
        resultLineNum.text = resultLine.ToString();

        // リザルト画面の表示
        gameOverPanel.SetActive(true);
    }

    // ポーズ画面を開く処理
    public void DisplayPausedPanel()
    {
        // ゲーム内時間を止める
        Time.timeScale = 0f;
        // ポーズ画面の表示
        pausedPanel.SetActive(true);
    }

    // ポーズ画面を閉じる（ゲームを再開する）
    public void HidePausedPanel()
    {
        // ゲーム内時間を再度動かす
        Time.timeScale = 1f;
        // ポーズ画面の非表示
        pausedPanel.SetActive(false);
    }
}
