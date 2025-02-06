using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BattleCharacter : MonoBehaviour
{
    public bool isEnmey;

    private float moveSpeed = 1;

    private string identifier;

    public AudioSource moveSoundSource;
    public AudioSource idleSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource takeDamageSoundSource;
    public AudioSource deathSoundSource;
    public AudioSource spawnSoundSource;

    private BattleCharacter target;

    private float attackTimer;
    public float attackTime = 1.7f;

    private int currentHP;
    public int maxHP = 100;
    private bool isWalking = false;
    public int attackValue = 10;

    public float attackRange = 1;
    public float findEnemyRange = 5;

    public bool isDead = false;
    
    BattleCharacterSound sound;
    private bool isSpawnSoundFinished = false;

    public  int currentAxis = 0;
    
    public void Init(string id,int axis)
    {
        identifier = id;
        currentAxis = axis;
        sound = SoundLoadManager.Instance.enemySoundDict.GetValueOrDefault(id);
        if (sound!=null)
        {
            deathSoundSource.clip = sound.deathClip;
            spawnSoundSource.clip = sound.spawnClip;
            idleSoundSource.clip = sound.idleClip;
            moveSoundSource.clip = sound.moveClip;
        }
        
        spawnSoundSource.Play();

    }
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (!isSpawnSoundFinished)
        {
            if (!spawnSoundSource.isPlaying)
            {
                isSpawnSoundFinished = true;
                if (!isWalking)
                {
                    StopWalking();
                }
            }
        }

        speedupTimer -= Time.deltaTime;
        if (!isEnmey)
        {
            if (target == null)
            {
                bool found = false;
                foreach (var enemy in BattleField.Instance.enemies)
                {
                    var dir = enemy.transform.position - transform.position;
                    var distance = dir.magnitude;
                    if (distance < attackRange * 1.5f)
                    {
                        target = enemy;
                        found = true;
                    }
                }

                if (!found)
                {
                    target = BattleField.Instance.enemies.RandomItem();
                }
                if (target != null)
                {
                    currentAxis = target.currentAxis;
                }
            }
            
            if (target != null)
            {
                var dir = target.transform.position - transform.position;
                var distance = dir.magnitude;
                if (distance > attackRange)
                {
                    transform.position += dir.normalized * moveSpeed * Time.deltaTime;
                    if (!isWalking)
                    {
                        StartWalking();
                    }
                    isWalking = true;
                }
                else
                {
                     if (attackTimer <= 0)
                     {
                         attackTimer = AttackTime();
                         Attack(target);
                     }
                     else
                     {
                         attackTimer -= Time.deltaTime;
                     }
                    StopWalking();
                    isWalking = false;
                }
            }
        }
        else
        {
            
            
            if (target && target.currentAxis == currentAxis)
            {
                var dir = target.transform.position - transform.position;
                var distance = dir.magnitude;
                if (distance < attackRange)
                {
                    if (attackTimer <= 0)
                    {
                        attackTimer = attackTime;
                        Attack(target);
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;
                    }

                    StopWalking();
                    isWalking = false;
                }
                else
                {
                    var rayDir = transform.position.normalized;
                    float targetProjectedDistance = Vector2.Dot(dir, rayDir);
                    if (targetProjectedDistance > 0)
                    {
                            transform.position += rayDir * moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        
                        transform.position -= rayDir * moveSpeed * Time.deltaTime;
                    }
                    if (!isWalking)
                    {
                        StartWalking();
                    }
                    isWalking = true;
                }
            }
            else
            {
                target = null;
                foreach (var enemy in BattleField.Instance.allies)
                {
                    var dir = enemy.transform.position - transform.position;
                    var distance = dir.magnitude;
                    if (distance < findEnemyRange && enemy.currentAxis == currentAxis)
                    {
                        target = enemy;
                    }
                }
                if (target == null)
                {
                    
                    StopWalking();
                    isWalking = false;
                }
            }
        }
    }

    void TakeDamage(BattleCharacter attacker, int damage)
    {
        if (isDead)
        {
            return;
        }
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        if (currentHP <= 0)
        {
            Die();
            return;
        }
        if (sound!=null)
        {
            takeDamageSoundSource.PlayOneShot(sound.hurtClips.RandomItem());
        }
        else
        {
            takeDamageSoundSource.Play();
        }
        //todo change later
        if (target == null)
        {
            target = attacker;
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        if (isEnmey)
        {
            BattleField.Instance.enemies.Remove(this);
        }
        else
        {
            BattleField.Instance.allies.Remove(this);
        }

        deathSoundSource.transform.parent = transform.parent;
        Destroy(deathSoundSource.gameObject,1);
        deathSoundSource.Play();
        isDead = true;
        Destroy(gameObject);
    }
    void Attack(BattleCharacter target)
    {
        if (isDead)
        {
            return;
        }

        if (sound!=null)
        {
            attackSoundSource.PlayOneShot(sound.attackClips.RandomItem());
        }
        else
        {
            attackSoundSource.Play();
        }
        target.TakeDamage(this, AttackValue);
    }

    public void Heal()
    {
        currentHP = Mathf.Clamp(currentHP + 50, 0, maxHP);
    }

    private float speedupTime = 5;
    private float speedupValue = 2;
    private float speedupTimer = 0;
    public void Speedup()
    {
        speedupTimer = speedupTime;
    }

    bool isSpeedUp()
    {
        return speedupTimer > 0;
    }

    float AttackTime()
    {
        if (isSpeedUp())
        {
            return attackTime / 2f;
        }
        return attackTime;
    }
    
    public int AttackValue
    {
        get
        {
            return attackValue;
        }
    }

    void StartWalking()
    {
        idleSoundSource.Stop();
        moveSoundSource.Play();
    }
    void StopWalking()
    {
        
        idleSoundSource.Play();
        moveSoundSource.Stop();
    }
}
