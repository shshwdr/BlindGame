using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    public bool isEnmey;

    private float moveSpeed = 1;

    public AudioSource moveSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource spawnSoundSource;

    private BattleCharacter target;

    private float attackTimer;
    private float attackTime = 1f;

    private int currentHP;
    private int maxHP = 100;
    private bool isWalking = false;

    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void TakeDamage(BattleCharacter attacker, int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Destroy(gameObject);
    }
    void Attack(BattleCharacter target)
    {
        attackSoundSource.Play();
        target.TakeDamage(this, AttackValue);
    }

    public void Heal()
    {
        currentHP = Mathf.Clamp(currentHP + 10, 0, maxHP);
    }
    
    public int AttackValue
    {
        get
        {
            return 10;
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
