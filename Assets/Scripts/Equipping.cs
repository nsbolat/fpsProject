using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Equipping : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject slot1, slot2, slot3;

    private void Awake()
    {
        slot1.SetActive(false);
        slot2.SetActive(false);
        slot3.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switchSlot();
    }

    void switchSlot()
    {
        if (Input.GetKeyDown("1"))
        {
            Equip1();
        }
        if (Input.GetKeyDown("2"))
        {
            Equip2();
        }        if (Input.GetKeyDown("3"))
        {
            Equip3();
        }
    }

    void Equip1()
    {
        slot1.SetActive(true);
        slot2.SetActive(false);
        slot3.SetActive(false);
    }
    void Equip2()
    {
        slot1.SetActive(false);
        slot2.SetActive(true);
        slot3.SetActive(false);
    }
    void Equip3()
    {
        slot1.SetActive(false);
        slot2.SetActive(false);
        slot3.SetActive(true);
    }
}
