using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Text continueText;
    private GameData data;
    public Button delete;
    // Start is called before the first frame update
    void Start()
    {
        data = GameObject.Find("GameData").GetComponent<GameData>();
        if (data.firstTime)
        {
            continueText.text = "Continuar";
        }
        else
        {
            continueText.text = "Empezar";
        }
        delete.onClick.AddListener(data.delete);
    }


    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
