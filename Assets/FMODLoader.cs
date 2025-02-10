using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FMODUnity;

public class FMODLoader : MonoBehaviour
{
    // 可以通过 Inspector 指定要加载的银行名称和对应路径
    public string[] banksToLoad = { "Master", "Master.strings" };
    // 主场景名称（加载完银行后跳转到该场景）
    public string mainSceneName = "MainScene";

    IEnumerator Start()
    {
        // 等待一帧，确保 WebGL 平台初始化完毕
        yield return new WaitForEndOfFrame();

        // 循环加载指定的银行
        foreach (string bank in banksToLoad)
        {
            // 加载银行文件（同步加载）
            RuntimeManager.LoadBank(bank);
            // 如果需要等待银行加载完成，可以考虑添加适当延时或使用异步加载方案
            // 这里简单地等待 0.5 秒
            yield return new WaitForSeconds(0.5f);
        }

        // 等待所有银行加载完毕
        // 你也可以在这里检查 RuntimeManager.BankLoadStatus 之类的状态

        // 完成 FMOD 初始化后，切换到主场景
        SceneManager.LoadScene(mainSceneName);
    }
}