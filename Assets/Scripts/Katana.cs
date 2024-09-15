
using UnityEngine;

public class Katana : MonoBehaviour
{
    public bool equiped, unlocked = false;
    private Transform _equipPos;
    public Transform unEquipPos;
    public float speed, rotSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _equipPos = GameManager.player.gunpos;
    }


    public void UnlockKatana()
    {
        unlocked = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponentInChildren<Collider>().enabled = false;
        GetComponent<Outline>().enabled = false;
        gameObject.layer = 17;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = 17;
        }
        transform.parent = _equipPos;
        transform.localPosition= Vector3.zero;
        transform.localRotation = Quaternion.Euler(0,0,0);
      
   
        Equip();
    }
    public void Equip()
    {
        GameManager.player.anim.SetBool("KatanaEquiped", true);
        GameManager.habilities.readyToAttack = false;
        equiped = true;
        Invoke("EnableMesh", 0.7f);
        GameManager.habilities.Invoke("ResetMeeleAttack", 0.4f);
    }
    public void UnEquip()
    {
        GameManager.player.anim.SetBool("KatanaEquiped", false);
        equiped = false;
        EnableMesh();
    }
    public void EnableMesh()
    {
        GetComponentInChildren<MeshRenderer>().enabled = equiped;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponentInChildren<Collider>().enabled = false;
            UnlockKatana();               
        }
            
    }
}
