using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////
    /////////////////////////// シーンチェンジ ////////////////////////////
    ///////////////////////////////////////////////////////////////////////

    public void ChangeSceneToStage() // シーンチェンジ（ステージへ）
    {
        SceneManager.LoadScene("Stage");
    }
}
