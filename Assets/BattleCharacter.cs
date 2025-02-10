using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class BattleCharacter : MonoBehaviour
{
    public bool isEnmey;
    public bool isRanged = false;

    public float moveSpeed = 1;

    private string identifier;

    public AudioSource moveSoundSource;
    public AudioSource idleSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource takeDamageSoundSource;
    public AudioSource deathSoundSource;
    public AudioSource spawnSoundSource;
    public AudioSource talkSoundSource;

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

    private Dictionary<string, float> timers = new Dictionary<string, float>();

    public  int currentAxis = 0;

    public bool isSpeaking()
    {
        return talkSoundSource.isPlaying;
    }
    public void Speak(string key,bool isInterrupt = false)
    {
        if (!isInterrupt && isSpeaking())
        {
            return;
        }
        if (sound != null)
        {
            var clips = sound.others.GetValueOrDefault(key);
            if (clips!=null &&clips.Count > 0)
            {
                talkSoundSource.clip = clips.RandomItem();
                talkSoundSource.Play();
            }
        }
    }
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
    public void UpdateBattle(float time)
    {
        if (isDead)
        {
            return;
        }

        foreach (var timer in timers.ToList())
        {
            if (timer.Value <= 0)
            {
                timers.Remove(timer.Key);
            }
            else
            {
                timers[timer.Key] -= time;
            }
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

        speedupTimer -= time;
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

                    if (BattleField.Instance.enemies.Count > 0)
                    {
                        
                        target = BattleField.Instance.enemies.RandomItem();
                    }
                }
                if (target != null)
                {
                    currentAxis = target.currentAxis;
                }
            }
            
            if (target != null)
            {

                bool canAttack = false;
                Vector3 dir = new Vector3();
                if (isRanged)
                {

                    var targetPosition =
                        BattleField.GetLocation(target.currentAxis, Vector3.Magnitude(transform.position));
                    if (Vector3.Distance(targetPosition, transform.position) > 0.1f)
                    {
                        canAttack = false;
                        dir = targetPosition - transform.position;
                    }
                    else
                    {
                        dir = target.transform.position - transform.position;
                        var distance = dir.magnitude;
                        if (distance > attackRange)
                        {
                            canAttack = false;
                        }
                        else
                        {
                            canAttack = true;
                        }
                    }
                }
                else
                {
                    dir = target.transform.position - transform.position;
                    var distance = dir.magnitude;
                    if (distance > attackRange)
                    {
                        canAttack = false;
                    }
                    else
                    {
                        canAttack = true;
                    }
                }

                if (!canAttack)
                {
                    
                    transform.position += dir.normalized * moveSpeed * time;
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
                         attackTimer -= time;
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
                    if (targetProjectedDistance > 0.1f)
                    {
                            transform.position += rayDir * moveSpeed * Time.deltaTime;
                    }
                    else if(targetProjectedDistance<-0.1f)
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

    public IEnumerator MoveTo(int axis, float distance,float time)
    {
        var target = BattleField.GetLocation(axis, distance);
        currentAxis = axis;
        transform.DOMove(target, time);
        StartWalking();
        yield return new WaitForSeconds(time);
        
        StopWalking();
    }

    public void KilledOneEnemy()
    {
        if (Random.Range(0, 100) < 40)
        {
            Speak("KillEnemy");
        }

        if (!isEnmey)
        {
            BattleField.Instance.KillEnemy();
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
            attacker.KilledOneEnemy();
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

        if (HPPercent() < 0.3f && !timers.ContainsKey("LowHP"))
        {
            Speak("LowHP");
            timers.Add("LowHP", 5f);
        }
        else if (HPPercent() < 0.5f && !timers.ContainsKey("MiddleHP"))
        {
            
            Speak("MiddleHP");
            timers.Add("MiddleHP", 5f);
        }
    }

    float HPPercent()
    {
        return currentHP / (float)maxHP;
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
        
        //talkSoundSource.transform.parent = transform.parent;
       // Destroy(talkSoundSource.gameObject,1);
        Speak("LowHP");
        
        deathSoundSource.Play();
        isDead = true;
        if (isEnmey)
        {
            
            Destroy(gameObject);
        }
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
        
        Speak("SpellHealed");
    }

    private float speedupTime = 5;
    private float speedupValue = 2;
    private float speedupTimer = 0;
    public void Speedup()
    {
        speedupTimer = speedupTime;
        
        Speak("SpellSpeedUp");
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
