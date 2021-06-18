using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    [SerializeField] FadeInAndOut fIAO;
    [SerializeField] SceneChangeManager sCM;

    // シーン名のstring
    string title = "Title";
    string stage = "Stage";
    string result = "Result";

    // 開幕にフェードアウトするかどうか
    public bool initialFadeOut = false;

    void Start()
    {
        // 開幕のフェードアウトの開始
        if (initialFadeOut) fIAO.StartFadeOut();
    }
    public void SceneChangeToTitleAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(title));
    }

    public void SceneChangeToStageAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(stage));
    }

    public void SceneChangeToResultAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeToStageAfterFadeIn(result));
    }

    IEnumerator Coroutine_SceneChangeToStageAfterFadeIn(string sceneName)
    {
        // フェードインの開始
        fIAO.StartFadeIn();

        yield return new WaitForSeconds(1.0f);

        // シーンチェンジ
        sCM.ChangeSceneToStage(sceneName);
    }
}
