using System;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static fps_Models;

public class GunSystem : MonoBehaviour
{
    public PlayerSettingsModel playerSettings;

    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, recoilForce, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // bools 
    bool shooting, readyToShoot, reloading, noAmmoCooldown;

    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy, groundLayer;

    // Graphics
    [Header("Muzzles")] 
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public GameObject groundBulletHolePrefab;
    public GameObject enemyBulletHolePrefab;
    public GameObject defaultBulletHolePrefab;
    
    [Header("Ammo UI Text")] 
    public TextMeshProUGUI ammoUI;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip noAmmoSound;
    public AudioClip reloadSound;

    [Header("Recoil")]
    public AdvancedCamRecoil camRecoil;
    public AdvancedWeaponRecoil weaponRecoil;

    // Weapon Mesh
    public GameObject weaponModel;
    public Transform AdsPositionTransform;
    [Header("ADS")]
    public Vector3 hipfirePosition;
    public GameObject sightTransform;
    public Vector3 adsPosition;
    public float adsSpeed;
    public Image crosshair;
    public float camFovAds, camFovDefault;
    public bool isAds;

    // Animator fpsAnim;
    [Header("Camera Recoil Settings")]
    public Vector3 gunRecoilRotation;
    public Vector3 gunRecoilRotationAiming;
    public float gunRotationSpeed;
    public float gunReturnSpeed;

    private void OnEnable()
    {
        gunCamRecoilUpdate();
    }

    private void Awake()
    {
        hipfirePosition = weaponModel.transform.localPosition;
        adsPosition = AdsPositionTransform.transform.localPosition;
        camRecoil = GameObject.Find("CameraRecoil").GetComponent<AdvancedCamRecoil>();
        cam = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>();
        camFovDefault = 75;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        MyInput();

        // SetText
        ammoUI.SetText(bulletsLeft + " / " + magazineSize);
    }

    public void gunCamRecoilUpdate()
    {
        camRecoil.RecoilRotation = gunRecoilRotation;
        camRecoil.RecoilRotationAiming = gunRecoilRotationAiming;
        camRecoil.rotationSpeed = gunRotationSpeed;
        camRecoil.returnSpeed = gunReturnSpeed;
    }

    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            playShootSound();
            // ShakeCamera
            camRecoil.Fire();
            weaponRecoil.Fire();
        }

        if (shooting && !reloading && bulletsLeft <= 0)
        {
            if (!noAmmoCooldown)
            {
                // Play no ammo sound
                Debug.Log("No ammo");
                if (noAmmoSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(noAmmoSound);
                }
                noAmmoCooldown = true;
                Invoke("ResetNoAmmoCooldown", 0.25f);
            }
        }

        ADS();
    }

    private void ADS()
    {
        if (Input.GetButton("Fire2") && !reloading)
        {
            isAds = true;
            weaponModel.transform.localPosition = Vector3.Lerp(weaponModel.transform.localPosition, adsPosition, Time.deltaTime * adsSpeed);
            crosshair.color = new Color(0, 0, 0, 0);
            camRecoil.aiming = true;
            weaponRecoil.aiming = true;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFovAds, Time.deltaTime * adsSpeed);
        }
        else
        {
            isAds = false;
            weaponModel.transform.localPosition = Vector3.Lerp(weaponModel.transform.localPosition, hipfirePosition, Time.deltaTime * adsSpeed);
            crosshair.color = new Color(255, 255, 255, 255);
            camRecoil.aiming = false;
            weaponRecoil.aiming = false;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFovDefault, Time.deltaTime * adsSpeed);
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate Direction with Spread
        Vector3 rayDirection = cam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (Physics.Raycast(cam.transform.position, rayDirection, out rayHit, range))
        {
            if (rayHit.collider.CompareTag("Enemy"))
            {
                // rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
            }

            var rb2d = rayHit.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(rayDirection*20,rayHit.point,ForceMode.Impulse);
            }
            var enemyHitBox = rayHit.collider.GetComponent<enemyHitbox>();
            if (enemyHitBox)
            {
                enemyHitBox.onRaycastHit(this, rayDirection);
            }

            createBulletHole();
        }

        // Graphics
        GameObject muzzleFlashLocate = Instantiate(muzzleFlash, attackPoint.position, attackPoint.transform.rotation);
        muzzleFlashLocate.transform.SetParent(attackPoint);  // Muzzle flash'ı silahın attackPoint'ine ekle
        Destroy(muzzleFlashLocate, 1f);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void playShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void createBulletHole()
    {
        GameObject bulletHolePrefab;

        int hitLayer = rayHit.collider.gameObject.layer;

        if ((whatIsEnemy.value & (1 << hitLayer)) > 0)
        {
            bulletHolePrefab = enemyBulletHolePrefab;
        }
        else if ((groundLayer.value & (1 << hitLayer)) > 0)
        {
            bulletHolePrefab = groundBulletHolePrefab;
        }
        else
        {
            bulletHolePrefab = defaultBulletHolePrefab;
        }

        // Instantiate bullet hole
        GameObject bulletHole = Instantiate(bulletHolePrefab, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));

        // Attach bullet hole to the hit object
        bulletHole.transform.SetParent(rayHit.collider.transform);

        Destroy(bulletHole, 2f);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void ResetNoAmmoCooldown()
    {
        noAmmoCooldown = false;
    }

    private void Reload()
    {
        reloading = true;
        audioSource.PlayOneShot(reloadSound);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
