using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleField : Singleton<BattleField>
{
    private float spawnTimer;
    public float spawnTime = 10f;


    public string battleId;
    
    public static float RotateDegree = 45;

    public List<BattleCharacter> enemies;
    public List<BattleCharacter> allies;
    public static int MaxAxis = 7;

    private int goblinMaxNumber = 5;
    private int enemyCount = 0;
    private int enemyKilled = 0;

    public bool isStart = false;

    private FMOD.Studio.EventInstance battlemusic;

    private void Start()
    {
        
        battlemusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/mus_battle_1");
    }

    static public float AxisToDegree(int axis)
    {
        return axis * RotateDegree;
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
        isStart = true;
          this.battleId = id;
        allies[0].Init("hero1",allies[0].currentAxis);

        allies[0].Speak("BattleBegin",true);
        spawnTimer = 5;
        enemyKilled = 0;
        battlemusic.start();
        battlemusic.setVolume(currentVolumn);
    }
    // Start is called before the first frame update

    
    
    IEnumerator afterBattle()
    {
        
        battlemusic.stop(STOP_MODE.ALLOWFADEOUT);
        allies[0].Speak("BattleFinish",true);
        isStart = false;
        yield return new WaitForSeconds(5);
        DialogueManager.Instance.StartDialogue(CSVLoader.Instance.battleInfoDict[battleId].afterBattleEvent[1]);
    }
    public void KillEnemy()
    {
        enemyKilled++;
        if (enemyKilled >= goblinMaxNumber)
        {
            StartCoroutine(afterBattle());
        }
        else if(enemyKilled>= goblinMaxNumber-1)
        {
            allies[0].Speak("BattleOneLeft");
        }
        else if(enemyKilled>= goblinMaxNumber/2)
        {
            allies[0].Speak("BattleMiddle");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            return;
        }

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

         foreach (var ally in allies)
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
             
        var go =Instantiate(Resources.Load<GameObject>("Prefabs/Enemy"), position, Quaternion.identity);
        go.GetComponent<BattleCharacter>().Init("goblin",axis);
        enemies.Add(go.GetComponent<BattleCharacter>());
        
        
        if (Random.Range(0, 100) < 40)
        {
                   
            allies[0].Speak("NewGoblinSpawn");
        }
        
        
        enemyCount++;
    }
}
