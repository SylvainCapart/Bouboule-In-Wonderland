using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GeneralSceneMgt : MonoBehaviour
{
    public enum SceneIndex {MENU, GAME};

    public static GeneralSceneMgt instance;

    private float m_delay = 2f;
    public SceneIndex m_sceneTarget;
    private const float EPSILON = 0.01f;

    private bool m_IsMenuPlayedOnce;

    public delegate void GenScnMgtDlg();
    public static event GenScnMgtDlg OnMenuInit;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            GoToGame();
        }

    }

    private void Start()
    {
        if (!m_IsMenuPlayedOnce)
        {
            if (OnMenuInit != null)
                OnMenuInit();
            m_IsMenuPlayedOnce = true;
        }
    }

    public void GoToScene(SceneIndex scene_index)
    {
        SceneManager.LoadScene((int)scene_index);
    }

    public void GoToMenu()
    {
        if (System.Math.Abs(Time.timeScale - 1f) > EPSILON)
            Time.timeScale = 1f;
        AudioManager.instance.StopSound("Music");
        AudioManager.instance.PlaySound("Guitar");
        SceneManager.LoadScene((int)SceneIndex.MENU);

    }

    public void GoToGame()
    {
        if (System.Math.Abs(Time.timeScale - 1f) > EPSILON)
            Time.timeScale = 1f;
        AudioManager.instance.StopSound("Guitar");
        AudioManager.instance.PlaySound("Music");
        SceneManager.LoadScene((int)SceneIndex.GAME);

    }

    public void goToScene()
    {
        StartCoroutine(goToSceneCo());

    }
    private IEnumerator goToSceneCo()
    {
        yield return (new WaitForSeconds(m_delay));
        SceneManager.LoadScene((int)m_sceneTarget);
        yield return null;
    }


    public void nextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void chooseSceneWithDelay(SceneIndex sceneIndex, float delay)
    {
        StartCoroutine(chooseSceneWithDelayCo(sceneIndex, delay));
    }


    private IEnumerator chooseSceneWithDelayCo(SceneIndex sceneIndex, float delay)
    {
        yield return (new WaitForSeconds(delay));
        SceneManager.LoadScene((int)sceneIndex);
        yield return null;
    }

    private IEnumerator goToGameDelay()
    {
        yield return (new WaitForSeconds(m_delay));
        SceneManager.LoadScene((int)SceneIndex.GAME);
        yield return null;
    }



    private IEnumerator nextSceneDelay()
    {
        yield return (new WaitForSeconds(m_delay));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
