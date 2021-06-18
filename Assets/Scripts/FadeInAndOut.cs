using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInAndOut : MonoBehaviour
{
    // フェードの早さ
    public float fadeSpeed = 0.002f;
    // 透明度
    float alpha;
    // 色
    float red, green, blue;

    // FadePanel
    [SerializeField] GameObject fadePanel;

    // FadePanelのImageコンポーネント
    [SerializeField] Image image;

    // FadeInを実行中かどうか
    bool fadeIn = false;
    // FadeOutを実行中かどうか
    bool fadeOut = false;

    void Awake()
    {
        // パネルの色を取得
        red = image.color.r;
        green = image.color.g;
        blue = image.color.b;

        // FadePanelのRaycastTargetを無効にする（他のボタンをクリックできるようにする）
        image.raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        // FadeIn, FadeOutの実行
        if (fadeIn || fadeOut) FadeProcess();
    }

    // FadeInの開幕処理
    public void StartFadeIn()
    {
        // 既にfadeIn、fadeOut処理を実行中の場合は無効
        if (!fadeIn && !fadeOut)
        {
            // FadePanelのRaycastTargetを有効にする（他のボタンをクリックできないようにする）
            image.raycastTarget = true;
            // 初期状態は透明
            alpha = 0;
            // fadeIn処理の実行
            fadeIn = true;
        }
    }

    // FadeOutの開幕処理
    public void StartFadeOut()
    {
        // 既にfadeIn、fadeOut処理を実行中の場合は無効
        if (!fadeIn && !fadeOut)
        {
            // FadePanelのRaycastTargetを有効にする（他のボタンをクリックできないようにする）
            image.raycastTarget = true;
            // 初期状態は不透明
            alpha = 1;
            // fadeIn処理の実行
            fadeOut = true;
        }
    }

    public void FadeProcess()
    {
        // FadeInの処理の場合、alpha に fadeSpeed を加算していき、1を越えたら1を代入
        // FadeOutの処理の場合は fadeOut を減算していき、0を下回ったら0を代入
        if (fadeIn) alpha = (alpha + fadeSpeed < 1) ? (alpha + fadeSpeed) : 1;
        else alpha = (alpha - fadeSpeed > 0) ? (alpha - fadeSpeed) : 0;

        // 透明度の更新
        image.color = new Color(red, green, blue, alpha);

        // FadeInの処理の場合、透明度が1になったら終了
        // FadeOutの処理の場合、透明度が0になったら終了
        if (fadeIn && alpha >= 1)
        {
            // FadePanelのRaycastTargetを無効にする（他のボタンをクリックできるようにする）
            image.raycastTarget = false;

            fadeIn = false;
        }
        if (fadeOut && alpha <= 0)
        {
            // FadePanelのRaycastTargetを無効にする（他のボタンをクリックできるようにする）
            image.raycastTarget = false;

            fadeOut = false;
        } 
    }
}
