using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveWeapon : MonoBehaviour
{
    private GunSystem weapon;
    private List<GunSystem> weapons = new List<GunSystem>();
    public GameObject itemSlotPrefab;
    public Transform inventoryBarTransform;
    public GameObject currentWeaponUI;
    public Image cuWeapon; // Reference to your WeaponPanel GameObject

    public Transform slot1, slot2, slot3;

    private int currentWeaponIndex = -1;

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
            currentWeaponUI.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            currentWeaponUI.GetComponent<CanvasGroup>().alpha = 0;
        }
        
        // Weapon switch input handling
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeaponIndex != 0)
            {
                SwitchWeapon(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SwitchToNextWeapon();
        }
        else if (scroll < 0f)
        {
            SwitchToPreviousWeapon();
        }
    }

    public void Equip(GunSystem newWeapon)
    {
        // Check if all slots are full
        if (slot1.childCount > 0 && slot2.childCount > 0 && slot3.childCount > 0)
        {
            return;
        }

        // Equip weapon to the first available slot
        if (slot1.childCount == 0)
        {
            EquipToSlot(newWeapon, slot1, 1);
        }
        else if (slot2.childCount == 0)
        {
            EquipToSlot(newWeapon, slot2, 2);
        }
        else if (slot3.childCount == 0)
        {
            EquipToSlot(newWeapon, slot3, 3);
        }
    }

    private void EquipToSlot(GunSystem newWeapon, Transform slot, int slotIndex)
    {
        weapons.Add(newWeapon);
        newWeapon.transform.parent = slot;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.gameObject.SetActive(false); // Initially inactive

        inventoryBarTransform.GetComponent<UI_Inventory>().ShowInventoryBar();

        // UI için ItemSlot prefabını oluşturun
        GameObject itemSlotGO = Instantiate(itemSlotPrefab, inventoryBarTransform); // inventoryBarTransform, ItemSlot prefabını içeren parent objenin transformu olmalıdır
        itemSlotGO.name = newWeapon.label;
        // ItemSlot componentini alın
        ItemSlot itemSlot = itemSlotGO.GetComponent<ItemSlot>();

        // Silahın ikonunu ve etiketini ItemSlot'a atayın
        if (itemSlot != null)
        {
            itemSlot.SetIcon(newWeapon.icon); // icon, silahın ikonunu temsil eden bir değişken olmalı
            itemSlot.SetLabel(newWeapon.label); // label, silahın etiketini temsil eden bir değişken olmalı
            itemSlot.SetSlotIndex(slotIndex);
        }
        
        // Update WeaponPanel icon

        if (currentWeaponIndex == -1)
        {
            currentWeaponIndex = 0;
            weapon = newWeapon;
            weapon.gameObject.SetActive(true); // Activate the first weapon if it's the first one
            // Seçilen silahın bulunduğu slotun "Selected" objesini aktif hale getirin
            Transform selectedSlot = inventoryBarTransform.GetChild(currentWeaponIndex);
            if (selectedSlot != null)
            {
                Transform selected = selectedSlot.Find("Selected");
                if (selected != null)
                {
                    selected.gameObject.SetActive(true);
                    UpdateWeaponPanelIcon(newWeapon.icon);
                }
            }
        }
    }

    public void SwitchWeapon(int slotIndex)
    {
        inventoryBarTransform.GetComponent<UI_Inventory>().ShowInventoryBar();
        
        // Kontrolü ekle: Eğer mevcut silah reloading veya shooting ise işlemi engelle
        if (weapon && (weapon.reloading || weapon.shooting))
        {
            return;
        }

        // Kontrolü ekle: Eğer mevcut silah zaten seçili slot ise işlemi engelle
        if (currentWeaponIndex == slotIndex)
        {
            return;
        }

        if (slotIndex < 0 || slotIndex >= weapons.Count || weapons[slotIndex] == null)
        {
            return;
        }

        if (weapon)
        {
            weapon.ResetShot(); // Current weapon reset
            weapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = slotIndex;
        weapon = weapons[slotIndex];
        weapon.gameObject.SetActive(true);
        

        // UI'deki tüm item slotlarındaki "Selected" objelerini devre dışı bırakın
        foreach (Transform child in inventoryBarTransform)
        {
            ItemSlot itemSlot = child.GetComponent<ItemSlot>();
            if (itemSlot != null)
            {
                Transform selected = child.Find("Selected");
                if (selected != null)
                {
                    selected.gameObject.SetActive(false);
                }
            }
        }

        // Seçilen silahın bulunduğu slotun "Selected" objesini aktif hale getirin
        Transform selectedSlot = inventoryBarTransform.GetChild(slotIndex);
        if (selectedSlot != null)
        {
            Transform selected = selectedSlot.Find("Selected");
            if (selected != null)
            {
                selected.gameObject.SetActive(true);
            }
        }
        
        // Update WeaponPanel icon
        UpdateWeaponPanelIcon(weapon.icon);
    }

    private void SwitchToNextWeapon()
    {
        if (weapons.Count == 0)
        {
            return;
        }

        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        SwitchWeapon(nextIndex);
    }

    private void SwitchToPreviousWeapon()
    {
        if (weapons.Count == 0)
        {
            return;
        }

        int previousIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        SwitchWeapon(previousIndex);
    }

    private void UpdateWeaponPanelIcon(Sprite icon)
    {
        Image weaponIconImage = cuWeapon;
            weaponIconImage.sprite = icon;
    }
}
