using System.Threading;

namespace Code.WeaponFSM
{
    public class WeaponStateReload : WeaponState
    {
        private CancellationTokenSource _reloadCts;
        public WeaponStateReload(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }
    
        public override bool CanEnterState => Weapon.CanReload();
        public override bool CanExitState => !Weapon.ReloadingInProcess();
        public override void OnEnterState()
        {
            _reloadCts = new CancellationTokenSource();
            _ = Weapon.ReloadAsync(_reloadCts.Token);
        }

        public override void OnExitState()
        {
            _reloadCts?.Cancel();
            _reloadCts?.Dispose();
            _reloadCts = null;
        }
    }
}