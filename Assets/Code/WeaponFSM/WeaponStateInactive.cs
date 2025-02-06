
namespace Code.WeaponFSM
{
    public class WeaponStateInactive : WeaponState
    {
        public WeaponStateInactive(WeaponFSM weaponFsm, Weapon.Weapon weapon) : base(weaponFsm, weapon)
        {
        }
    
        public override bool CanExitState => Weapon;
    }
}