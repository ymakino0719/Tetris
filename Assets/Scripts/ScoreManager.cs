using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // スコア
    int score = 0;
    // レベル
    int level = 1;
    // 消したラインの総数
    int line = 0;

    // 各表示上限値
    int scoreLimit = 99999999;
    int levelLimit = 999;
    int lineLimit = 999;

    // 各UIのTextコンポーネント
    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] Text lineText;

    // Start is called before the first frame update
    void Start()
    {
        // スコアの初期化
        DisplayScores();
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

    // スコアの更新
    public void UpdateScore(int count)
    {
        score += count * 100;
        line += count;
        level = line / 10 + 1;
    }

    // 各スコアの表示上限の確認
    public int CheckDisplayLimit(int num, int limit)
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
