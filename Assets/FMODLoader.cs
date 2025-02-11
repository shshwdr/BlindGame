using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class FMODLoader : MonoBehaviour
{
    public string[] banksToLoad = { "Master", "Master.strings" };
    public string mainSceneName = "MainScene";

    IEnumerator Start()
    {
        // 等待一帧，确保平台初始化完成
        yield return new WaitForEndOfFrame();

        // 用来跟踪所有银行的加载状态
        List<Bank> loadedBanks = new List<Bank>();

        // 异步加载每个银行
        foreach (string bankName in banksToLoad)
        {
            Bank bank;
            RuntimeManager.StudioSystem.loadBankFile(bankName, LOAD_BANK_FLAGS.NORMAL, out bank);

            if (bank.isValid())
            {
                loadedBanks.Add(bank);
            }
            else
            {
                Debug.LogError($"Failed to load bank: {bankName}");
            }
        }

        // 检查所有银行是否加载完成
        bool allBanksLoaded = false;
        while (!allBanksLoaded)
        {
            allBanksLoaded = true;
            foreach (var bank in loadedBanks)
            {
                if (!IsBankLoaded(bank))
                {
                    allBanksLoaded = false;
                    break;
                }
            }

            if (!allBanksLoaded)
            {
                yield return null; // 每帧检查一次
            }
        }

        Debug.Log("All FMOD banks loaded successfully!");

        // 所有银行加载完成后切换到主场景
        SceneManager.LoadScene(mainSceneName);
    }

    // 检查银行是否加载完成的辅助方法
    bool IsBankLoaded(Bank bank)
    {
        bank.getLoadingState(out LOADING_STATE state);
        return state == LOADING_STATE.LOADED;
    }
}