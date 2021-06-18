using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDisplayer : MonoBehaviour
{
    // リザルトスコア
    int resultScore = 0;
    // リザルトレベル
    int resultLevel = 1;
    // 消したラインのリザルト総数
    int resultLine = 0;

    // 各UIのTextコンポーネント
    [SerializeField] Text resultScoreNum;
    [SerializeField] Text resultLevelNum;
    [SerializeField] Text resultLineNum;
    // Start is called before the first frame update
    void Start()
    {
        DisplayResultScores();
    }

    void DisplayResultScores()
    {
        resultScoreNum.text = resultScore.ToString();
        resultLevelNum.text = resultLevel.ToString();
        resultLineNum.text = resultLine.ToString();
    }

    public int ResultScore
    {
        set { resultScore = value; }
        get { return resultScore; }
    }
    public int ResultLevel
    {
        set { resultLevel = value; }
        get { return resultLevel; }
    }
    public int ResultLine
    {
        set { resultLine = value; }
        get { return resultLine; }
    }
}
