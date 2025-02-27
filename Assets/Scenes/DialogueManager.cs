using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public TMP_Text dialogueText;
    public AudioSource systemDialogueSource;
    public AudioSource dialogueSource;
    public BattlePlayer player;
    public Dictionary<string, BattleCharacter> allies = new Dictionary<string, BattleCharacter>();
    public void Init()
    {
        foreach (var character in FindObjectsOfType<BattleCharacter>(true))
        {
            allies[character.name] = character;
        }
    }

    public void StartDialogue(string dialogueName)
    {
        StartCoroutine(PlayDialogue(CSVLoader.Instance.dialogueInfoDict[dialogueName]));
    }

    bool isStopped() 
    {
        var stopped = !dialogueSource.isPlaying || interrupt;
        
        return stopped;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            FinishDialogue();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetInput("space");
        }
    }

    private string waitingKey = "";
    private bool getWaitingKey = false;
    private bool canInterrupt = false;
    private bool interrupt = false;
    void SetWaitingKey(string key)
    {
        waitingKey = key;
        getWaitingKey = false;
        
        
    }
    public void GetInput(string key)
    {
        bool succeed = false;
        if (key == waitingKey)
        {
            succeed = true;
        }

        if (key == "rotate" && waitingKey == "face")
        {
            if (player.currentAxis == allies["hero1"].currentAxis)
            {
                succeed = true;
            }
        }

        if (key == "healHero2" && waitingKey == "healHero1")
        {
            StartCoroutine(waitAndPlaySFX("healHero1"));
        }
        
        

        if (succeed)
        {
            
            getWaitingKey = true;

            if (canInterrupt)
            {
                interrupt = true;
            }
        }
    }

    IEnumerator waitAndPlaySFX(string n)
    {
        yield return new WaitForSeconds(3f);
        playSFXOnScene(n);
    }
    void DoEvent(DialogueInfo info)
    {
        switch (info.otherEvent[0])
        {
            case "move":
                var playerAxis = player.currentAxis;
                var targetAxis = playerAxis+3;
                targetAxis = targetAxis% BattleField.MaxAxis;
                
                StartCoroutine(allies[info.otherEvent[1]].MoveTo(targetAxis,5,3f));
                break;
            case "music":
                MusicManager.Instance.PlayMusic(info.otherEvent[1]);
                break;
            case "hero1PlayMove":
                BattleField.Instance.allies[0].StartWalking();
                break;
            case "hero1StopPlayMove":
                BattleField.Instance.allies[0].StopWalking();
                break;
            case "ambIn":
                foreach (var source in GameObject.Find(info.otherEvent[1]).GetComponentsInChildren<AudioSource>())
                {
                    source.volume = 0;
                    source.DOFade(1, 0.5f);
                    source.Play();
                }
                
                break;
            case "ambOut":
                foreach (var source in GameObject.Find(info.otherEvent[1]).GetComponentsInChildren<AudioSource>())
                {
                    source.volume = 1;
                    source.DOFade(0, 0.5f);
                }   
                break;
        }
    }
    IEnumerator PlayDialogue(DialogueInfo info)
    {
        currentInfo = info;
        if (info.speaker == "system")
        {
            dialogueSource = systemDialogueSource;
        }
        else
        {
            dialogueSource = allies[info.speaker].talkSoundSource;
        }
        dialogueSource.clip = Resources.Load<AudioClip>("audio/dialogue/" + info.id);
        
        if(dialogueSource.clip!=null)
        dialogueSource.Play();
        dialogueText.text = info.text;
        interrupt = false;
        SetWaitingKey(info.wait);
        canInterrupt = info.canInterrupt;

        DoEvent(info);
        
        while (true)
        {
            
            yield return new WaitUntil(isStopped);

            if (!interrupt)
            {
                if (info.wait != null && info.wait.Length > 0 &&getWaitingKey == false)
                {
                    canInterrupt = true;
                    DoAfterLineFinishe( info);
                    yield return new WaitForSeconds(info.delayTime);
                    if(dialogueSource.clip!=null && !interrupt)
                        dialogueSource.Play();
                }
                else
                {
                    DoAfterLineFinishe( info);
                    yield return new WaitForSeconds(info.delayTime);
                    break;
                }
            }
            else
            {
                DoAfterLineFinishe( info);
                yield return new WaitForSeconds(info.delayTime);
                break;
            }
        }

        SetWaitingKey("");

        DoAfterDialogueEvent(info);
        
        info = info.nextDialogue;
        if (info == null)
        {
            dialogueText.text = "";
            Debug.Log("finished");
        }
        else
        {
            yield return StartCoroutine(PlayDialogue(info));
        }
    }

    public void SetBattleDialogue(string battleName)
    {
        dialogueText.text = $"Battle {battleName} is In Progress";
    }

    void DoAfterLineFinishe(DialogueInfo info)
    {
        if (info.afterLine!=null &&info.afterLine.Count>0)
        {
            switch (info.afterLine[0])
            {
                case "sfx":
                    playSFXOnScene(info.afterLine[1]);
                    break;
                    
            }
        }
    }

    public void playSFXOnScene(string n)
    {
        
        GameObject.Find(n).GetComponent<AudioSource>().Play();
    }
    void DoAfterDialogueEvent(DialogueInfo info)
    {
        if (info.eventAfter!=null &&info.eventAfter.Count>0)
        {
            switch (info.eventAfter[0])
            {
                case "startBattle":
                    BattleField.Instance.StartBattle(info.eventAfter[1]);
                    break;
                case "addAlly":
                    BattleField.Instance.AddAlly(info.eventAfter[1]);
                    break;
                    
            }
        }
    }
    
    private DialogueInfo currentInfo;
    void FinishDialogue()
    {
        
        dialogueText.text = "";
        dialogueSource.Stop();
        
        StopAllCoroutines();

        while (currentInfo!=null)
        {
            
            DoAfterDialogueEvent(currentInfo);
            
            currentInfo  = currentInfo.nextDialogue;
        }

            //BattleField.Instance.StartBattle();
            currentInfo = null;
    }
}
