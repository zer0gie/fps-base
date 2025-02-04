using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateReload : WeaponState
    {
        private CancellationTokenSource _reloadCts;
        private bool _isCompleted;
        public WeaponStateReload(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }
    
        public override bool CanEnterState => Weapon.CanReload();
        public override bool CanExitState => _isCompleted;
        public override void OnEnterState()
        {
            _reloadCts = new CancellationTokenSource();
            
            ReloadProcess().Forget(UnityEngine.Debug.LogException);
        }
        private async UniTask ReloadProcess()
        {
            _isCompleted = false;
            try
            {
                await Weapon.ReloadAsync(_reloadCts.Token);
            }
            finally
            {
                _isCompleted = true;
                WeaponStateMachine.TrySetState<WeaponStateIdle>();
            }
        }
        public override void OnExitState()
        {
            _reloadCts?.Cancel();
            _reloadCts?.Dispose();
            _reloadCts = null;
        }
    }
}