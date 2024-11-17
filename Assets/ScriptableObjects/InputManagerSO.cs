using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[CreateAssetMenu(menuName = "InputManager")]
public class InputManagerSO : ScriptableObject
{
    Controls myControls;
    public event Action OnJump;
    public event Action OnAttack;
    public event Action OnGreen;
    public event Action OnRed;
    public event Action OnWhite;
    public event Action OnCrouch;
    public event Action OnCrouchCanceled;
    public event Action OnRun;
    public event Action OnRunCanceled;
    public event Action<Vector2> OnMove;

    private void OnEnable()
    {
        myControls = new Controls();
        myControls.Gameplay.Enable();
        myControls.Gameplay.Jump.started += Jump;
        myControls.Gameplay.Move.performed += Move;
        myControls.Gameplay.Move.canceled += Move;
        myControls.Gameplay.Attack.started += Attack;
        myControls.Gameplay.Green.started += Green;
        myControls.Gameplay.Red.started += Red;
        myControls.Gameplay.White.started += White;
        myControls.Gameplay.Crouch.performed += Crouch;
        myControls.Gameplay.Crouch.canceled += CrouchCanceled;
        myControls.Gameplay.Run.performed += Run;
        myControls.Gameplay.Run.canceled += RunCanceled;
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke();
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        OnJump?.Invoke();
    }

    private void Green(InputAction.CallbackContext ctx)
    {
        OnGreen?.Invoke();
    }
    private void White(InputAction.CallbackContext ctx)
    {
        OnWhite?.Invoke();
    }
    private void Red(InputAction.CallbackContext ctx)
    {
        OnRed?.Invoke();
    }

    private void Crouch(InputAction.CallbackContext ctx)
    {
        OnCrouch?.Invoke();
    }
    private void Run(InputAction.CallbackContext ctx)
    {
        OnRun?.Invoke();
    }

    private void RunCanceled(InputAction.CallbackContext ctx)
    {
        OnRunCanceled?.Invoke();
    }

    private void CrouchCanceled(InputAction.CallbackContext ctx)
    {
        OnCrouchCanceled?.Invoke();
    }

}
