using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{
    private int currentAxis = 0;
    public AudioSource soundSource;

    public AudioClip healClip;
    public AudioClip speedupClip;
    public AudioClip rotateClip;
    public AudioClip cancelClip;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAxis -= 1;
            if (currentAxis < -BattleField.MaxAxis)
            {
                currentAxis = -BattleField.MaxAxis;
                soundSource.PlayOneShot(cancelClip);
            }
            else
            {
                Rotate ();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAxis += 1;
            
            if (currentAxis > BattleField.MaxAxis)
            {
                currentAxis = BattleField.MaxAxis;
                soundSource.PlayOneShot(cancelClip);
            }
            else
            {
                
                Rotate();
            }
            
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var target = findCharacterInFront();
            if (target != null)
            {
                soundSource.PlayOneShot(healClip);
                target.Heal();
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

            // Check if the angle is within 30 degrees
            if (angle <= 20f)
            {
                return ally;
                soundSource.PlayOneShot(healClip);
                ally.Heal();
                Debug.Log("The object is within 30 degrees in front of the player.");
                break;
            }
            else
            {
                soundSource.PlayOneShot(cancelClip);
                Debug.Log("The object is outside the 30-degree angle.");
            }
        }

        return null;
    }

    void Rotate()
    {
        soundSource.PlayOneShot(rotateClip);
        var degree = BattleField.AxisToDegree(currentAxis);
        transform.rotation = Quaternion.Euler(0, degree, 0);

        //transform.RotateAround(Vector3.zero, Vector3.up, degree);
    }
}
