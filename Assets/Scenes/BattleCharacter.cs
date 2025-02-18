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
    public bool isHealer = false;
    private string identifier;
    public float chargeTime = 0f;
    public bool canAttackOtherLine = false;
    public bool canAttackHealer = false;

    public AudioSource moveSoundSource;
    public AudioSource idleSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource takeDamageSoundSource;
    public AudioSource deathSoundSource;
    public AudioSource spawnSoundSource;
    public AudioSource talkSoundSource;
    public AudioSource weaponAttack;

    private List<AudioSource> sourceExceptTalk;
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

    public GameObject renderer;

    public void resetBattle()
    {
        StopWalking();
    }
    
    public bool isSpeaking()
    {
        if (talkSoundSource == null)
        {
            return false;
        }

        if (!talkSoundSource.isPlaying || talkSoundSource.time == 0)
        {
            return false;
        }

        return true;
        return talkSoundSource!=null && (talkSoundSource.isPlaying || talkSoundSource.time != 0);
    }

    public IEnumerator speakWithDelay(string key, float time, bool isInterrupt = false)
    {
         yield return new WaitForSeconds(time);
         Speak(key,isInterrupt);
    }
    public void Speak(string key,bool isInterrupt = false)
    {
        if (talkSoundSource == null)
        {
            return;
        }
        
        if (sourceExceptTalk!=null)
        {
            
            foreach (var source in sourceExceptTalk)
            {
                source.volume = 0;
            }
        }
        
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
        isDead = false;
        renderer.SetActive(true);
        sourceExceptTalk= new List<AudioSource>()
        {
            moveSoundSource,
            idleSoundSource,
            attackSoundSource,
            takeDamageSoundSource,
           // deathSoundSource,
            spawnSoundSource
        };
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

        if (spawnSoundSource!=null && spawnSoundSource.clip != null)
        {
            
            spawnSoundSource.Play();
        }

        currentHP = maxHP;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateBattle(float time)
    {
        if (isDead)
        {
            return;
        }

        if (isHealer)
        {
            return;
        }
        if (isStun())
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        if (chargeTime > 0 && chargeTimer>0)
        {
            //var originChargeTimer = chargeTimer;
            chargeTimer-= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                var target = BattleField.Instance.currentAlliesInBattle.RandomItem();
                if (target != null)
                {
                    Attack(target);
                }
            }
        }
        
        if (!isSpeaking())
        {
            if (sourceExceptTalk!=null)
            foreach (var source in sourceExceptTalk)
            {
                source.volume = 1;
            }
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
            if (target != null && target.isDead)
            {
                target = null;
            }
            if (target == null || target.isDead)
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
            
            if (target != null&& !target.isDead)
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
        else//enemy
        {

            if (isCharging)
            {
                return;
            }
            if (target && ( target.currentAxis == currentAxis || target == BattleField.Instance.healer))
            {
                var dir = target.transform.position - transform.position;
                var distance = dir.magnitude;
                if (distance < attackRange)
                {
                    if (attackTimer <= 0)
                    {
                        attackTimer = attackTime;

                        if (chargeTime > 0)
                        {
                            Charge();
                        }
                        else
                        {
                            
                            Attack(target);
                        }
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
                    else
                    {
                        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
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
                foreach (var enemy in BattleField.Instance.currentAlliesInBattle)
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

                    if (canAttackHealer)
                    {
                        target = BattleField.Instance.healer;
                    }
                    else
                    {
                        StopWalking();
                        isWalking = false;
                    }
                    
                }
            }
        }
    }

    private float chargeTimer = 0;
    bool isCharging=> chargeTimer>0;

    public IEnumerator MoveTo(int axis, float distance,float time)
    {
        var target = BattleField.GetLocation(axis, distance);
        currentAxis = axis;
        transform.DOMove(target, time);
        StartWalking();
        yield return new WaitForSeconds(time);
        
        StopWalking();
    }

    public void Charge()
    {
        chargeTimer = chargeTime;
        spawnSoundSource.clip = sound.ChargeStartClip;
        if (spawnSoundSource!=null && spawnSoundSource.clip != null)
        spawnSoundSource.Play();
    }

    void StopCharge()
    {
        
        spawnSoundSource.Stop();
        chargeTimer = 0;
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
        {if (takeDamageSoundSource!=null && takeDamageSoundSource.clip != null)
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
            deathSoundSource.transform.parent = transform.parent;
            Destroy(deathSoundSource.gameObject,1);
        }
        else
        {
            BattleField.Instance.currentAlliesInBattle.Remove(this);
            renderer.SetActive(false);    
            deathSoundSource.transform.position = BattleField.GetLocation(currentAxis, 1);
talkSoundSource.Stop();
        }

        //talkSoundSource.transform.parent = transform.parent;
       // Destroy(talkSoundSource.gameObject,1);
        
        if (deathSoundSource!=null && deathSoundSource.clip != null)
        deathSoundSource.Play();
        isDead = true;

        if (isHealer)
        {
            BattleField.Instance.LoseBattle(false);
        }
        
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
        StopCharge();

        if (sound!=null)
        {
            attackSoundSource.PlayOneShot(sound.attackClips.RandomItem());
            if(sound.weaponAttackClips.Count>0)
            weaponAttack.PlayOneShot(sound.weaponAttackClips.RandomItem());
        }
        else
        {if (attackSoundSource!=null && attackSoundSource.clip != null)
            attackSoundSource.Play();
        }
        target.TakeDamage(this, AttackValue);
    }

    public void Heal()
    {
        currentHP = Mathf.Clamp(currentHP + 50, 0, maxHP);
        
        StartCoroutine(speakWithDelay("SpellHealed",0.3f));
    }

    private float speedupTime = 5;
    private float speedupValue = 2;
    private float speedupTimer = 0;
    public void Speedup()
    {
        speedupTimer = speedupTime;
        
        StartCoroutine(speakWithDelay("SpellSpeedUp",0.3f));
    }

    private float stunTime = 5;
    private float stunTimer = 0;

    bool isStun()
    {
        return stunTimer > 0;
    }
    float distance()
    {
        return Vector3.Magnitude(transform.position);
    }
    public void Push()
    {
        transform.position = BattleField.GetLocation(currentAxis, distance() + 5);
    }
    public void Stun()
    {
        stunTimer = stunTime;
        StopCharge();
        StopWalking();
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
        if(moveSoundSource!=null &&moveSoundSource.clip!=null)
        moveSoundSource.Play();
    }
    void StopWalking()
    {
        
        if(idleSoundSource!=null &&idleSoundSource.clip!=null)
        idleSoundSource.Play();
        moveSoundSource.Stop();
    }
}
