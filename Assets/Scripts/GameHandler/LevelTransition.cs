using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance { get; private set; }

    private Animator animator;
    private int fadeAnimHash = Animator.StringToHash("Start");

    private float transitionTime = 1.0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        animator = GetComponent<Animator>();
    }

    public void Load(string levelName)
    {
        StartCoroutine(LoadLevel(levelName));
    }

    private IEnumerator LoadLevel(string levelName)
    {
        animator.SetTrigger(fadeAnimHash);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelName, LoadSceneMode.Single);

        yield return new WaitForSeconds(0.2f);

        animator.SetTrigger(fadeAnimHash);
    }
}
