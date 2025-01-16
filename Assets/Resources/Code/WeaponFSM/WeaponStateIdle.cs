namespace Resources.Code.WeaponFSM;

public sealed class WeaponStateIdle : WeaponState
{
    public WeaponStateIdle(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
    {
    }
    
    public override void OnEnterState()
    {
        Weapon.Idle();
        
        if (Weapon.ReadyToAutoReload())
        {
            WeaponStateMachine.TrySetState<WeaponStateReload>();
        }
    }
}