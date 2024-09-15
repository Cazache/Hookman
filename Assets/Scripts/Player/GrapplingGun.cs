using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GrapplingGun : MonoBehaviour
{
    private Vector3 GrapplePoint;
    public LayerMask WhatisGrapeable;
    public Transform Guntip, Camera, player;
    public float maxDistance = 100f;
    private SpringJoint _joint;



    private void Update()
    {
        transform.localPosition = Vector3.zero;
        if (!GameManager.manager.pause)
            if (Input.GetMouseButtonDown(1))
                StartGrapple();
            else if (!Input.GetMouseButton(1) && IsGrappling())
                StopGrapple();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.position, Camera.forward, out hit, maxDistance, WhatisGrapeable))
        {
            GrapplePoint = hit.point;
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = GrapplePoint;

            float distancefrompoint = Vector3.Distance(player.position, GrapplePoint);

            _joint.maxDistance = distancefrompoint * 0.8f;
            _joint.minDistance = distancefrompoint * 0.25f;

            _joint.spring = 4.5f;
            _joint.damper = 7f;
            _joint.massScale = 4.5f;
            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.grappingGunSound, transform, 0.5f);

            // hand.GetComponent<SpringJoint>().connectedAnchor = GrapplePoint;
        }
    }

    public void StopGrapple()
    {
        GameManager.player.anim.SetBool("Grab", false);
        Destroy(_joint);
    }


    public bool IsGrappling()
    {
        return _joint != null;
    }
    public Vector3 GraplingPoint()
    {
        return GrapplePoint;
    }
}
