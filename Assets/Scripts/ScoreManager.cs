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

    // 背景のコンポーネント
    [SerializeField] BackgroundBehavior bB;

    // レベルアップ
    bool levelUp = false;

    // ブロック移動のインターバル時間
    float intervalTime = 1.0f;
    // ブロック移動のインターバル時間の最小値
    float intervalTime_Min = 0.1f;
    // ブロック移動のインターバル時間の減少係数
    float decreaseCoef = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        // スコアの初期化
        InitializeScores();
        // スコアの表示
        DisplayScores();
    }

    void InitializeScores()
    {
        // スコアの初期化
        score = 0;
        level = 1;
        line = 0;
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

        // レベルが上がったらtrueを代入
        int beforeL = level;
        level = line / 10 + 1;

        // 1レベル上がったとき
        if (beforeL < level) levelUp = true;
    }

    // 各スコアの表示上限の確認
    public int CheckDisplayLimit(int num, int limit)
    {
        if (num <= limit) return num;
        else return limit;
    }

    // ラインを消した時、またはレベルが上がった時、背景スピードや画像を変更する
    public void ChangeBackground_WhenLineOrLevelIncreased(int count)
    {
        // ブロック落下のインターバル時間を減少させる
        DecreaseIntervalTimeOfBlockMovement();

        if (levelUp)
        {
            // レベルアップ時
            // スクロール速度を初期化
            bB.ResetScrollSpeed();

            // 背景画像番号の更新
            bB.UpdateCurrentBackgroundNum();
            // 背景画像の変更
            bB.ChangeBackgroundImages();

            // countの更新（消去ライン数を、レベルアップ時の余剰分のライン数に変更）
            count = line % 10;

            levelUp = false;
        }

        // 消去したラインの数だけスクロール速度を上昇
        bB.AddScrollSpeed(count);
    }

    // ブロック落下のインターバル時間を減少させる
    // ※この関数及び変数をどこに置くか迷ったが、暫定的にScoreManagerに置くことにした
    void DecreaseIntervalTimeOfBlockMovement()
    {
        float a = intervalTime * decreaseCoef;

        // 最小値よりは小さくならないようにする
        intervalTime = (a > intervalTime_Min) ? a : intervalTime_Min;
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
    public float IntervalTime
    {
        set { intervalTime = value; }
        get { return intervalTime; }
    }
}
