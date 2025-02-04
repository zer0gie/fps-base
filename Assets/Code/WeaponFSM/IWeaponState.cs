using Cysharp.Threading.Tasks;

namespace Code.WeaponFSM
{
    public interface IWeaponState
    {
        bool CanEnterState { get; }
        bool CanExitState { get; }
        void OnEnterState();
        void OnExitState();
    }
}