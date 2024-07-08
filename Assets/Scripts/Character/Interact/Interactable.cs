using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour // "abstract" unreal'daki gibi bir sistem için gerekli
{
    public bool useEvents;
    [SerializeField]
    public string promptMessage; //Interact Text
    public bool border;

    private void Awake()
    {
        
    }

    public void BaseInteract() //oyuncunun çağıracağı fonksiyon
    {
        if (useEvents)
        {
            GetComponent<InteractEvent>().onInteract.Invoke();
        }
        Interact();
    }

    protected virtual void Interact()
    {
        //Temel etkileşimli betikten gelen herhangi bir kod çağrısı olmayacak.
       // Herhangi bir özel kod, miras aldığımız betiklerdeki bu yöntemin içine girecek.
    }
}
