using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ComixArea.AI
{
    abstract public class AAiStateController : MonoBehaviour
    {
        [Header("AI State Controller properties")]

        [SerializeField]
        private AAiStateFactory[] _states;
        [SerializeField]
        private AAiStateFactory _initState;
        private Type _initStateType;

        [Tooltip("Drag particle systems that should be played when the controller is dead from scene view.")]
        public ParticleSystem[] DeathParticles;

        [SerializeField]
        float _gizmoSphereRadius = 0.5f;

        private readonly Dictionary<Type, IAiState> _stateMap = new();

        private CancellationTokenSource _cancellationTokenSource;

        Color _gizmoColor = Color.yellow;

        virtual protected void Awake()
        {
        }

        virtual protected void Start()
        {
            registerState(_initState);
            _initStateType = _initState.StateType;
            foreach (var factory in _states)
            {
                registerState(factory);
            }
            Reset();

            void registerState(AAiStateFactory factory)
            {
                var state = factory.CreateState();
                if (!_stateMap.ContainsKey(state.GetType()))
                {
                    _stateMap.Add(state.GetType(), state);
                    state.Init(this);
                }
            }
        }

        virtual public void Reset()
        {
            Start(_initStateType).Forget();
        }

        private async UniTask Start(Type initStateType)
        {
            CancelCurrentProcess();
            _cancellationTokenSource = new CancellationTokenSource();

            Type curState = initStateType;
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _gizmoColor = _stateMap[curState].GizmoColor;
                curState = await _stateMap[curState].Enter(_cancellationTokenSource.Token);
                if (curState == null)
                {
                    break;
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Vector3 thisPos = transform.position;
            Gizmos.DrawWireSphere(thisPos, _gizmoSphereRadius);
        }

        private void OnDestroy()
        {
            CancelCurrentProcess();
        }

        public bool IsStateRegistered(Type type) => _stateMap.ContainsKey(type);

        private void CancelCurrentProcess()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
