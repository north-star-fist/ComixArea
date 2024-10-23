using System;
using Cysharp.Threading.Tasks;

namespace ComixArea.StateMachine
{
    public interface IStateMachine
    {
        public bool InTransition { get; }

        /// <summary>
        /// Registers a state instance corresponding it's type. Must be invoked before a state machine switching to it.
        /// </summary>
        /// <typeparam name="T"> type of state </typeparam>
        /// <param name="state"> state instance </param>
        public void RegisterState<T>(T state) where T : class, IState;

        /// <summary>
        /// Switches a state machine to the specified state. Throws an <see cref="InvalidOperationException"/>
        /// if the state machine is already in transition to some state.
        /// </summary>
        /// <typeparam name="T"> state type </typeparam>
        /// <param name="stateSetup"> optional state setup method </param>
        public UniTask GoToStateAsync<T>(Action<T> stateSetup = null) where T : class, IState;

        /// <summary>
        /// Tries to switch a state machine to the specified state. Returns false if the switching failed.
        /// </summary>
        /// <typeparam name="T"> state type </typeparam>
        /// <param name="stateSetup"> optional state setup method </param>
        public UniTask<bool> TryGoToStateAsync<T>(Action<T> stateSetup = null) where T : class, IState;
    }
}
