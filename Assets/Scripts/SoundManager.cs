using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // AudioSource（BGMとSFXそれぞれで作成、取得する）
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    [SerializeField] static int sfxNum = 6;
    [SerializeField] AudioClip[] sfx = new AudioClip[sfxNum];

    // sfx[sfxNum] 内訳

    // カットインの効果音
    // 0 : go
    // 1 : levelUp
    // 2 : gameOver

    // ブロック回りの効果音
    // 3 : rotateBlock
    // 4 : putBlock
    // 5 : deleteLines

    // BGMの音量を小さくし始めるときのbool
    bool decreaseBGMVolume = false;
    // 音量減衰係数
    float decreaseCoef = 0.96f;
    // 最低音量
    float minVolume = 0.01f;

    // BGM音量の初期値
    float initialBGMVol;

    void Awake()
    {
        initialBGMVol = bgmSource.volume;
    }

    void FixedUpdate()
    {
        if (decreaseBGMVolume) DecreasingBGMVolume();
    }

    public void PlayBGM()
    {
        bgmSource.Play();
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    void DecreasingBGMVolume()
    {
        if(bgmSource.volume >= minVolume)
        {
            // 最低音量を下回るまで減衰係数をかけ続ける
            bgmSource.volume *= decreaseCoef;
        }
        else
        {
            // 減衰処理の終了
            bgmSource.volume = 0;
            bgmSource.Stop();
            decreaseBGMVolume = false;
        }
    }

    // BGM音量を一時的に変更する
    public void ChangeBGMVolume(float mag)
    {
        bgmSource.volume *= mag;
    }

    // BGM音量を初期値に戻す
    public void TurnBackBGMVolumeToInitial()
    {
        bgmSource.volume = initialBGMVol;
    }

    public void PlaySFX(int num)
    {
        sfxSource.PlayOneShot(sfx[num]);
    }
    public bool DecreaseBGMVolume
    {
        set { decreaseBGMVolume = value; }
        get { return decreaseBGMVolume; }
    }
}
