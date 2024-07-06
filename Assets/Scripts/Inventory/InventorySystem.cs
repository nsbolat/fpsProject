using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public InventoryItemData[] items;
    public InventoryItemData testItem;
    public InventoryItemData testItem2;

    private void Start()
    {
        InitVariables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddItem(testItem);
        } if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem(testItem2);
        }
    }

    public void AddItem(InventoryItemData newItem)
    {
        int newItemIndex = (int)newItem.WeaponStyle;
        if (newItemIndex != null)
        {
            RemoveItem(newItemIndex);
        }
        items[newItemIndex] = newItem;
        
    }

    public void RemoveItem(int index)
    {
        items[index] = null;
    }
    private void InitVariables()
    {
        items = new InventoryItemData[3];
    }
}

