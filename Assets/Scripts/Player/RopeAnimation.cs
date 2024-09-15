using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnimation : MonoBehaviour
{
    //i tried to do this by myself but i dont find the way, so i follow a tutorial 


    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    private GrapplingGun _grapplingGun;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;

    void Awake()
    {
        _grapplingGun = GetComponent<GrapplingGun>();
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!_grapplingGun.IsGrappling())
        {
            currentGrapplePosition = _grapplingGun.Guntip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;

            GameManager.player.anim.SetBool("Grab", false);
            return;
        }
        GameManager.player.anim.SetBool("Grab", true);
        //In the start of grapring set the velocity and points
        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }
        //Just set some settings
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

     

        //Get the grappling point of grappinggun script
        var grapplePoint = _grapplingGun.GraplingPoint();

        //Get the guntip position
        var gunTipPosition = _grapplingGun.Guntip.position;

        //Get the up directiion of the rope so if you are looking a slight angle it will still go up 
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;
       

        //Use this variable for move the rope 
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        //Okay thats a little complicated for me but i think i understant 
        for (var i = 0; i < quality + 1; i++)
        {
            //this goes from 0 to 1 depending of the position on the rope 
            var delta = i / (float)quality;

            //set the waves streng and cuantity on a var
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            //Just set te positions
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }

}
