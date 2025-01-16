using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : IInitializable, IDisposable
{

    private SignalBus _signalBus;
    private PlayerInputActions _inputActions;

    public InputManager (SignalBus signalBus)
    {
        _signalBus = signalBus;

        _inputActions = new PlayerInputActions();
    }
    public void Initialize()
    {
        _inputActions.Enable();

        _inputActions.Player.Fire.started += Fire_started;

        _inputActions.Player.Fire.canceled += Fire_canceled;

        _inputActions.Player.Reload.performed += Reload_performed;

        _inputActions.Player.FirstSlotActivated.performed += FirstSlotActivated_performed;

        _inputActions.Player.SecondSlotActivated.performed += SecondSlotActivated_performed;

        _inputActions.Player.ThirdSlotActivated.performed += ThirdSlotActivated_performed;
    }
    public void EnableFPSControl()
    {
        _inputActions.Player.Enable();
    }
    private void ThirdSlotActivated_performed(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<ThirdSlotActivatedSignal>();
    }

    private void SecondSlotActivated_performed(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<SecondSlotActivatedSignal>();
    }
    private void FirstSlotActivated_performed(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<FirstSlotActivatedSignal>();
    }

    private void Reload_performed(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<ReloadPerformedSignal>();
    }

    private void Fire_canceled(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<FireActionCancelledSignal>();
    }

    private void Fire_started(InputAction.CallbackContext obj)
    {
        _signalBus.Fire<FireActionStartedSignal>();
    }
    public Vector2 GetMovementVectorNormalized()
    {
        var inputVector = _inputActions.Player.Movement.ReadValue<Vector2>();
        return inputVector.normalized;
    }
    public Vector2 GetLookDelta()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }
    public void Dispose()
    {
        _inputActions.Player.Fire.started -= Fire_started;
        _inputActions.Player.Fire.canceled -= Fire_canceled;
        _inputActions.Player.Reload.performed -= Reload_performed;

        _inputActions.Disable();
    }
}