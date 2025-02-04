using System.Threading;
using UnityEngine;

namespace Code.WeaponFSM
{
    public class WeaponStateShoot : WeaponState
    {
        private CancellationTokenSource _shootingCts;

        public WeaponStateShoot(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }

        public override bool CanEnterState => Weapon.CanShoot();
        public override bool CanExitState => !Weapon.ShootingInProcess();
        public override void OnEnterState()
        {
            _shootingCts = new CancellationTokenSource();
            _ = Weapon.ShootingAsync(_shootingCts.Token);
        }

        public override void OnExitState()
        {
            _shootingCts?.Cancel();
            _shootingCts?.Dispose();
            _shootingCts = null;
        }
    }
}