using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class DialogueInfo
{
    public string id;
    public string text;
    public string speaker;
    public bool isLoop;
    public List<string> otherEvent;
    public bool isEnd;
    public string wait;
    public List<string> eventAfter;
    public DialogueInfo nextDialogue;

}
public class CSVLoader : Singleton<CSVLoader>
{
    
    public Dictionary<string, DialogueInfo> dialogueInfoDict = new Dictionary<string, DialogueInfo>();

    public void Init()
    {
        var gateInfos = CsvUtil.LoadObjects<DialogueInfo>(GetFileNameWithABTest("dialogue"));
        DialogueInfo lastInfo = null;
        foreach (var info in gateInfos)
        {
            dialogueInfoDict[info.id] = info;
            if (lastInfo != null && !lastInfo.isEnd)
            {
                lastInfo.nextDialogue = info;
                
            }

            lastInfo = info;
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
