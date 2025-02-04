using System.Threading;

namespace Code.WeaponFSM
{
    public class WeaponStateEquip: WeaponState
    {
        private CancellationTokenSource _equipCts;
        public WeaponStateEquip(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }

        public override bool CanEnterState => !Weapon.EquipInProcess();
        public override bool CanExitState => !Weapon.EquipInProcess();
        public override void OnEnterState()
        {
            _equipCts = new CancellationTokenSource();
            _ = Weapon.EquipAsync(_equipCts.Token);
        }

        public override void OnExitState()
        {
            _equipCts?.Cancel();
            _equipCts?.Dispose();
            _equipCts = null;
        }
    }
}