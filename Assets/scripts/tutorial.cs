using UnityEngine;
using UnityEngine.UIElements;

public class tutorial : MonoBehaviour
{
    public GameObject[] tutorialScenes;
    public GameObject prevButton;
    public GameObject nextButton;
    int totalScenes;
    int CurrentScene;
    void Start()
    {
        totalScenes = tutorialScenes.Length;
        CurrentScene = 0;
        tutorialScenes[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentScene == 0)
            prevButton.SetActive(false);
        else
            prevButton.SetActive(true);
        if (CurrentScene == totalScenes - 1)
            nextButton.SetActive(false);
        else
            nextButton.SetActive(true);
    }
    public void nextSlide()
    {
        tutorialScenes[CurrentScene].SetActive(false);
        CurrentScene++;
        tutorialScenes[CurrentScene].SetActive(true);
    }
    public void prevSlide()
    {
        tutorialScenes[CurrentScene].SetActive(false);
        CurrentScene--;
        tutorialScenes[CurrentScene].SetActive(true);
    }
}
