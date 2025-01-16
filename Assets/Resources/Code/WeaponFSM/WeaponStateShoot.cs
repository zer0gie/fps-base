namespace Resources.Code.WeaponFSM;

public class WeaponStateShoot : WeaponState
{
    public WeaponStateShoot(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
    {
    }
    
    public override bool CanEnterState => Weapon.ReadyToStartShooting();
    public override bool CanExitState => !Weapon.IsShooting();

    public override void OnEnterState()
    {
        Weapon.Shoot();
    }

    public override void OnExitState()
    {
        Weapon.ResetWeapon();
    }
}