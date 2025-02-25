using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 需要持续播放的音频
    public AudioSource persistentAudio;

    void Start()
    {
        // 确保这个音频在全局暂停时依然播放
        persistentAudio.ignoreListenerPause = true;
        persistentAudio.loop = true;
        //persistentAudio.Play();
    }

    void Update()
    {
        // 按 P 键切换暂停状态，仅作为示例
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
                persistentAudio.Stop();
            }
            else
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                persistentAudio.Play();
            }
        }
    }
}