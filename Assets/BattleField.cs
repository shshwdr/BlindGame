using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Singleton<BattleField>
{
    private float spawnTimer;
    public float spawnTime = 10f;

    public List<BattleCharacter> enemies;
    public List<BattleCharacter> allies;
    public static int MaxAxis = 3;

    private int goblinMaxNumber = 5;
    private int enemyCount = 0;
    private int enemyKilled = 0;

    public bool isStart = false;
    static public float AxisToDegree(int axis)
    {
        return axis * 30;
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

    public void StartBattle()
    {
        isStart = true;
        allies[0].Init("hero1",allies[0].currentAxis);

        allies[0].Speak("BattleBegin",true);
        spawnTime = 5;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void KillEnemy()
    {
        enemyKilled++;
        if (enemyKilled >= goblinMaxNumber)
        {
            allies[0].Speak("BattleFinish",true);
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
        spawnTimer -= Time.deltaTime;
         if (spawnTimer <= 0 && enemyCount< goblinMaxNumber)
         {
             spawnTimer = spawnTime;

             int axis = Random.Range(-3, 4);
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

         foreach (var ally in allies)
         {
             ally.UpdateBattle(Time.deltaTime);
         }
         foreach (var ally in enemies)
         {
             ally.UpdateBattle(Time.deltaTime);
         }
    }
}
