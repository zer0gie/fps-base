namespace Code.WeaponFSM
{
    public sealed class WeaponStateIdle : WeaponState
    {
        public WeaponStateIdle(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }

        public override void OnEnterState()
        {
            Weapon.Idle();
            
            if (Weapon.ReadyToAutoReload())
            {
                // Auto reload weapon
                //WeaponStateMachine.TrySetState<WeaponStateReload>();
            }
        }
    }
}