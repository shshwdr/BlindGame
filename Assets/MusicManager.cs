using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource currentMusicSource;
    public AudioClip calmDialogueMusic;
    public AudioClip tenseDialogueMusic;
    public AudioClip battleMusic;
    public AudioClip battleMusicStart;
    public AudioClip battleMusicStartTail;
    
    // 淡入淡出的持续时间（秒）
    public float fadeDuration = 2f;

    // 当前正在播放的音频源（如果没有正在播放，则为 null）
    private AudioSource currentSource = null;
    // 用于淡入的新目标音频源
    private AudioSource targetSource = null;

    // 淡出起始音量（当前音频的音量）
    private float currentStartVolume = 1f;
    // 目标音频的最终音量（默认1）
    private float targetFinalVolume = 0.4f;
    // 淡入淡出的计时器
    private float fadeTimer = 0f;
    // 标记是否正在进行淡入淡出过程
    private bool isFading = false;

    public void PlayMusic(string str)
    {
        if (str == "calm")
        {
            FadeTo(calmDialogueMusic);
            targetSource.loop = true;
        }
        else if (str == "tense")
        {
            FadeTo(tenseDialogueMusic);
            targetSource.loop = true;
        }
        else if (str == "battle")
        {
            FadeTo(battleMusicStart);
            targetSource.loop = false;
        }
    }

    private AudioSource anotherSource => (currentSource == audioSource1) ? audioSource2 : audioSource1;

    /// <summary>
    /// 对外接口：传入目标 AudioClip，进行交叉淡入淡出
    /// </summary>
    /// <param name="newClip">目标音频剪辑</param>
    public void FadeTo(AudioClip newClip)
    {
        // if (isFading)
        // {
        //     Debug.LogWarning("当前正在淡入淡出，调用被忽略。");
        //     return;
        // }

        // 根据当前播放的 AudioSource 选择另一个作为目标
        if (currentSource == null)
        {
            // 如果当前没有播放任何音频，则使用 audioSource1 作为目标
            targetSource = audioSource1;
            currentStartVolume = 1f; // 默认起始音量
        }
        else
        {
            // 当前正在播放，则选择另一个 AudioSource
            targetSource =anotherSource;
            currentStartVolume = currentSource.volume;
        }

        // 准备目标 AudioSource：设置新剪辑、音量为0，并开始播放
        targetSource.clip = newClip;
        targetSource.volume = 0f;
        targetSource.Play();

        // 初始化淡入淡出计时器
        fadeTimer = 0f;
        isFading = true;
    }

    private void Update()
    {
        bool setValue = false;
        if (Input.GetKeyDown(KeyCode.W))
        {
            targetFinalVolume += 0.1f;
            if (targetFinalVolume > 1)
            {
                targetFinalVolume = 1;
            }

            setValue = true;
            //battlemusic.setVolume(currentVolumn);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            targetFinalVolume -= 0.1f;
            if (targetFinalVolume < 0)
            {
                targetFinalVolume = 0;
            }
            setValue = true;
            //battlemusic.setVolume(currentVolumn);
        }

        if (setValue)
        {
            audioSource1.volume =  targetFinalVolume;
            audioSource2.volume =  targetFinalVolume;
        }
        
        if (currentSource&& !currentSource.isPlaying && currentSource.clip == battleMusicStart)
        {
            currentSource.loop = true;
            currentSource.clip = battleMusic;
            currentSource.Play();
            // anotherSource.clip = battleMusicStartTail;
            // anotherSource.loop = false;
            // anotherSource.volume = targetFinalVolume; 
            // anotherSource.Play();
        }
        
        if (!isFading)
            return;

        fadeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(fadeTimer / fadeDuration);

        // 如果有当前正在播放的音频，则淡出它
        if (currentSource != null)
        {
            currentSource.volume = Mathf.Lerp(currentStartVolume, 0f, t);
        }

        // 淡入目标音频：从0淡入到目标音量（默认1）
        targetSource.volume = Mathf.Lerp(0f, targetFinalVolume, t);

        // 淡入淡出完成
        if (t >= 1f)
        {
            if (currentSource != null)
            {
                currentSource.Stop();
            }
            // 将目标音频设为当前音频
            currentSource = targetSource;
            targetSource = null;
            isFading = false;
        }

    }
    
}
