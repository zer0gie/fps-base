namespace Code.WeaponFSM
{
    public abstract class WeaponState : IWeaponState
    {
        protected readonly WeaponFSM WeaponFsm;
        protected readonly Weapon.Weapon Weapon;

        protected WeaponState(WeaponFSM weaponFsm, Weapon.Weapon weapon)
        {
            WeaponFsm = weaponFsm;
            Weapon = weapon;
        }

        public virtual bool CanEnterState => true;
        
        public virtual bool CanExitState => true;

        public virtual void OnEnterState() { }
        
        public virtual void OnExitState() { }
    }
}