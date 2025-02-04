using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateShoot : WeaponState
    {
        private CancellationTokenSource _shootingCts;
        private bool _isCompleted;
        public WeaponStateShoot(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }

        public override bool CanEnterState => Weapon.CanShoot();
        public override bool CanExitState => _isCompleted;
        public override void OnEnterState()
        {
            _shootingCts = new CancellationTokenSource();

            ShootProcess().Forget(UnityEngine.Debug.LogException);
        }

        private async UniTask ShootProcess()
        {
            _isCompleted = false;
            try
            {
                await Weapon.ShootingAsync(_shootingCts.Token);
            }
            finally
            {
                _isCompleted = true;
                WeaponStateMachine.TrySetState<WeaponStateIdle>();
            }
        }
        public override void OnExitState()
        {
            _shootingCts?.Cancel();
            _shootingCts?.Dispose();
            _shootingCts = null;
        }
    }
}