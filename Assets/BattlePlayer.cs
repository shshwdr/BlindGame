using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{
    public int currentAxis = 0;
    public AudioSource soundSource;

    public AudioClip healClip;
    public AudioClip speedupClip;
    public AudioClip rotateClip;
    public AudioClip cancelClip;
    
    // Start is called before the first frame update
    void Start()
    {
        healClip = Resources.Load<AudioClip>("sfx/spell/heal");
        speedupClip = Resources.Load<AudioClip>("sfx/spell/speedup");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAxis -= 1;
            if (currentAxis < 0)
            {
                currentAxis = BattleField.MaxAxis;
            }
            Rotate ();
            // if (currentAxis < -BattleField.MaxAxis)
            // {
            //     currentAxis = -BattleField.MaxAxis;
            //     soundSource.PlayOneShot(cancelClip);
            // }
            // else
            // {
            //     Rotate ();
            // }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAxis += 1;
            
            if (currentAxis >BattleField.MaxAxis)
            {
                currentAxis = 0;
            }
            
            Rotate ();
            
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var target = findCharacterInFront();
            if (target != null)
            {
                soundSource.PlayOneShot(healClip);
                target.Heal();
                DialogueManager.Instance.GetInput("heal");
            }
        }
        
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var target = findCharacterInFront();
            if (target != null)
            {
                soundSource.PlayOneShot(speedupClip);
                target.Speedup();
            }
        }
    }

    BattleCharacter findCharacterInFront()
    {
        foreach (var ally in BattleField.Instance.allies)
        {
            Vector3 playerForward = transform.forward;

            // Calculate the direction from the player to the target object
            Vector3 directionToTarget = ally.transform.position - transform.position;

            // Calculate the angle between the player's forward direction and the target's direction
            float angle = Vector3.Angle(playerForward, directionToTarget);

            // Check if the angle is within 45 degrees
            if (angle <= BattleField.RotateDegree*0.6f)
            {
                return ally;
            }
            else
            {
                soundSource.PlayOneShot(cancelClip);
                Debug.Log("The object is outside the 45-degree angle.");
            }
        }

        return null;
    }

    void Rotate()
    {
        soundSource.PlayOneShot(rotateClip);
        var degree = BattleField.AxisToDegree(currentAxis);
        transform.rotation = Quaternion.Euler(0, degree, 0);
        DialogueManager.Instance.GetInput("rotate");
        //transform.RotateAround(Vector3.zero, Vector3.up, degree);
    }
}
