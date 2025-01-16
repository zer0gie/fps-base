using System;
using Animancer;
using Resources.Code.ScriptableObjects;
using Resources.Code.UI;
using Resources.Code.WeaponFSM;
using UnityEngine;
using Zenject;

namespace Resources.Code.Weapon;

public class Weapon : MonoBehaviour
{
    [Header("Global Settings")] 
    [SerializeField] private WeaponSettingsSO weaponSettings;

    [Header("Weapon Shoot Mode")] 
    [SerializeField] private ShootMode weaponShootMode;

    [Header("Shooting effects")] 
    [SerializeField] private GameObject muzzleEffect;

    private ParticleSystem _muzzleParticles;
    
    private WeaponStateMachine _weaponStateMachine;
    
    private int _currentAmmo;
    private int _stackAmmo;
    
    private bool _isShootingExecuted;
    private bool _isReloading;
    private bool _isShooting;
    private bool _initFinished;

    [Header("Bullet Settings")] 
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform bulletSpawn;

    [Header("Animancer")] 
    [SerializeField] private AnimancerComponent weaponAnimancer;
    private AnimancerState _currentAnimState;

    // Dependencies
    private IAudioManager _audioManager;
    private IUIManager _uiManager;
    private SignalBus _signalBus;
    private IBulletManager _bulletManager;

    [Inject]
    public void Construct(IAudioManager audioManager, SignalBus signalBus, IUIManager uiManager,
        IBulletManager bulletManager)
    {
        _audioManager = audioManager;
        _signalBus = signalBus;
        _uiManager = uiManager;
        _bulletManager = bulletManager;

        _currentAmmo = weaponSettings.clipSize;
        _stackAmmo = weaponSettings.stackSize;
        _muzzleParticles = muzzleEffect.GetComponent<ParticleSystem>();
        InitializeStateMachine();
    }
    protected void OnEnable()
    {
        _signalBus.Subscribe<FireActionStartedSignal>(OnFireStarted);
        _signalBus.Subscribe<FireActionCancelledSignal>(OnFireCancelled);
        _signalBus.Subscribe<ReloadPerformedSignal>(OnReloadPerformed);
        
        _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
        _uiManager.ShowAmmoPanel();
        
        _weaponStateMachine.TrySetState<WeaponStateInit>();
    }

    protected void OnDisable()
    {
        weaponAnimancer.Stop();
        _weaponStateMachine.DisableWeaponStateMachine();
        
        _uiManager.HideAmmoPanel(); 
        _audioManager.StopWeaponSounds();
        
        _signalBus.Unsubscribe<FireActionStartedSignal>(OnFireStarted);
        _signalBus.Unsubscribe<FireActionCancelledSignal>(OnFireCancelled);
        _signalBus.Unsubscribe<ReloadPerformedSignal>(OnReloadPerformed);
    }
    private void OnReloadPerformed()
    {
        _weaponStateMachine.TrySetState<WeaponStateReload>();
    }

    private void OnFireCancelled()
    {
        _isShootingExecuted = false;
    }

    private void OnFireStarted()
    {
        _isShootingExecuted = true;
        _weaponStateMachine.TrySetState<WeaponStateShoot>();
    }
    private void PlayAnimation(ClipTransition clip, Action onEnd = null)
    {
        _currentAnimState = weaponAnimancer.Play(clip, 0.25f);
        if (onEnd != null)
        {
            _currentAnimState.Events(this).OnEnd = onEnd;
        }
    }

    private void InitializeStateMachine()
    {
        _weaponStateMachine = new WeaponStateMachine();
        _weaponStateMachine.AddState(new WeaponStateIdle(_weaponStateMachine, this));
        _weaponStateMachine.AddState(new WeaponStateReload(_weaponStateMachine, this));
        _weaponStateMachine.AddState(new WeaponStateShoot(_weaponStateMachine, this));
        _weaponStateMachine.AddState(new WeaponStateInit(_weaponStateMachine, this));
    }

    public void InitWeapon()
    {
        _initFinished = false;
        PlayAnimation(weaponSettings.takeAnimation, () =>
        {
            _initFinished = true;
            _weaponStateMachine.TrySetState<WeaponStateIdle>();
        });
    }
    public void Idle()
    {
        PlayAnimation(weaponSettings.idleAnimation);
    }
    public void Shoot()
    {
        _isShooting = true;

        if (_currentAmmo <= 0)
        {
            _audioManager.PlayWeaponSound(weaponSettings.emptyMagazineSound);
            
            _weaponStateMachine.TrySetState<WeaponStateIdle>();

            return;
        }
        
        _muzzleParticles.Play();
        
        _audioManager.PlayWeaponSound(weaponSettings.shootingSound);
        
        PlayAnimation(weaponSettings.shootAnimation, EndShoot);       

        var bulletsToShoot = BulletsPerShot();

        if (bulletsToShoot > _currentAmmo) bulletsToShoot = _currentAmmo; // For shotguns

        // Firing bullets
        for (var i = bulletsToShoot; i > 0; i--)
        {
            var shootingDirection = CalculateSpreadAndDirection();
            
            var bulletProjectile = _bulletManager.GetBullet(bulletSpawn.position);

            if (bulletProjectile.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                bulletProjectile.transform.forward = shootingDirection;

                rb.AddForce(shootingDirection * weaponSettings.bulletVelocity, ForceMode.Impulse);
            }

            _currentAmmo--;
        }

        _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
    }
    private void EndShoot()
    {
        switch (weaponShootMode)
        {
            case ShootMode.Single:
                _isShootingExecuted = false;
                _weaponStateMachine.TrySetState<WeaponStateIdle>();
                return;
            case ShootMode.Auto:
                if (_isShootingExecuted)
                {
                    _weaponStateMachine.RestartState();
                }
                else
                {
                    _weaponStateMachine.TrySetState<WeaponStateIdle>();
                }
                return;
            case ShootMode.Shotgun:
                _isShootingExecuted = false;
                _weaponStateMachine.TrySetState<WeaponStateIdle>();
                return;
            default:
                Debug.LogError("Weapon is not configured.");
                return;
        }
    }
    private int BulletsPerShot()
    {
        return weaponShootMode switch
        {
            
            ShootMode.Single => 1,
            ShootMode.Auto => 1,
            ShootMode.Shotgun => 6,
            _ => 0
        };
    }
    private Vector3 CalculateSpreadAndDirection()
    {
        if (Camera.main == null) return Vector3.zero;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        var targetPoint = Physics.Raycast(ray, out var hit)
            ? hit.point
            : ray.GetPoint(weaponSettings.maxShootDistance);
        var direction = targetPoint - bulletSpawn.position;
        var x = UnityEngine.Random.Range(-weaponSettings.spreadIntensity, weaponSettings.spreadIntensity);
        var y = UnityEngine.Random.Range(-weaponSettings.spreadIntensity, weaponSettings.spreadIntensity);

        var spread = bulletSpawn.TransformDirection(new Vector3(x, y, 0));

        return (direction + spread).normalized;
    }
    public void Reload()
    {
        _isReloading = true;
        
        _audioManager.PlayWeaponSound(weaponSettings.reloadSound);
        
        PlayAnimation(weaponSettings.reloadAnimation, FinishReload);
    }
    private void FinishReload()
    {
        if (_stackAmmo >= weaponSettings.clipSize - _currentAmmo)
        {
            _stackAmmo -= weaponSettings.clipSize - _currentAmmo;
            _currentAmmo = weaponSettings.clipSize;
        }
        else
        {
            _currentAmmo += _stackAmmo;
            _stackAmmo = 0;
        }

        _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);

        _isReloading = false;
        
        _weaponStateMachine.TrySetState<WeaponStateIdle>();
    }
    public void ResetWeapon()
    {
        _isShooting = false;
        _isReloading = false;
    }
    public bool ReadyToStartShooting()
    {
        return (_isShootingExecuted && !_isShooting && !_isReloading && _currentAmmo > 0);
    }

    public bool ReadyToStartReload()
    {
        return (_currentAmmo < weaponSettings.clipSize && !_isReloading && _stackAmmo > 0);
    }

    public bool ReadyToAutoReload()
    {
        return (_currentAmmo <= 0 && !_isReloading && _stackAmmo > 0);
    }

    public bool IsShooting()
    {
        return _isShooting;
    }
    public bool IsReloading()
    {
        return _isReloading;
    }

    public bool InitFinished()
    {
        return _initFinished;
    }

    private enum ShootMode
    {
        Single,
        Auto,
        Shotgun,
    }
}