using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    public bool isEnmey;

    private float moveSpeed = 1;

    public AudioSource moveSoundSource;
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

    public bool isDead = false;
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

        speedupTimer -= Time.deltaTime;
        if (!isEnmey)
        {
            if (target == null)
            {
                target = BattleField.Instance.enemies.RandomItem();
            }
            
            if (target != null)
            {
                var dir = target.transform.position - transform.position;
                var distance = dir.magnitude;
                if (distance > 1)
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
            if (target)
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
        takeDamageSoundSource.Play();
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
        attackSoundSource.Play();
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
        moveSoundSource.Play();
    }
    void StopWalking()
    {
        moveSoundSource.Stop();
    }
}
