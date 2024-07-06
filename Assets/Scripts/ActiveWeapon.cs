using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
   private GunSystem weapon;
   public Transform weaponParent;

   private void Start()
   {
      GunSystem existingWeapon = GetComponentInChildren<GunSystem>();
      if (existingWeapon)
      {
         Equip(existingWeapon);
      }
   }

   private void Update()
   {
      if (weapon)
      {
         weapon.MyInput();
         weapon.ADS();
      }
   }

   public void Equip(GunSystem newWeapon)
   {
      if (weapon)
      {
         Destroy(weapon.gameObject);
      }
      weapon = newWeapon;
      weapon.transform.parent = weaponParent;
      weapon.transform.localPosition = Vector3.zero;
      weapon.transform.localRotation = Quaternion.identity;
      weapon.gameObject.SetActive(true);
   }
   
}
