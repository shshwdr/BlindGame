using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Singleton<BattleField>
{
    private float spawnTimer;
    private float spawnTime = 10f;

    public List<BattleCharacter> enemies;
    public List<BattleCharacter> allies;
    public static int MaxAxis = 3;
    static public float AxisToDegree(int axis)
    {
        return axis * 30;
    }
    public Vector3 GetLocation(int axis, float distance)
    {
        // Convert degree to radians
        float radians = AxisToDegree(axis) * Mathf.Deg2Rad;

        // Calculate x and z positions based on the angle and distance
        float x = Mathf.Sin(radians) * distance;
        float z = Mathf.Cos(radians) * distance;

        // Return the position with y set to 0 (2D plane)
        return new Vector3(x, 0, z);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
         if (spawnTimer <= 0)
         {
             spawnTimer = spawnTime;

             var position = GetLocation(Random.Range(-3, 4), 10);
             
             var go =Instantiate(Resources.Load<GameObject>("Prefabs/Enemy"), position, Quaternion.identity);
               enemies.Add(go.GetComponent<BattleCharacter>());
         }
    }
}
