using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    // ゲームオーバー時に全域に表示する完全に透明なパネル
    [SerializeField] GameObject gameOverPanel;
    // ポーズ画面
    [SerializeField] GameObject pausedPanel;
    // 操作不能パネル
    [SerializeField] GameObject inoperablePanel;

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

    // サウンドマネージャー
    [SerializeField] SoundManager soundM;

    // ポーズ画面中かどうか
    bool paused = false;
    void Awake()
    {
        // ゲームオーバーパネルとポーズ画面の非表示化
        gameOverPanel.SetActive(false);
        pausedPanel.SetActive(false);

        // 操作不能パネルの表示（開幕のボタンによる操作を不可にする）
        inoperablePanel.SetActive(true);
    }

    // ポーズ画面を開く処理
    public void DisplayPausedPanel()
    {
        // ポーズ中にする
        paused = true;
        // ゲーム内時間を止める
        Time.timeScale = 0f;
        // BGMの音量を小さくする
        soundM.ChangeBGMVolume(0.3f);
        // ポーズ画面の表示
        pausedPanel.SetActive(true);
    }

    // ポーズ画面を閉じる（ゲームを再開する）
    public void HidePausedPanel()
    {
        // ポーズを終了する
        paused = false;
        // ゲーム内時間を再度動かす
        Time.timeScale = 1f;
        // BGMの音量を初期値に戻す
        soundM.TurnBackBGMVolumeToInitial();
        // ポーズ画面の非表示
        pausedPanel.SetActive(false);
    }

    // ゲームオーバーパネルを開く処理
    public void DisplayGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    // 操作不能パネルを非表示にする
    public void HideInoperablePanel()
    {
        inoperablePanel.SetActive(false);
    }
    public bool Paused
    {
        set { paused = value; }
        get { return paused; }
    }
}
