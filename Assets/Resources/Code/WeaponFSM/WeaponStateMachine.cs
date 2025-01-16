using System;
using System.Collections.Generic;

namespace Resources.Code.WeaponFSM;

public class WeaponStateMachine
{
    private WeaponState StateCurrent { get;  set; }
    
    private readonly Dictionary<Type, WeaponState> _states = new();

    public void AddState(WeaponState state)
    {
        _states?.Add(state.GetType(), state);
    }

    public void TrySetState<T>() where T : IWeaponState
    {
        var type = typeof(T);
        if (_states.TryGetValue(type, out var newState))
        {
            if (!newState.CanEnterState) return;
            //if (StateCurrent != null && !StateCurrent.CanExitState) return;
            StateCurrent?.OnExitState();
            StateCurrent = newState;
            StateCurrent.OnEnterState();
        }
    }
    public void DisableWeaponStateMachine()
    {
        StateCurrent?.OnExitState();
        StateCurrent = null;
    }

    public void RestartState()
    {
        StateCurrent?.OnExitState();
        StateCurrent?.OnEnterState();
    }
}