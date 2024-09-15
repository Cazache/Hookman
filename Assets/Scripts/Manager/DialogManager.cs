using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public struct sentence
    {
        public List<string> toSay;
    }
    public Text dialogText;
    public sentence[] sentences;
    public GameObject dialogBox, hud;
    public GameObject Button;
    private float _speedWrite = 0.03f;

    private int _index = 0, _sentence;

    // Start is called before the first frame update
    void Start()
    {
        _index = 0;
        CleanText();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //First time on main room
            if (!GameManager.data.firstTime)
            {
                Button.transform.GetChild(0).tag = "1";
                GameManager.player.canMove = false;
                Invoke("WaitAndTalkStart", 0.5f);
                GameManager.data.firstTime = true;
            }
            else if (GameManager.data.level1)
            {
                Button.transform.GetChild(0).tag = "2";
            }
            else
            {
                Button.transform.GetChild(0).tag = "Untagged";
            }
            if (GameManager.data.level1 && GameManager.data.level2 && GameManager.data.level3 && !GameManager.data.end)
            {
                GameManager.player.canMove = false;
                Invoke("WaitAndTalkBoss", 0.5f);
                GameManager.data.end = true;
            }
        }





        if (SceneManager.GetActiveScene().buildIndex == 2 && !GameManager.data.firstTimeLevel1)
        {
            GameManager.player.canMove = false;
            Invoke("WaitAndTalkStartLevel1", 0.5f);
            GameManager.data.firstTimeLevel1 = true;
        }
    }
    void WaitAndTalkBoss()
    {
        ShowDialog(3);
    }
    void WaitAndTalkStart()
    {
        ShowDialog(0);
    }
    void WaitAndTalkStartLevel1()
    {
        ShowDialog(6);
    }
    void CleanText()
    {
        dialogText.text = "";
    }
    void enableDisableBox(bool enable)
    {
        dialogBox.SetActive(enable);
        hud.SetActive(!enable);
    }
    public void ShowDialog(int index)
    {
        _index = index;
        if (dialogBox.activeSelf == false)
        {
            enableDisableBox(true);
        }

        if (_index < sentences.Length)
        {
            StartCoroutine(Talk(0));
        }
        else
        {
            CleanText();
        }
    }
    IEnumerator Talk(int index)
    {
        _sentence = index;
        dialogText.text = "";
        for (int i = 0; i < sentences[_index].toSay.Count; i++)
        {
            CleanText();
            foreach (char letra in sentences[_index].toSay[_sentence].ToCharArray())
            {
                dialogText.text += letra;
                yield return new WaitForSeconds(_speedWrite);
            }
            _sentence++;
            if (_index == 1 && _sentence == 2)
            {
                GameManager.manager.SpawnPet();
            }
            yield return new WaitForSeconds(2.5f);
        }

        yield return new WaitForSeconds(3);

        GameManager.player.canMove = true;
        GameManager.data.Save();
        enableDisableBox(false);

    }
}
