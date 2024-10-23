using Cysharp.Threading.Tasks;

namespace ComixArea.StateMachine
{
    public interface IState
    {
        /// <summary>
        /// The method that is executed on state machine entering the state.
        /// </summary>
        public UniTask OnEnterAsync();

        /// <summary>
        /// The method that is executed on state machine exiting the state.
        /// </summary>
        public UniTask OnExitAsync();

        /// <summary>
        /// The method is invoked after <see cref="OnEnterAsync()"/> method finished.
        /// </summary>
        public UniTask StartAsync();
    }
}
