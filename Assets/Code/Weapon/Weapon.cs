using System;
using System.Threading;
using Animancer;
using Code.Audio;
using Code.PlayerInput;
using Code.ScriptableObjects;
using Code.UI;
using Code.WeaponFSM;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Code.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] private WeaponConfigSO weaponConfig;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ShootMode weaponShootMode;
        
        [Header("Scene context")] 
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private AnimancerComponent weaponAnimancer;
        
        private ParticleSystem _weaponParticleSystem;
        private WeaponFSM.WeaponFSM _weaponFsm;
        
        public bool EquipStateInProcess { get; private set; }
        public bool ReloadStateInProcess { get; private set; }
        public bool ShootStateInProcess { get; private set; }

        private bool _isShootingButtonHolding;
        
        private int _currentAmmo;
        private int _stackAmmo;
        
        // Dependencies
        private IAudioManager _audioManager;
        private IUIManager _uiManager;
        private SignalBus _signalBus;
        private IBulletManager _bulletManager;
        private PlayerController _playerController;

        [Inject]
        public void Construct(IAudioManager audioManager, SignalBus signalBus, IUIManager uiManager,
            IBulletManager bulletManager, PlayerController playerController)
        {
            _signalBus = signalBus;
            _playerController = playerController;
            _bulletManager = bulletManager;
            _uiManager = uiManager;
            _audioManager = audioManager;
            _currentAmmo = weaponConfig.clipSize;
            _stackAmmo = weaponConfig.stackSize;
            _weaponParticleSystem = muzzleFlash.GetComponent<ParticleSystem>();
            InitializeWeaponStateMachine();
        }
        protected void OnEnable()
        {
            UniTask.SwitchToMainThread();
            _signalBus.Subscribe<FireActionStartedSignal>(OnFireStarted);
            _signalBus.Subscribe<FireActionCancelledSignal>(OnFireCancelled);
            _signalBus.Subscribe<ReloadPerformedSignal>(OnReloadPerformed);
            _weaponFsm.TrySetState<WeaponStateEquip>();
        }
        protected void OnDisable()
        {
            _weaponFsm.ForceSetState<WeaponStateInactive>();
            _uiManager.HideHUD(); 
            _audioManager.StopWeaponSounds();
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
        public bool CanEquip()
        {
            return !weaponAnimancer.IsPlaying(weaponConfig.takeAnimation);
        }
        public bool CanShoot()
        {
            return !weaponAnimancer.IsPlaying(weaponConfig.shootAnimation);
        }

        public bool CanReload()
        {
            return (!weaponAnimancer.IsPlaying(weaponConfig.reloadAnimation) && _currentAmmo < weaponConfig.clipSize && _stackAmmo > 0);
        }
        public bool ReadyToAutoReload()
        {
            return (_currentAmmo <= 0 && _stackAmmo > 0);
        }

        public async UniTask EquipAsync(CancellationToken cancellationToken)
        {
            EquipStateInProcess = true;
            try
            {
                _audioManager.PlayWeaponSound(weaponConfig.equipSound);
                var state = weaponAnimancer.Play(weaponConfig.takeAnimation);
                _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
                _uiManager.ShowHUD();
                await UniTask.WaitWhile(() => state.NormalizedTime < 1, cancellationToken: cancellationToken);
                
                EquipStateInProcess = false;
                _weaponFsm.TrySetState<WeaponStateIdle>();
            }
            catch (OperationCanceledException)
            {
                EquipStateInProcess = false;
                Debug.Log("Weapon.EquipAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.EquipWeaponAsync(): {e.Message}");
            }
        }
        public void Idle()
        {
            weaponAnimancer.Play(weaponConfig.idleAnimation, 0.25f);
        }

        public async UniTask ShootingAsync(CancellationToken cancellationToken)
        {
            ShootStateInProcess = true;
            try
            {

                if (_currentAmmo <= 0) // Empty magazine handle
                {
                    _audioManager.PlayWeaponSound(weaponConfig.emptyMagazineSound);
                    ShootStateInProcess = false;
                    _weaponFsm.TrySetState<WeaponStateIdle>();
                    return;
                }
                switch (weaponShootMode)
                {
                    case ShootMode.Auto:
                        while (_isShootingButtonHolding && _currentAmmo > 0)
                        {
                            weaponAnimancer.Stop();
                            var autoState = weaponAnimancer.Play(weaponConfig.shootAnimation, 0.25f);
                            ExecuteShot();
                            
                            await UniTask.WaitWhile(() => autoState.NormalizedTime < 1, 
                                cancellationToken: cancellationToken);
                        }
                        ShootStateInProcess = false;
                        _weaponFsm.TrySetState<WeaponStateIdle>();
                        break;
                    
                    case ShootMode.Single:
                    case ShootMode.Shotgun:
                        var state = weaponAnimancer.Play(weaponConfig.shootAnimation, 0.25f);
                        ExecuteShot();
                        
                        await UniTask.WaitWhile(() => state.NormalizedTime < 1,
                            cancellationToken: cancellationToken);
                        ShootStateInProcess = false;
                        _weaponFsm.TrySetState<WeaponStateIdle>();
                        break;
                    default:
                        Debug.LogError("WEAPON ERROR: Unknown ShootMode");
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                ShootStateInProcess = false;
                Debug.Log("Weapon.ShootingAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.ShootingAsync(): {e.Message}");
            }
        }
        private void ExecuteShot()
        {
            _audioManager.PlayWeaponSound(weaponConfig.shootingSound);
            _weaponParticleSystem.Play();
            _playerController.ApplyRecoil(weaponConfig.recoilAmount);
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
            rb.AddForce(shootingDirection * weaponConfig.bulletVelocity, ForceMode.Impulse);

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
                : ray.GetPoint(weaponConfig.maxShootDistance);
        }

        private Vector3 CalculateSpread()
        {
            var x = UnityEngine.Random.Range(-weaponConfig.spreadIntensity, weaponConfig.spreadIntensity);
            var y = UnityEngine.Random.Range(-weaponConfig.spreadIntensity, weaponConfig.spreadIntensity);
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
            ReloadStateInProcess = true;
            try
            {
                _audioManager.PlayWeaponSound(weaponConfig.reloadSound);
                var state = weaponAnimancer.Play(weaponConfig.reloadAnimation, 0.25f);
                await UniTask.WaitWhile(() => state.NormalizedTime < 1, cancellationToken: cancellationToken);
                
                UpdateAmmoAfterReload();
                _uiManager.UpdateAmmoPanel(_currentAmmo, _stackAmmo);
                ReloadStateInProcess = false;
                _weaponFsm.TrySetState<WeaponStateIdle>();
            }
            catch (OperationCanceledException)
            {
                ReloadStateInProcess = false;
                Debug.Log("Weapon.ReloadAsync() was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR Weapon.ReloadAsync(): {e.Message}");
            }
        }

        private void UpdateAmmoAfterReload()
        {
            if (_stackAmmo >= weaponConfig.clipSize - _currentAmmo)
            {
                _stackAmmo -= weaponConfig.clipSize - _currentAmmo;
                _currentAmmo = weaponConfig.clipSize;
            }
            else
            {
                _currentAmmo += _stackAmmo;
                _stackAmmo = 0;
            }
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
        private enum ShootMode
        {
            Single,
            Auto,
            Shotgun,
        }
    }
}