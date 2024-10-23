using ComixArea.StateMachine;
using VContainer;

namespace ComixArea.Flow
{
    public class AppFlow : DiStateMachine, IAppFlow
    {
        public AppFlow(IObjectResolver resolver) : base(resolver) { }
    }
}
