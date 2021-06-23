using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PerformanceManager_Result : MonoBehaviour
{
    [SerializeField] FadeInAndOut fIAO;
    [SerializeField] SceneChangeManager sCM;

    // シーン名のstring
    string title = "Title";
    string stage = "Stage";

    // 開幕にフェードアウトするかどうか
    public bool initialFadeOut = false;

    void Start()
    {
        // 開幕のフェードアウトの開始
        if (initialFadeOut) fIAO.StartFadeOut();
    }

    public void SceneChangeToTitleAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeAfterFadeIn(title));
    }

    public void SceneChangeToStageAfterFadeIn()
    {
        StartCoroutine(Coroutine_SceneChangeAfterFadeIn(stage));
    }

    IEnumerator Coroutine_SceneChangeAfterFadeIn(string sceneName)
    {
        // フェードインの開始
        fIAO.StartFadeIn();

        yield return new WaitForSeconds(1.0f);

        // シーンチェンジ
        sCM.ChangeSceneToSceneName(sceneName);
    }
}
