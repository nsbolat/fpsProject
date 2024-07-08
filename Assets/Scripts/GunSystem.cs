using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GunSystem : MonoBehaviour
{
    [Header("ARM TARGET")] 
    public Transform RightHandTarget;
    public Transform LeftHandTarget;
    [Header("Gun Stats")]
    public DefaultInput gunDefaultInput;
    public fps_CharacterController FPSCharacterController;

    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, recoilForce, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public Transform gunFirePoint;

    // bools 
    public bool shooting, readyToShoot, reloading, noAmmoCooldown;

    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public Animator animator;
    public LayerMask whatIsEnemy, groundLayer, playerLayer;

    // Graphics
    [Header("Muzzles")] 
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public GameObject groundBulletHolePrefab;
    public GameObject enemyBulletHolePrefab;
    public GameObject defaultBulletHolePrefab;
    
    [Header("Ammo UI Text")] 
    public TextMeshProUGUI ammoUI;
    public Sprite icon;
    public string label;

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
    public Quaternion hipfireRotation;
    public GameObject sightTransform;
    public Vector3 adsPosition,adsRotation;
    public float adsSpeed;
    public Image crosshair;
    public float camFovAds, camFovDefault;
    public bool isAds;
    public bool isAiming;

    // Animator fpsAnim;
    [Header("Camera Recoil Settings")]
    public Vector3 gunRecoilRotation;
    public Vector3 gunRecoilRotationAiming;
    public float gunRotationSpeed;
    public float gunReturnSpeed;

    private void OnEnable()
    {
        gunCamRecoilUpdate();
        gunDefaultInput.Enable();
    }

    private void OnDisable()
    {
        gunDefaultInput.Disable();
        isAiming = false;
    }

    private void Awake()
    {
        gunDefaultInput = new DefaultInput();

        #region - ADS Key -

        gunDefaultInput.Weapon.ADSPressed.performed += e => ADSInPressed();
        gunDefaultInput.Weapon.ADSReleased.performed += e => ADSInReleased();

        #endregion

        hipfirePosition = weaponModel.transform.localPosition;
        hipfireRotation = weaponModel.transform.localRotation;
        adsPosition = AdsPositionTransform.transform.localPosition;
        camRecoil = GameObject.Find("CameraRecoil").GetComponent<AdvancedCamRecoil>();
        cam = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>();
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        animator = GetComponent<Animator>();
        gunFirePoint = GameObject.Find("Raycast").GetComponentInChildren<Transform>();
        
        camFovDefault = 75;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        audioSource = GetComponent<AudioSource>();
        ammoUI = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (FPSCharacterController == null)
        {
            FPSCharacterController = GameObject.FindWithTag("Player").GetComponent<fps_CharacterController>();
        }
        
    }

    private void Update()
    {
        
        // SetText
        ammoUI.SetText(bulletsLeft + " / " + magazineSize);
        sprintAnim();
    }

    public void gunCamRecoilUpdate()
    {
        camRecoil.RecoilRotation = gunRecoilRotation;
        camRecoil.RecoilRotationAiming = gunRecoilRotationAiming;
        camRecoil.rotationSpeed = gunRotationSpeed;
        camRecoil.returnSpeed = gunReturnSpeed;
    }

    #region MY INPUT
    public void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = gunDefaultInput.Weapon.Fire.ReadValue<float>() > timeBetweenShooting;
        }
        else
        {
            shooting = gunDefaultInput.Weapon.Fire.triggered;
        }

        if (gunDefaultInput.Weapon.Reload.triggered && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // Shoot
        if (readyToShoot && shooting && !FPSCharacterController.isSprint &&!reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            StartCoroutine(Shoot(cam.transform.forward, cam.transform.position));
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
                if (noAmmoSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(noAmmoSound);
                }
                noAmmoCooldown = true;
                Invoke("ResetNoAmmoCooldown", 0.25f);
            }
        }
    }
    
    #endregion

    #region - ADS -
    public void ADS()
    {
        if (isAiming && !reloading && !FPSCharacterController.isSprint)
        {
            isAds = true;
            weaponModel.transform.localPosition = Vector3.Lerp(weaponModel.transform.localPosition, adsPosition, Time.deltaTime * adsSpeed);
            weaponModel.transform.localRotation = Quaternion.Lerp(weaponModel.transform.localRotation, Quaternion.Euler(adsRotation), Time.deltaTime * adsSpeed); // Apply local rotation
        
            crosshair.color = new Color(0, 0, 0, 0);
            camRecoil.aiming = true;
            weaponRecoil.aiming = true;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFovAds, Time.deltaTime * adsSpeed);
        }
        else
        {
            isAds = false;
            weaponModel.transform.localPosition = Vector3.Lerp(weaponModel.transform.localPosition, hipfirePosition, Time.deltaTime * adsSpeed);
            weaponModel.transform.localRotation = Quaternion.Lerp(weaponModel.transform.localRotation, hipfireRotation, Time.deltaTime * adsSpeed); // Reset local rotation
        
            crosshair.color = new Color(255, 255, 255, 255);
            camRecoil.aiming = false;
            weaponRecoil.aiming = false;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFovDefault, Time.deltaTime * adsSpeed);
        }
    }

    private void ADSInPressed()
    {
        isAiming = true;
    } private void ADSInReleased()
    {
        isAiming = false;
    }

    #endregion
    
    #region - SHOOT -
    
    public IEnumerator Shoot(Vector3 target, Vector3 AttackPoint)
    {
        readyToShoot = false;
        animator.SetBool("Fire",true);

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate Direction with Spread

        // RayCastif (Physics.Raycast(cam.transform.position, target, out rayHit, range))
        if (Physics.Raycast(AttackPoint, target + new Vector3(x,y,0), out rayHit, range))
        {
            if (rayHit.collider.CompareTag("Enemy"))
            {
                // rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
            }
            
            var rb2d = rayHit.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(target*20,rayHit.point,ForceMode.Impulse);
            }
            var enemyHitBox = rayHit.collider.GetComponent<enemyHitbox>();
            if (enemyHitBox)
            {
                enemyHitBox.onRaycastHit(this, target);
            }

            if (rayHit.collider.CompareTag("Player"))
            {
                
            }
            createBulletHole();
        }

        // Graphics
        GameObject muzzleFlashLocate = Instantiate(muzzleFlash, attackPoint.position, attackPoint.transform.rotation);
        muzzleFlashLocate.transform.SetParent(attackPoint);  // Muzzle flash'ı silahın attackPoint'ine ekle
        Destroy(muzzleFlashLocate, 1f);

        bulletsLeft--;
        bulletsShot--;
        
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            StartCoroutine(Shoot(target, AttackPoint));
            readyToShoot = false;
        }
        
        yield return new WaitForSeconds(timeBetweenShooting);
        readyToShoot = true;
        animator.SetBool("Fire",false);
        
    }
    #endregion

    #region - SOUND -
    public void playShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    #endregion

    #region BULLET HOLE

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
        }else if ((playerLayer.value & (1 << hitLayer)) > 0)
        {
            bulletHolePrefab = null;
            return;
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


    #endregion
    public void ResetShot()
    {
        readyToShoot = true;
        animator.SetBool("Fire",false);
    }

    private void ResetNoAmmoCooldown()
    {
        noAmmoCooldown = false;
    }

    private void Reload()
    {
        reloading = true;
        audioSource.PlayOneShot(reloadSound);
        animator.SetBool("isReload", true);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        animator.SetBool("isReload", false);

    }

    private void sprintAnim()
    {
        if (FPSCharacterController.isSprint)
        {
        animator.SetBool("isSprint",true);    
        }
        else
        {
            animator.SetBool("isSprint",false);    

        }
    }
}
