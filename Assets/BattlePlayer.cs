using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate (-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(1);
        }
    }

    void Rotate(int dir)
    {
        if (dir == 1)
        {
            transform.Rotate(0, 30, 0);
        }
        else if (dir == -1)
        {
            transform.Rotate(0, -30, 0);
        }
    }
}
