using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void DisplayGameOverUI()
    {
        int resultScore = sM.CheckDisplayLimit(sM.Score, scoreLimit); // リザルトスコアの更新
        resultScoreText.text = resultScore.ToString("00000000");

        int resultLevel = sM.CheckDisplayLimit(sM.Level, levelLimit); // リザルトスコアの更新
        resultScoreText.text = resultLevel.ToString("000");

        int resultLine = sM.CheckDisplayLimit(sM.Line, lineLimit); // リザルトスコアの更新
        resultScoreText.text = resultLine.ToString("000");

        gameOverPanel.SetActive(true); // リザルト画面の表示
    }
}
