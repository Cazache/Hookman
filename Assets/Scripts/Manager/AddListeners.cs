
using UnityEngine;
using UnityEngine.UI;

public class AddListeners : MonoBehaviour
{
   // I don't like do this like that, but it's the only way I found it.
    public GameData gameData;
    void Start()
    {
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        if (transform.tag == "SaveButton")
            GetComponent<Button>().onClick.AddListener(gameData.Save);
        else
        {
            GetComponent<Button>().onClick.AddListener(gameData.Load);
            GetComponent<Button>().onClick.AddListener(gameData.LoadCurrentScene);
        }
            
    }
}
