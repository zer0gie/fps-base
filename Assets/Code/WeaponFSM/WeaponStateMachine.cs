using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.WeaponFSM
{
    public class WeaponStateMachine
    {
        private WeaponState CurrentWeaponState { get; set; }
    
        private readonly Dictionary<Type, WeaponState> _states = new();

        public void AddState(WeaponState state)
        {
            _states?.Add(state.GetType(), state);
        }

        public void TrySetState<T>() where T : IWeaponState
        {
            Debug.Log($"WEAPON_FSM: TRYING SET NEW WEAPON STATE - {typeof(T)}");
            if (CurrentWeaponState == null)
            {
                Debug.Log("WEAPON_FSM: CurrentWeaponState is null!");
                return;
            }
            if (!CurrentWeaponState.CanExitState)
            {
                Debug.Log($"WEAPON_FSM: {CurrentWeaponState} can't exit now");
                return;
            }
            var type = typeof(T);
            if (type == CurrentWeaponState.GetType())
            {
                Debug.Log($"WEAPON_FSM: {CurrentWeaponState} already is current.");
                return;
            }
            if (_states.TryGetValue(type, out var newWeaponState))
            {
                if (!newWeaponState.CanEnterState)
                {
                    Debug.Log($"WEAPON_FSM: {newWeaponState} can't enter now");
                    return;
                } 
                CurrentWeaponState?.OnExitState();
                var previousWeaponState = CurrentWeaponState;
                CurrentWeaponState = newWeaponState;
                
                
                CurrentWeaponState.OnEnterState();
                Debug.Log($"WEAPON_FSM: Success switching {previousWeaponState} => {newWeaponState} !");
            }
        }
        public void ForceSetState<T>() where T : IWeaponState
        {
            var type = typeof(T);
            
            CurrentWeaponState?.OnExitState();
            
            if (_states.TryGetValue(type, out var newWeaponState))
            {
                CurrentWeaponState = newWeaponState;
                
                Debug.Log($"WEAPON_FSM: FORCED switch to {CurrentWeaponState}!");
            }
        }
    }
}