using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel; // ゲームオーバー時に表示されるリザルト画面

    [SerializeField] GameObject resultScoreNum; // ゲームオーバー時のスコア点数を表示するテキストオブジェクト
    [SerializeField] GameObject resultLevelNum; // ゲームオーバー時のレベルを表示するテキストオブジェクト
    [SerializeField] GameObject resultLineNum; // ゲームオーバー時の消去ライン数を表示するテキストオブジェクト

    // 各UIのTextコンポーネント
    Text resultScoreText;
    Text resultLevelText;
    Text resultLineText;

    // 各表示上限値
    int scoreLimit = 99999999;
    int levelLimit = 999;
    int lineLimit = 999;

    ScoreManager sM;
    void Awake()
    {
        sM = GetComponent<ScoreManager>();

        // 各UIのTextコンポーネントの取得
        resultScoreText = resultScoreNum.GetComponent<Text>();
        resultLevelText = resultLevelNum.GetComponent<Text>();
        resultLineText = resultLineNum.GetComponent<Text>();

        gameOverPanel.SetActive(false); // リザルト画面の非表示化
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayGameOverUI() // ゲームオーバー時に表示するUI
    {
        int resultScore = sM.CheckDisplayLimit(sM.Score, scoreLimit); // ゲームオーバー時のスコアの更新
        resultScoreText.text = resultScore.ToString();

        int resultLevel = sM.CheckDisplayLimit(sM.Level, levelLimit); // ゲームオーバー時のレベル数の更新
        resultLevelText.text = resultLevel.ToString();

        int resultLine = sM.CheckDisplayLimit(sM.Line, lineLimit); // ゲームオーバー時のライン数の更新
        resultLineText.text = resultLine.ToString();

        gameOverPanel.SetActive(true); // リザルト画面の表示
    }

    ///////////////////////////////////////////////////////////////////////
    /////////////////////////// シーンチェンジ ////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    public void RetryStage() // ゲームのリトライ
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ChangeSceneToTitle() // シーンチェンジ（タイトルへ）
    {
        SceneManager.LoadScene("Title");
    }
}
