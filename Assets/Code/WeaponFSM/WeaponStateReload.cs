using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateReload : WeaponState
    {
        private CancellationTokenSource _reloadCts;
        public WeaponStateReload(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }
    
        public override bool CanEnterState => Weapon.CanReload();
        public override bool CanExitState => !Weapon.ReloadStateInProcess;
        public override void OnEnterState()
        {
            _reloadCts = new CancellationTokenSource();
            
            Weapon.ReloadAsync(_reloadCts.Token).Forget(UnityEngine.Debug.LogException);
        }
        public override void OnExitState()
        {
            _reloadCts?.Cancel();
            _reloadCts?.Dispose();
            _reloadCts = null;
        }
    }
}