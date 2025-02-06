using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateEquip: WeaponState
    {
        private CancellationTokenSource _equipCts;
        private bool _isCompleted;
        public WeaponStateEquip(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }

        public override bool CanEnterState => Weapon.CanEquip();
        public override bool CanExitState => _isCompleted;
        public override void OnEnterState()
        {
            _equipCts = new CancellationTokenSource();
            
            EquipProcess().Forget(UnityEngine.Debug.LogException);
        }
        private async UniTask EquipProcess()
        {
            _isCompleted = false;
            try
            {
                await Weapon.EquipAsync(_equipCts.Token);
            }
            finally
            {
                _isCompleted = true;
                WeaponFsm.TrySetState<WeaponStateIdle>();
            }
        }
        public override void OnExitState()
        {
            _equipCts?.Cancel();
            _equipCts?.Dispose();
            _equipCts = null;
        }
    }
}