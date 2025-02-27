using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Start is called before the first frame update
    void Awake()
    {
        CSVLoader.Instance.Init();
        DialogueManager.Instance.Init();

        MusicManager.Instance.PlayMusic("calm");
        //DialogueManager.Instance.StartDialogue("dialogue_6_7");
        StartCoroutine(waitToStartDialogue());
    }

    IEnumerator waitToStartDialogue()
    {
        yield return new WaitForSeconds(5f);
        
        DialogueManager.Instance.StartDialogue("dialogue_0_1");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
