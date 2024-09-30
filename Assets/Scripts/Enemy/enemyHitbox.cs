using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHitbox : MonoBehaviour
{
    public enemyHealth enemyHealth;
    public bool isHead = false;

    public void onRaycastHit(GunSystem weapon, Vector3 direction,GameObject hitObject)
    {
        enemyHealth.takeDamage(weapon.damage, direction, hitObject,isHead);
    }
}