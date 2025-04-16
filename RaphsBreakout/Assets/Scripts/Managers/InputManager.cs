using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Utilities;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
    {
        public static event Action<InputAction.CallbackContext> Input_OnMove;

        private InputSystem_Actions.PlayerActions _playerActions;
        protected override void Init()
        {
            base.Init();
            _playerActions = new InputSystem_Actions.PlayerActions();
            _playerActions.Enable();
            _playerActions.AddCallbacks(this);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Input_OnMove?.Invoke(context);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnFastForward(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}