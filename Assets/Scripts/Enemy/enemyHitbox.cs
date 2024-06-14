using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHitbox : MonoBehaviour
{
    public enemyHealth enemyHealth;

    public void onRaycastHit(GunSystem weapon, Vector3 direction)
    {
        enemyHealth.takeDamage(weapon.damage,direction);
    }
}
