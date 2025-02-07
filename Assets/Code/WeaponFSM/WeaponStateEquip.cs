using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public class WeaponStateEquip: WeaponState
    {
        private CancellationTokenSource _equipCts;
        public WeaponStateEquip(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }

        public override bool CanEnterState => Weapon.CanEquip();
        public override bool CanExitState => !Weapon.EquipStateInProcess;
        public override void OnEnterState()
        {
            _equipCts = new CancellationTokenSource();

            Weapon.EquipAsync(_equipCts.Token).Forget(UnityEngine.Debug.LogException);
        }
        public override void OnExitState()
        {
            _equipCts?.Cancel();
            _equipCts?.Dispose();
            _equipCts = null;
        }
    }
}