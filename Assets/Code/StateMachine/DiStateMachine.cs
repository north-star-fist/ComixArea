using VContainer;

namespace ComixArea.StateMachine
{
    public class DiStateMachine : StateMachine
    {
        private readonly IObjectResolver _resolver;

        public DiStateMachine(IObjectResolver resolver) => _resolver = resolver;

        public override void RegisterState<T>(T state)
        {
            _resolver.Inject(state);
            base.RegisterState(state);
        }
    }
}
