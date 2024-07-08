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
    public Image cuWeapon;
    public ArmGrip armGrip;
    public Animator spineAnim;

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
            EquipToSlot(newWeapon, slot1, 0);
            armGrip.UpdateIK(weapon.RightHandTarget, weapon.LeftHandTarget);
            UpdateAnimatorLayers(weapon.label);
        }
        else if (slot2.childCount == 0)
        {
            EquipToSlot(newWeapon, slot2, 1);
        }
        else if (slot3.childCount == 0)
        {
            EquipToSlot(newWeapon, slot3, 2);
        }

        // If this is the first weapon being equipped, update IK and animator layers
        if (currentWeaponIndex == -1)
        {
            currentWeaponIndex = 0;
            weapon = newWeapon;
            weapon.gameObject.SetActive(true);


            // Activate the "Selected" object of the slot containing the selected weapon
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

    private void EquipToSlot(GunSystem newWeapon, Transform slot, int slotIndex)
    {
        weapons.Add(newWeapon);
        newWeapon.transform.parent = slot;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.gameObject.SetActive(false); // Initially inactive

        inventoryBarTransform.GetComponent<UI_Inventory>().ShowInventoryBar();

        // Create ItemSlot prefab for UI
        GameObject itemSlotGO = Instantiate(itemSlotPrefab, inventoryBarTransform); // inventoryBarTransform should be the parent object's transform containing ItemSlot prefab
        itemSlotGO.name = newWeapon.label;
        // Get ItemSlot component
        ItemSlot itemSlot = itemSlotGO.GetComponent<ItemSlot>();

        // Assign weapon icon and label to ItemSlot
        if (itemSlot != null)
        {
            itemSlot.SetIcon(newWeapon.icon); // icon should represent the weapon's icon
            itemSlot.SetLabel(newWeapon.label); // label should represent the weapon's label
            itemSlot.SetSlotIndex(slotIndex);
        }

        // Update WeaponPanel icon

        if (currentWeaponIndex == -1)
        {
            currentWeaponIndex = 0;
            weapon = newWeapon;
            weapon.gameObject.SetActive(true); // Activate the first weapon if it's the first one
            // Activate the "Selected" object of the slot containing the selected weapon
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

        // Add check: If current weapon is reloading or shooting, prevent the switch
        if (weapon && (weapon.reloading || weapon.shooting))
        {
            return;
        }

        // Add check: If current weapon is already in the selected slot, prevent the switch
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
            weapon.ResetShot(); // Reset the current weapon
            weapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = slotIndex;
        weapon = weapons[slotIndex];
        weapon.gameObject.SetActive(true);

        // Update IK targets
        armGrip.UpdateIK(weapon.RightHandTarget, weapon.LeftHandTarget);

        // Deactivate the "Selected" objects in all item slots in the UI
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

        // Activate the "Selected" object of the slot containing the selected weapon
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

        // Update animator layers
        UpdateAnimatorLayers(weapon.label);
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

    private void UpdateAnimatorLayers(string weaponLabel)
    {
        // Reset all layers to 0
        for (int i = 0; i < spineAnim.layerCount; i++)
        {
            spineAnim.SetLayerWeight(i, 0);
        }

        // Activate the layer corresponding to the current weapon
        switch (weaponLabel)
        {
            case "Pistol":
                spineAnim.SetLayerWeight(spineAnim.GetLayerIndex("Pistol"), 1);
                break;
            case "Rifle":
                spineAnim.SetLayerWeight(spineAnim.GetLayerIndex("Rifle"), 1);
                break;
            case "Shotgun":
                spineAnim.SetLayerWeight(spineAnim.GetLayerIndex("Shotgun"), 1);
                break;
            default:
                spineAnim.SetLayerWeight(spineAnim.GetLayerIndex("Hand"), 1);
                break;
        }
    }
}