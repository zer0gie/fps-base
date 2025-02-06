using System;
using System.Threading;
using Animancer;
using Code.Audio;
using Code.Player;
using Code.ScriptableObjects;
using Code.UI;
using Code.WeaponFSM;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [Header("Global Settings")] 
        [SerializeField] private WeaponSettingsSO weaponSettings;

        [Header("Weapon Shoot Mode")] 
        [SerializeField] private ShootMode weaponShootMode;

        [Header("Shooting effects")] 
        [SerializeField] private GameObject muzzleEffect;

        private ParticleSystem _weaponParticleSystem;
    
        private WeaponFSM.WeaponFSM _weaponFsm;
    
        private int _currentAmmo;
        private int _stackAmmo;
        
        private bool _isShootingButtonHolding;

        [Header("Bullet Settings")] 
        [SerializeField] private GameObject bulletPrefab;

        [SerializeField] private Transform bulletSpawn;

        [Header("Animancer")] 
        [SerializeField] private AnimancerComponent weaponAnimancer;

        // Dependencies
        private IAudioManager _audioManager;
        private IUIManager _uiManager;
        private SignalBus _signalBus;
        private IBulletManager _bulletManager;
        private PlayerController _playerController;
        
        private const int EMPTY_MAGAZINE_SOUND_DELAY = 150;

        [Inject]
        public void Construct(IAudioManager audioManager, SignalBus signalBus, IUIManager uiManager,
            IBulletManager bulletManager, PlayerController playerController)
        {
            _audioManager = audioManager;
            _signalBus = signalBus;
            _uiManager = uiManager;
            _bulletManager = bulletManager;
            _playerController = playerController;

            _currentAmmo = weaponSettings.clipSize;
            _stackAmmo = weaponSettings.stackSize;
            _weaponParticleSystem = muzzleEffect.GetComponent<ParticleSystem>();

            InitializeWeaponStateMachine();
        }
        protected void OnEnable()
        {
            UniTask.SwitchToMainThread();
            _signalBus.Subscribe<FireActionStartedSignal>(OnFireStarted);
            _signalBus.Subscribe<FireActionCancelledSignal>(OnFireCancelled);
            _signalBus.Subscribe<ReloadPerformedSignal>(OnReloadPerformed);
        
            _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
            _uiManager.ShowAmmoPanel();
            
            _weaponFsm.TrySetState<WeaponStateEquip>();
        }
        protected void OnDisable()
        {
            weaponAnimancer.Stop();
            _audioManager.StopWeaponSounds();
            _weaponFsm.ForceSetState<WeaponStateInactive>();
            
            _uiManager.HideAmmoPanel(); 
        
            _signalBus.Unsubscribe<FireActionStartedSignal>(OnFireStarted);
            _signalBus.Unsubscribe<FireActionCancelledSignal>(OnFireCancelled);
            _signalBus.Unsubscribe<ReloadPerformedSignal>(OnReloadPerformed);
        }
        private void OnReloadPerformed()
        {
            _weaponFsm.TrySetState<WeaponStateReload>();
        }
    
        private void OnFireCancelled()
        {
            _isShootingButtonHolding = false;
        }
    
        private void OnFireStarted()
        {
            _isShootingButtonHolding = true;
            _weaponFsm.TrySetState<WeaponStateShoot>();
        }
    
        private void InitializeWeaponStateMachine()
        {
            _weaponFsm = new WeaponFSM.WeaponFSM();
            _weaponFsm.AddState(new WeaponStateIdle(_weaponFsm, this));
            _weaponFsm.AddState(new WeaponStateReload(_weaponFsm, this));
            _weaponFsm.AddState(new WeaponStateShoot(_weaponFsm, this));
            _weaponFsm.AddState(new WeaponStateEquip(_weaponFsm, this));
            _weaponFsm.AddState(new WeaponStateInactive(_weaponFsm, this));
        }

        public async UniTask EquipAsync(CancellationToken cancellationToken)
        {
            try
            {
                _audioManager.PlayWeaponSound(weaponSettings.equipSound);
                var state = weaponAnimancer.Play(weaponSettings.takeAnimation, 0.25f);
                await UniTask.WaitWhile(() => state.IsPlayingAndNotEnding(), cancellationToken: cancellationToken);
                
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Weapon.EquipAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.EquipWeaponAsync(): {e.Message}");
            }
        }
        public void Idle()
        {
            weaponAnimancer.Play(weaponSettings.idleAnimation, 0.25f);
        }

        public async UniTask ShootingAsync(CancellationToken cancellationToken)
        {
            try
            {

                if (_currentAmmo <= 0) // Empty magazine handle
                {
                    _audioManager.PlayWeaponSound(weaponSettings.emptyMagazineSound);
                    await UniTask.Delay(EMPTY_MAGAZINE_SOUND_DELAY, 
                        cancellationToken: cancellationToken);
                    
                    return;
                }
                switch (weaponShootMode)
                {
                    case ShootMode.Auto:
                        while (_isShootingButtonHolding && _currentAmmo > 0)
                        {
                            weaponAnimancer.Stop();
                            var autoState = weaponAnimancer.Play(weaponSettings.shootAnimation);
                            ExecuteShot();
                            
                            await UniTask.WaitWhile(() => autoState.IsPlayingAndNotEnding(),
                                cancellationToken: cancellationToken);
                        }
                        break;
                    
                    case ShootMode.Single:
                    case ShootMode.Shotgun:
                        var state = weaponAnimancer.Play(weaponSettings.shootAnimation);
                        ExecuteShot();
                        
                        await UniTask.WaitWhile(() => state.IsPlayingAndNotEnding(), 
                            cancellationToken: cancellationToken);
                        break;
                    default:
                        Debug.LogError("WEAPON ERROR: Unknown ShootMode");
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Weapon.ShootingAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.ShootingAsync(): {e.Message}");
            }
        }
        private void ExecuteShot()
        {
            _audioManager.PlayWeaponSound(weaponSettings.shootingSound);
            _weaponParticleSystem.Play();
            _playerController.ApplyRecoil(weaponSettings.recoilAmount);
            FireShot();
            _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
        }
        private void FireShot()
        {
            var bulletsInSameShot = Mathf.Min(BulletsPerShot(), _currentAmmo);
            
            for (var i = 0; i < bulletsInSameShot; i++)
            {
                FireSingleBullet();
                _currentAmmo--;
            }
        }

        private void FireSingleBullet()
        {
            var shootingDirection = CalculateSpreadAndDirection();
            var bullet = _bulletManager.GetBullet(bulletSpawn.position);

            if (!bullet.TryGetComponent<Rigidbody>(out var rb)) return;
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.transform.forward = shootingDirection;
            rb.AddForce(shootingDirection * weaponSettings.bulletVelocity, ForceMode.Impulse);

        }
        private Vector3 CalculateSpreadAndDirection()
        {
            if (Camera.main == null) return Vector3.zero;
            
            var targetPoint = GetTargetPoint();
            var direction = targetPoint - bulletSpawn.position;
            var spread = CalculateSpread();

            return (direction + spread).normalized;
        }

        private Vector3 GetTargetPoint()
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            return Physics.Raycast(ray, out var hit) 
                ? hit.point 
                : ray.GetPoint(weaponSettings.maxShootDistance);
        }

        private Vector3 CalculateSpread()
        {
            var x = UnityEngine.Random.Range(-weaponSettings.spreadIntensity, weaponSettings.spreadIntensity);
            var y = UnityEngine.Random.Range(-weaponSettings.spreadIntensity, weaponSettings.spreadIntensity);
            return bulletSpawn.TransformDirection(new Vector3(x, y, 0));
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

        public async UniTask ReloadAsync(CancellationToken cancellationToken)
        {
            try
            {
                _audioManager.PlayWeaponSound(weaponSettings.reloadSound);
            
                var state = weaponAnimancer.Play(weaponSettings.reloadAnimation, 0.25f);
                await UniTask.WaitWhile(() => state.IsPlayingAndNotEnding(), 
                    cancellationToken: cancellationToken);

                UpdateAmmoAfterReload();
                _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Weapon.ReloadAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.ReloadAsync(): {e.Message}");
            }
        }

        private void UpdateAmmoAfterReload()
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
        }
        public bool CanEquip()
        {
            return !weaponAnimancer.IsPlaying(weaponSettings.takeAnimation);
        }
        public bool CanShoot()
        {
            return !weaponAnimancer.IsPlaying(weaponSettings.shootAnimation);
        }

        public bool CanReload()
        {
            return (!weaponAnimancer.IsPlaying(weaponSettings.reloadAnimation) && _currentAmmo < weaponSettings.clipSize && _stackAmmo > 0);
        }

        public bool ReadyToAutoReload()
        {
            return (_currentAmmo <= 0 && _stackAmmo > 0);
        }
        
        private enum ShootMode
        {
            Single,
            Auto,
            Shotgun,
        }
    }
}