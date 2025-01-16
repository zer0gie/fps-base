namespace Resources.Code.WeaponFSM;

public class WeaponStateInit : WeaponState
{
    public WeaponStateInit(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
    {
    }

    public override bool CanExitState => Weapon.InitFinished();
    public override void OnEnterState()
    {
        Weapon.InitWeapon();
    }

    public override void OnExitState()
    {
        Weapon.ResetWeapon();
    }
}