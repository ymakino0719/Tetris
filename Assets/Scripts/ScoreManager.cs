using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int score = 0; // スコア
    int level = 1; // レベル
    int line = 0; // 消したラインの総数

    // 各表示上限値
    int scoreLimit = 99999999;
    int levelLimit = 999;
    int lineLimit = 999;

    // 各UIのゲームオブジェクト
    [SerializeField] GameObject text_Score;
    [SerializeField] GameObject text_Level;
    [SerializeField] GameObject text_Line;

    // 各UIのTextコンポーネント
    Text scoreText;
    Text levelText;
    Text lineText;

    void Awake()
    {
        // 各UIのTextコンポーネントの取得
        scoreText = text_Score.GetComponent<Text>();
        levelText = text_Level.GetComponent<Text>();
        lineText = text_Line.GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayScores(); // スコアの初期化
    }

    public void DisplayScores()
    {
        // 各スコアの表示上限を確認（上限値より上回った場合上限値を返す）
        int dScore = CheckDisplayLimit(score, scoreLimit);
        int dLine = CheckDisplayLimit(line, lineLimit);
        int dLevel = CheckDisplayLimit(level, levelLimit);

        scoreText.text = dScore.ToString("00000000");
        levelText.text = dLevel.ToString("000");
        lineText.text = dLine.ToString("000");
    }

    public void UpdateScore(int count) // スコアの更新
    {
        score += count * 100;
        line += count;
        level = line / 10 + 1;
    }
    public int CheckDisplayLimit(int num, int limit) // 各スコアの表示上限の確認
    {
        if (num <= limit) return num;
        else return limit;
    }

    public int Score
    {
        set { score = value; }
        get { return score; }
    }
    public int Level
    {
        set { level = value; }
        get { return level; }
    }
    public int Line
    {
        set { line = value; }
        get { return line; }
    }
}
