namespace Resources.Code.WeaponFSM;

public class WeaponStateReload : WeaponState
{
    public WeaponStateReload(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
    {
    }
    
    public override bool CanEnterState => Weapon.ReadyToStartReload();
    public override bool CanExitState => !Weapon.IsReloading();
    public override void OnEnterState()
    {
        Weapon.Reload();
    }

    public override void OnExitState()
    {
        Weapon.ResetWeapon();
    }
}