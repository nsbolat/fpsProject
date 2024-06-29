using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Interactable
{
    private ActiveWeapon _activeWeapon;
    public GunSystem weaponFab;

    private void Awake()
    {
        _activeWeapon = GameObject.FindGameObjectWithTag("Player").GetComponent<ActiveWeapon>();
    }

    protected override void Interact()
    {
        ActiveWeapon activeWeapon = _activeWeapon;
        if (activeWeapon)
        {
            GunSystem newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);
        }
        Destroy(this.gameObject);
    }
}
