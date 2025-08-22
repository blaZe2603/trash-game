using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool gameOver = false;
    public int finalScore = 0;
    void Start()
    {

    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            SceneManager.LoadSceneAsync(2);
        }
        try
        {
            GameObject.Find("FinalScore").GetComponent<TextMeshProUGUI>().text = "Final Score : " + finalScore;
        }
        catch
        {

        }
    }

    public void SetScore(int score)
    {
        finalScore = score;
    }
}
