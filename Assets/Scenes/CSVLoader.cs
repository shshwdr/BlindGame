using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class DialogueInfo
{
    public string id;
    public string text;
    public string speaker;
    public bool canInterrupt;
    public bool isLoop;
    public List<string> otherEvent;
    public bool isEnd;
    public string wait;
    public List<string> eventAfter;
    public List<string> afterLine;
    public DialogueInfo nextDialogue;
    public float delayTime;
}

public class BattleInfo
{
    public string id;
    public List<string> enemy;
    public float spawnTime;
    public List<string> afterBattleEvent;

}
public class CSVLoader : Singleton<CSVLoader>
{
    
    public Dictionary<string, DialogueInfo> dialogueInfoDict = new Dictionary<string, DialogueInfo>();
    public Dictionary<string, BattleInfo> battleInfoDict = new Dictionary<string, BattleInfo>();

    public void Init()
    {
        var gateInfos = CsvUtil.LoadObjects<DialogueInfo>(GetFileNameWithABTest("dialogue"));
        DialogueInfo lastInfo = null;
        foreach (var info in gateInfos)
        {
            var clip = Resources.Load<AudioClip>("audio/dialogue/" + info.id);
            if (clip == null)
            {
                //Debug.LogError($"dialogue {info.id} not found");
            }
            dialogueInfoDict[info.id] = info;
            if (lastInfo != null && !lastInfo.isEnd)
            {
                lastInfo.nextDialogue = info;
                
            }

            lastInfo = info;
        }
        var battleInfos = CsvUtil.LoadObjects<BattleInfo>(GetFileNameWithABTest("battle"));
        foreach (var info in battleInfos)
        {
            battleInfoDict[info.id] = info;
        }
    }
    string GetFileNameWithABTest(string name)
    {
        // if (ABTestManager.Instance.testVersion != 0)
        // {
        //     var newName = $"{name}_{ABTestManager.Instance.testVersion}";
        //     //check if file in resource exist
        //      
        //     var file = Resources.Load<TextAsset>("csv/" + newName);
        //     if (file)
        //     {
        //         return newName;
        //     }
        // }
        return name;
    }
}
