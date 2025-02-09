using UnityEngine;

public class AudioDirectionFilter : MonoBehaviour
{
    // 参考玩家位置和前向向量
    public Transform playerTransform;
    
    // AudioLowPassFilter 组件
    private AudioLowPassFilter[] lowPassFilters;
    
    // 调整参数
    public float minCutoff = 500f;  // 当声音在背后时的最低 cutoff（单位 Hz）
    public float maxCutoff = 22000f; // 当声音在正前方时的最高 cutoff（基本上无滤波）

    void Start()
    {
        lowPassFilters = GetComponentsInChildren<AudioLowPassFilter>();
        playerTransform = DialogueManager.Instance.player.transform;
    }

    void Update()
    {
        if (playerTransform == null)
            return;
        
        // 获取声音源位置和玩家前向
        Vector3 soundPos = transform.position;
        Vector3 playerPos = playerTransform.position;
        Vector3 playerForward = playerTransform.forward;
        
        // 声音源方向向量
        Vector3 direction = (soundPos - playerPos).normalized;
        
        // 计算声音方向与玩家正前方的点积
        float dot = Vector3.Dot(playerForward, direction);
        
        // dot 范围为 -1 到 1，当 dot = -1 时，声音完全在背后；dot = 1 时完全在前方
        // 我们可以根据 dot 值来插值 cutoff 频率
        // 当 dot 小于 0（背后）时，使用较低 cutoff；当 dot 接近1时，使用较高 cutoff
        float t = Mathf.Clamp01((dot + 1f) * 0.5f); // 将 -1~1 映射到 0~1
        // 例如，我们希望当 t 越低时，cutoff 越低；当 t 越高时，cutoff 越高
        float cutoff = Mathf.Lerp(minCutoff, maxCutoff, t);
        
        foreach (AudioLowPassFilter lowPassFilter in lowPassFilters)
        {
            if (lowPassFilter.enabled)
            {
                lowPassFilter.cutoffFrequency = cutoff;
            }
        }
       // lowPassFilter.cutoffFrequency = cutoff;
    }
}