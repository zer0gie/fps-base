
namespace Code.WeaponFSM
{
    public abstract class WeaponState : IWeaponState
    {
        protected readonly WeaponStateMachine WeaponStateMachine;
        protected readonly Weapon.Weapon Weapon;

        protected WeaponState(WeaponStateMachine weaponStateMachine, Weapon.Weapon weapon)
        {
            WeaponStateMachine = weaponStateMachine;
            Weapon = weapon;
        }

        public virtual bool CanEnterState => true;
        
        public virtual bool CanExitState => true;

        public virtual void OnEnterState() { }
        
        public virtual void OnExitState() { }
    }
}