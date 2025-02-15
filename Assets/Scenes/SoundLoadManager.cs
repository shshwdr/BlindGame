using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleCharacterSound
{
    public List<AudioClip> attackClips;
    public List<AudioClip> weaponAttackClips;
    public List<AudioClip> hurtClips;
    public AudioClip moveClip;
    public AudioClip healToFullClip;
    public AudioClip deathClip;
    public AudioClip spawnClip;
    public AudioClip idleClip;
    public AudioClip ChargeClip;
    public AudioClip ChargeStartClip;
    public Dictionary<string, List<AudioClip>> others;
}
public class SoundLoadManager : Singleton<SoundLoadManager>
{
    public Dictionary<string, BattleCharacterSound> enemySoundDict = new Dictionary<string, BattleCharacterSound>();
    
    public void FindMusicFiles(string id)
    {
        var folderPath = "sfx/character/" + id;
        var characterSound = new BattleCharacterSound();
        // 从 Resources 中加载指定文件夹下的所有 AudioClip 资源
        AudioClip[] allClips = Resources.LoadAll<AudioClip>(folderPath);
        
        if (allClips == null || allClips.Length == 0)
        {
            Debug.LogWarning("在 Resources 路径下未找到任何音乐文件: " + folderPath);
            return;
        }

        // 过滤出文件名以 "Attack_" 开头的音乐文件
        characterSound.attackClips = allClips.Where(clip => clip.name.StartsWith("Attack")).ToList();
        characterSound.weaponAttackClips = allClips.Where(clip => clip.name.StartsWith("WeaponAttack")).ToList();
        characterSound.hurtClips = allClips.Where(clip => clip.name.StartsWith("Hurt")).ToList();

        var spawns = allClips.Where(clip => clip.name.StartsWith("Spawn")).ToArray();
        if (spawns.Length > 0)
        {
            characterSound.spawnClip = spawns[0];
        }
        var healToFulls = allClips.Where(clip => clip.name.StartsWith("HealToFul")).ToArray();
        if (healToFulls.Length > 0)
        {
            characterSound.healToFullClip = healToFulls[0];
        }
        
        var idles = allClips.Where(clip => clip.name.StartsWith("Idle")).ToArray();
        if (idles.Length > 0)
        {
            characterSound.idleClip = idles[0];
        }
        
        var moves = allClips.Where(clip => clip.name.StartsWith("Run")).ToArray();
        if (moves.Length > 0)
        {
            characterSound.moveClip = moves[0];
        }
        
        var charges = allClips.Where(clip => clip.name.StartsWith("Charge")).ToArray();
        if (charges.Length > 0)
        {
            characterSound.ChargeClip = charges[0];
        }
        
        var chargeStarts = allClips.Where(clip => clip.name.StartsWith("ChargeStart")).ToArray();
        if (chargeStarts.Length > 0)
        {
            characterSound.ChargeStartClip = chargeStarts[0];
        }
        
        var deaths = allClips.Where(clip => clip.name.StartsWith("Die")).ToArray();
        if (deaths.Length > 0)
        {
            characterSound.deathClip = deaths[0];
        }
        enemySoundDict.Add(id, characterSound);
        characterSound.others = new Dictionary<string, List<AudioClip>>();
        var others = allClips.Where(clip => !clip.name.StartsWith("Attack") && !clip.name.StartsWith("Hurt") && !clip.name.StartsWith("Spawn") && !clip.name.StartsWith("Idle") && !clip.name.StartsWith("Die") && !clip.name.StartsWith("Run")).ToList();
        foreach (var clip in others)
        {
            var name = clip.name.Split('_');
            var key = name[0];
            if (!characterSound.others.ContainsKey(key))
            { 
                characterSound.others.Add(key, new List<AudioClip>());  
            }
            characterSound.others[key].Add(clip);
        }
        
        enemySoundDict[id] = characterSound;
        
        
    }
    // Start is called before the first frame update
    void Awake()
    {
        
        FindMusicFiles("goblin");
        FindMusicFiles("bat");
        FindMusicFiles("snake");
        FindMusicFiles("hero1");
        FindMusicFiles("hero2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
