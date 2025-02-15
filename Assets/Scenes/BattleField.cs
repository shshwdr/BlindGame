using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BattleField : Singleton<BattleField>
{
    private float spawnTimer;
    public float spawnTime = 10f;


    public string battleId;
    private BattleInfo battleInfo;
    public static float RotateDegree = 45;

    public List<BattleCharacter> enemies;
    public List<BattleCharacter> currentAllies;
    public List<BattleCharacter> allies;
    public BattleCharacter healer;
    public static int MaxAxis = 7;

    private int goblinMaxNumber = 5;
    private int enemyCount = 0;
    private int enemyKilled = 0;

    public bool isStart = false;
    private int lastJoinCharacter = 0;
    
    
    List<string> enemiesList = new List<string>();
    
    private FMOD.Studio.EventInstance battlemusic;
    

    public void AddAlly(string key)
    {
        var ally = allies.Find(x => x.name == key);
        ally.gameObject.SetActive(true);
        currentAllies.Add(ally);
        lastJoinCharacter = currentAllies.Count-1;
    }
    private void Start()
    {
        
        battlemusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/mus_battle_1");
    }

    static public float AxisToDegree(int axis)
    {
        return axis * RotateDegree;
    }

    public BattleCharacter GetCharacter()
    {
        if (lastJoinCharacter < currentAllies.Count)
        {
            return currentAllies[lastJoinCharacter];
        }
        else
        {
            return currentAllies.RandomItem();
        }
    }
    
    public static Vector3 GetLocation(int axis, float distance)
    {
        // Convert degree to radians
        float radians = AxisToDegree(axis) * Mathf.Deg2Rad;

        // Calculate x and z positions based on the angle and distance
        float x = Mathf.Sin(radians) * distance;
        float z = Mathf.Cos(radians) * distance;

        // Return the position with y set to 0 (2D plane)
        return new Vector3(x, 0, z);
    }

    private float currentVolumn = 0.4f;
    public void StartBattle(string id)
    {
        
        battleInfo = CSVLoader.Instance.battleInfoDict[id];
        enemiesList.Clear();
        for(int i  = 0;i< battleInfo.enemy.Count; i+=2)
        {
            var enemyName = battleInfo.enemy[i];
            var enemyCount = int.Parse(battleInfo.enemy[i + 1]);
            for (int j = 0; j < enemyCount; j++)
            {
                enemiesList.Add(enemyName);
            }
     
        }
        enemiesList.Shuffle();
        goblinMaxNumber = enemiesList.Count;
        
        isStart = true;
          this.battleId = id;

          foreach (var currentAlly in currentAllies)
          {
              currentAlly.Init(currentAlly.name,currentAlly.currentAxis);
          }



     GetCharacter().Speak("BattleBegin",true);
 
        spawnTime = battleInfo.spawnTime;
        spawnTimer = 5;
        //goblinMaxNumber = int.Parse( battleInfo.enemy[1]);
        enemyKilled = 0;
        battlemusic.start();
        battlemusic.setVolume(currentVolumn);
    }
    // Start is called before the first frame update


    IEnumerator winBattle(bool skip = false)
    {
        afterBattle();
        GetCharacter().Speak("BattleFinish",true);
        isStart = false;
        yield return new WaitForSeconds(skip?0:5);
        DialogueManager.Instance.StartDialogue(CSVLoader.Instance.battleInfoDict[battleId].afterBattleEvent[1]);
    }
    void afterBattle()
    {

        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
        
        battlemusic.stop(STOP_MODE.ALLOWFADEOUT);
        lastJoinCharacter++;
    }
    public void KillEnemy()
    {
        enemyKilled++;
        if (enemyKilled >= goblinMaxNumber)
        {
            WinBattle(false);
        }
        else if(enemyKilled>= goblinMaxNumber-1)
        {
            GetCharacter().Speak("BattleOneLeft");
        }
        else if(enemyKilled>= goblinMaxNumber/2)
        {
            GetCharacter().Speak("BattleMiddle");
        }
    }

    public void LoseBattle(bool skip)
    {
        
        StartCoroutine(loseBattle(skip));
    }
    
    IEnumerator loseBattle(bool skip = false)
    {
        afterBattle();
        isStart = false;
        yield return new WaitForSeconds(skip?0:5);
        DialogueManager.Instance.StartDialogue(CSVLoader.Instance.battleInfoDict[battleId].afterBattleEvent[1]);
    }
    public void WinBattle(bool skip)
    {
        
        StartCoroutine(winBattle(skip));
    }

    public void RestartBattle()
    {
        afterBattle();
        StartBattle(battleId);
    }
    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            return;
        }

        if (BattleField.Instance.allies.Count == 0)
        {
            //all dead
            LoseBattle(false);
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            WinBattle(true);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            RestartBattle();
        }

        DialogueManager.Instance.SetBattleDialogue(battleInfo.id);

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentVolumn += 0.1f;
            if (currentVolumn > 1)
            {
                currentVolumn = 1;
            }
            battlemusic.setVolume(currentVolumn);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
             currentVolumn -= 0.1f;
             if (currentVolumn < 0)
             {
                 currentVolumn = 0;
             }
             battlemusic.setVolume(currentVolumn);
        }
        
        spawnTimer -= Time.deltaTime;
         if (spawnTimer <= 0 && enemyCount< goblinMaxNumber)
         {
             spawnTimer = spawnTime;



             CreateEnemy();

         }

         foreach (var ally in currentAllies)
         {
             
             ally.UpdateBattle(Time.deltaTime);
         }
         foreach (var ally in enemies)
         {
             ally.UpdateBattle(Time.deltaTime);
         }
    }


    public void CreateEnemy()
    {
        int axis = Random.Range(0, MaxAxis+1);
        var position = GetLocation(axis, 10);
             
        var go =Instantiate(Resources.Load<GameObject>("Prefabs/"+enemiesList[0]), position, Quaternion.identity);
        go.GetComponent<BattleCharacter>().Init(enemiesList[0],axis);
        enemiesList.RemoveAt(0);
        enemies.Add(go.GetComponent<BattleCharacter>());
        
        
        if (Random.Range(0, 100) < 40)
        {
                   
            currentAllies.RandomItem().Speak("NewGoblinSpawn");
        }
        
        
        enemyCount++;
    }
}
