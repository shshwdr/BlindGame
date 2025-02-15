using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{
    public int currentAxis = 0;
    public AudioSource soundSource;

    public AudioClip healClip;
    public AudioClip speedupClip;
    public AudioClip pushClip;
    public AudioClip stunClip;
    public AudioClip rotateClip;
    public AudioClip cancelClip;

    private BattleCharacter currentSelectedCharacter = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        healClip = Resources.Load<AudioClip>("sfx/spell/heal");
        speedupClip = Resources.Load<AudioClip>("sfx/spell/speedup");
    }

    private KeyCode currentKeyCode = KeyCode.Backspace;
    // Update is called once per frame
    void Update()
    {
        if (!isCastingSpell)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentAxis -= 1;
                if (currentAxis < 0)
                {
                    currentAxis = BattleField.MaxAxis;
                }
                Rotate ();
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
        }
       
        
        List<KeyCode> spellKeys = new List<KeyCode>(){ KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4};
Dictionary<KeyCode,bool> isEnemy = new Dictionary<KeyCode,bool>()
{
    {KeyCode.Alpha1,false},
    {KeyCode.Alpha2,false},
    {KeyCode.Alpha3,true},
    {KeyCode.Alpha4,true},
};
        foreach (var key in spellKeys)
        {
            if (Input.GetKeyDown(key))
            {
                var target = findCharacterInFront(isEnemy[key]);
                if (target != null)
                {
                    currentKeyCode = key;
                    currentSelectedCharacter = target;
                    
                    foreach (var filter in FindObjectsOfType<AudioDirectionFilter>())
                    {
                        var character = filter.GetComponent<BattleCharacter>();
                        if (character && character == currentSelectedCharacter)
                        {
                            filter.CutCharacter(false);
                        }
                        else
                        {
                            filter.CutCharacter(true);
                        }
                    }
                }
            }

            if (Input.GetKeyUp(key))
            {
                if (currentKeyCode == key)
                {
                    currentKeyCode  = KeyCode.Backspace;

                    switch (key)
                    {
                        case KeyCode.Alpha1:
                            
                            currentSelectedCharacter.talkSoundSource.PlayOneShot(healClip);
                            currentSelectedCharacter.Heal();
                            DialogueManager.Instance.GetInput("heal");
                            break;
                        case KeyCode.Alpha2:
                            currentSelectedCharacter.talkSoundSource.PlayOneShot(speedupClip);
                            currentSelectedCharacter.Speedup();
                            break;
                        case KeyCode.Alpha3:
                            currentSelectedCharacter.talkSoundSource.PlayOneShot(pushClip);
                            currentSelectedCharacter.Push();
                            break;
                        case KeyCode.Alpha4:
                            currentSelectedCharacter.talkSoundSource.PlayOneShot(stunClip);
                            currentSelectedCharacter.Stun();
                            break;
                    }
                  
                    foreach (var filter in FindObjectsOfType<AudioDirectionFilter>())
                    {
                        
                        filter.CutCharacter(false);
                    }
                    // foreach (var resonanceSource in FindObjectsOfType<ResonanceAudioSource>())
                    // {
                    //     
                    //         resonanceSource.gainDb = 0;
                    //     
                    // }
                }
            }

            if (Input.GetKey(key))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentSelectedCharacter = findAnotherCharacterInFront(currentSelectedCharacter, true);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentSelectedCharacter = findAnotherCharacterInFront(currentSelectedCharacter, false);
                }


                
                // foreach (var resonanceSource in FindObjectsOfType<ResonanceAudioSource>())
                // {
                //     if (resonanceSource.transform.parent)
                //     {
                //         
                //         var character = resonanceSource.transform.parent.GetComponent<BattleCharacter>();
                //         if (character != currentSelectedCharacter)
                //         {
                //             resonanceSource.gainDb = -40;
                //         }
                //         else
                //         {
                //             resonanceSource.gainDb = 0;
                //         }
                //     }
                // }

                
            }
        }
        
        
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     var target = findCharacterInFront();
        //     if (target != null)
        //     {
        //         soundSource.PlayOneShot(healClip);
        //         target.Heal();
        //         DialogueManager.Instance.GetInput("heal");
        //     }
        // }
        //
        //
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     var target = findCharacterInFront();
        //     if (target != null)
        //     {
        //         soundSource.PlayOneShot(speedupClip);
        //         target.Speedup();
        //     }
        // }
    }
    
    

    bool isCastingSpell => (currentKeyCode != KeyCode.Backspace);
    BattleCharacter findCharacterInFront(bool isEnemy)
    {
        
        var character = isEnemy?BattleField.Instance.enemies:BattleField.Instance.allies;
        var charactersInArea = new List<BattleCharacter>();
        foreach (var ally in character)
        {
            Vector3 playerForward = transform.forward;

            // Calculate the direction from the player to the target object
            Vector3 directionToTarget = ally.transform.position - transform.position;

            // Calculate the angle between the player's forward direction and the target's direction
            float angle = Vector3.Angle(playerForward, directionToTarget);

            // Check if the angle is within 45 degrees
            if (angle <= BattleField.RotateDegree*0.6f)
            {
                charactersInArea.Add(ally);
            }
            else
            {
            }
        }

        if (charactersInArea.Count == 0)
        {
            soundSource.PlayOneShot(cancelClip);
            Debug.Log("The object is outside the 45-degree angle.");
            return null;
        }
        //sort charactersInArea by distance
        charactersInArea = charactersInArea.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();

        return charactersInArea[0];

    }
    BattleCharacter findAnotherCharacterInFront(BattleCharacter current, bool isNext)
    {
        var isEnemy = current.isEnmey;
        var character = isEnemy?BattleField.Instance.enemies:BattleField.Instance.allies;
        var charactersInArea = new List<BattleCharacter>();
        foreach (var ally in character)
        {
            Vector3 playerForward = transform.forward;

            // Calculate the direction from the player to the target object
            Vector3 directionToTarget = ally.transform.position - transform.position;

            // Calculate the angle between the player's forward direction and the target's direction
            float angle = Vector3.Angle(playerForward, directionToTarget);

            // Check if the angle is within 45 degrees
            if (angle <= BattleField.RotateDegree*0.6f)
            {
                charactersInArea.Add(ally);
            }
            else
            {
            }
        }

        if (charactersInArea.Count == 0)
        {
            return current;
        }
        
        //sort charactersInArea by distance
        charactersInArea = charactersInArea.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
        var index = charactersInArea.FindIndex(x => x == current);
        
        index+= isNext?1:-1;
        index = Mathf.Clamp(index, 0, charactersInArea.Count - 1);
        
        return charactersInArea[index];
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
