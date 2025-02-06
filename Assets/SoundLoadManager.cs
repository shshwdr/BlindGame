using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleCharacterSound
{
    public List<AudioClip> attackClips;
    public List<AudioClip> hurtClips;
    public AudioClip moveClip;
    public AudioClip deathClip;
    public AudioClip spawnClip;
    public AudioClip idleClip;
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
        characterSound.hurtClips = allClips.Where(clip => clip.name.StartsWith("Hurt")).ToList();

        var spawns = allClips.Where(clip => clip.name.StartsWith("Spawn")).ToArray();
        if (spawns.Length > 0)
        {
            characterSound.spawnClip = spawns[0];
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
        
        var deaths = allClips.Where(clip => clip.name.StartsWith("Die")).ToArray();
        if (deaths.Length > 0)
        {
            characterSound.deathClip = deaths[0];
        }
        enemySoundDict.Add(id, characterSound);
        
        enemySoundDict[id] = characterSound;
    }
    // Start is called before the first frame update
    void Awake()
    {
        
        FindMusicFiles("goblin");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
