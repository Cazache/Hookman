
using UnityEngine;

public class FarosManager : MonoBehaviour
{
    public int id;
    bool On;
     Light _mylight;
    // Start is called before the first frame update
    void Start()
    {
        _mylight = transform.GetChild(0).GetComponent<Light>();


        if (GameManager.data.level1 && id == 1)
            On = true;

        if (GameManager.data.level2 && id == 2)
            On = true;

        if (GameManager.data.level3 && id == 3)
            On = true;

        if(On)
        {
            GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Faro/FaroOn");
            _mylight.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Faro/faro");
            _mylight.color = Color.red;
        }
    }


}
