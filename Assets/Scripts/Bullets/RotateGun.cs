using UnityEngine;

public class RotateGun : MonoBehaviour 
{
    public GrapplingGun grappling;
    private Quaternion ResetRot;
    private float RotationSpeed = 5f;
    private void Update()
    {
       if (!grappling.IsGrappling())
       {
           ResetRot = transform.parent.rotation;
       }
       else
       {
           ResetRot = Quaternion.LookRotation(grappling.GraplingPoint() - -transform.forward);
       }
       transform.rotation = Quaternion.Lerp(transform.rotation, ResetRot, Time.deltaTime * RotationSpeed);

    }



}
