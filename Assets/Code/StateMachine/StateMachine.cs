using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ComixArea.StateMachine
{
    /// <summary>
    /// Default implementation of <see cref="IStateMachine"/> interface.
    /// </summary>
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _states = new();
        private IState _currentState = null;

        public bool InTransition => _inTranzition;
        private bool _inTranzition = false;


        public virtual void RegisterState<T>(T state) where T : class, IState
        {
            Type type = typeof(T);
            if (_states.ContainsKey(type))
            {
                throw new InvalidOperationException($"Already contains handler for state {type}");
            }
            _states[type] = state;
        }

        public virtual async UniTask GoToStateAsync<T>(Action<T> stateSetup = null) where T : class, IState
        {
            Type type = typeof(T);
            if (_inTranzition)
            {
                throw new InvalidOperationException($"State machine is in transition. Can't go to state {type}");
            }
            await GoToStateUnsafeAsync(stateSetup);
        }

        public async UniTask<bool> TryGoToStateAsync<T>(Action<T> stateSetup = null) where T : class, IState
        {
            if (_inTranzition)
            {
                return false;
            }
            await GoToStateUnsafeAsync(stateSetup);
            return true;
        }

        private async UniTask GoToStateUnsafeAsync<T>(Action<T> stateSetup = null) where T : class, IState
        {
            _inTranzition = true;

            Type type = typeof(T);
            if (!_states.TryGetValue(type, out var nextState))
            {
                throw new InvalidOperationException($"State {type} was not registered!");
            }

            stateSetup?.Invoke(nextState as T);

            if (_currentState != null)
            {
                await _currentState.OnExitAsync();
            }
            await nextState.OnEnterAsync();
            _currentState = nextState;
            _inTranzition = false;

            await nextState.StartAsync();
        }
    }
}
