
namespace Code.WeaponFSM
{
    public class WeaponStateInactive : WeaponState
    {
        public WeaponStateInactive(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon) : base(weaponStateMachine, weapon)
        {
        }
    
        public override bool CanExitState => Weapon.gameObject.activeSelf;
    }
}