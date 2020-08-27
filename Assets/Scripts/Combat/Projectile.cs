using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{

}

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;

    Health target = null;

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    public void SetTarget(Health target)
    {
        this.target = target;
    }

    private Vector3 GetAimLocation()
    {
        Vector3 aimLocation = target.transform.position;
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (targetCapsule)
        {
            aimLocation += Vector3.up * targetCapsule.height * 0.5f;
        }
        return aimLocation;
    }
}
