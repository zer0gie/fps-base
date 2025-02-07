using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateShoot : WeaponState
    {
        private CancellationTokenSource _shootingCts;
        public WeaponStateShoot(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }

        public override bool CanEnterState => Weapon.CanShoot();
        public override bool CanExitState => !Weapon.ShootStateInProcess;
        public override void OnEnterState()
        {
            _shootingCts = new CancellationTokenSource();

            Weapon.ShootingAsync(_shootingCts.Token).Forget(UnityEngine.Debug.LogException);
        }
        public override void OnExitState()
        {
            _shootingCts?.Cancel();
            _shootingCts?.Dispose();
            _shootingCts = null;
        }
    }
}