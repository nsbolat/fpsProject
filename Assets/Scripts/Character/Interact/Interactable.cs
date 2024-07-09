using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour // "abstract" unreal'daki gibi bir sistem için gerekli
{
    public bool useEvents;
    [SerializeField]
    public string promptMessage; //Interact Text
    public bool isHoldable; // Nesnenin basılı tutma ile mi yoksa tek tıklama ile mi etkileşime gireceğini belirler
    public float requiredHoldTime = 2f; // Basılı tutma süresi (varsayılan: 2 saniye)

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

    public void HoldBaseInteract()
    {
        HoldInteract();
    }

    protected virtual void Interact()
    {
        //Temel etkileşimli betikten gelen herhangi bir kod çağrısı olmayacak.
       // Herhangi bir özel kod, miras aldığımız betiklerdeki bu yöntemin içine girecek.
    }
    protected virtual void HoldInteract()
    {
        // Basılı tutma işlemleri burada yer alacak
    }
}
