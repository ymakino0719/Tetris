using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBehavior : MonoBehaviour
{
    // 背景画像の数
    [SerializeField] static int backgroundNum = 3;
    // 現在の背景画像番号（初期値は0）
    int currentBackgroundNum = 0;

    // スクロールで動かすGameObject
    [SerializeField] GameObject[] sGao = new GameObject[2];

    // スクロールで動かすGameObjectにアタッチされているSprite Renderer
    [SerializeField] SpriteRenderer[] sRen = new SpriteRenderer[2];

    [SerializeField] Sprite[] sSpr = new Sprite[backgroundNum];

    // 背景をスクロールさせるスピードの現在値
    float scrollSpeed = 0.003f;
    // 背景をスクロールさせるスピードの初期値
    float scrollSpeed_Initial = 0.003f;
    // 背景をスクロールさせるスピードの加算値
    float scrollSpeed_AddedValue = 0.003f;
    // 背景のスクロールを開始する位置
    float startLine;
    // 背景のスクロールが終了する位置
    float deadLine;

    // スクロールを行うかどうか
    bool scrolling = true;

    void Awake()
    {
        // 背景を初期画像へ変更
        ChangeBackgroundImages();
    }
    void Start()
    {
        // 初期値の代入
        startLine = sGao[1].transform.localPosition.y;
        deadLine = -startLine;
    }

    void Update()
    {
        // 背景のスクロール
        if(scrolling) ScrollBackground();
    }

    // 背景のスクロール
    void ScrollBackground()
    {
        for(int i = 0; i < sGao.Length; i++)
        {
            // x座標をscrollSpeed分下に動かす
            sGao[i].transform.Translate(0, -scrollSpeed, 0);

            // もし背景のx座標よりdeadLineが大きくなったら、背景をstartLineまで戻す
            if (sGao[i].transform.localPosition.y < deadLine) sGao[i].transform.localPosition = new Vector3(0, startLine, 0);
        }
    }

    // 背景画像番号の更新
    public void UpdateCurrentBackgroundNum()
    {
        currentBackgroundNum++;
        if (currentBackgroundNum >= backgroundNum) currentBackgroundNum = 0;
    }

    public void ChangeBackgroundImages()
    {
        // スクロール画像の差し替え
        for (int j = 0; j < sRen.Length; j++) sRen[j].sprite = sSpr[currentBackgroundNum];
    }

    // 消したラインの数だけ、スクロール速度の加速
    public void AddScrollSpeed(int count)
    {
        // 0の場合は処理を行わない（レベルアップで消去ライン数が0の倍数になったときに発生する）
        if(count != 0)
        {
            scrollSpeed += scrollSpeed_AddedValue * count;
        }
    }

    // スクロール速度を初期値に戻す
    public void ResetScrollSpeed()
    {
        scrollSpeed = scrollSpeed_Initial;
    }
    public bool Scrolling
    {
        set { scrolling = value; }
        get { return scrolling; }
    }
    public float ScrollSpeed
    {
        set { scrollSpeed = value; }
        get { return scrollSpeed; }
    }
    public int CurrentBackgroundNum
    {
        set { currentBackgroundNum = value; }
        get { return currentBackgroundNum; }
    }
}
