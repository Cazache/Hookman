
using UnityEngine;

public class PetDoor : MonoBehaviour
{
    bool PetInRange;
    public LayerMask WhatIsPet;
    public Transform ClosePos, OpenPos, detector;
    public float range;

    // Update is called once per frame
    void Update()
    {
        PetInRange = Physics.CheckSphere(detector.position, 2, WhatIsPet);
        if(PetInRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, OpenPos.position, 2 * Time.deltaTime);

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, ClosePos.position, 2 * Time.deltaTime);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detector.position, range);
    }
}
