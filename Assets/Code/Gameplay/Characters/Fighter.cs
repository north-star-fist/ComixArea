using ComixArea.Animation;
using ComixArea.Input;
using ComixArea.Navigation;
using ComixArea.Util;
using R3;
using UnityEngine;
using UnityEngine.Events;
using static ComixArea.IHealth;

namespace ComixArea.Creatures
{
    /// <summary>
    /// Fighter class. The main class for player or bot character.
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        private const int GroundDetectionMaxDistance = 9000;
        private const int BlockingLayer = 1;
        private static int s_animPunchTrigger = Animator.StringToHash("punch");
        private static int s_animKickTrigger = Animator.StringToHash("kick");
        private static int s_animJumpTrigger = Animator.StringToHash("jump");
        private static int s_animLocomotionParam = Animator.StringToHash("locomotion");
        private static int s_animLandingTrigger = Animator.StringToHash("land");
        private static int s_animFlipTrigger = Animator.StringToHash("flip");
        private static int s_animIsDeadParam = Animator.StringToHash("isDead");
        private static int s_animGettingHitTrigger = Animator.StringToHash("getHit");
        private static int s_animIsGroundedParam = Animator.StringToHash("isGrounded");

        private readonly Quaternion _leftRotation = Quaternion.Euler(0, -90, 0);
        private readonly Quaternion _rightRotation = Quaternion.Euler(0, 90, 0);


        [SerializeField]
        private RuntimeAnimatorController _animationController;
        [SerializeField]
        private Rigidbody _rigidBody;
        [SerializeField, Tooltip("This child transform is rotated when character direction is being switched")]
        private Transform _modelPivotTransform;
        [SerializeField]
        private float _locomotionSpeed = 5;
        [SerializeField]
        private float _blockModeSpeedFactor = .5f;
        [SerializeField]
        private Vector2 _jumpImpulse = new Vector2(5, 5);
        [SerializeField]
        private LayerMask _groundLayers = -1;
        [SerializeField]
        private float _groundDetectionDistance = 0.5f;
        [SerializeField, Tooltip("Height when a character should switch to landing animation when falling")]
        private float _landingHeight = 0.5f;
        [SerializeField]
        private LayerMask _enemiesLayerMask = -1;
        [SerializeField]
        private HumanBodyBones _rightHandBone = HumanBodyBones.RightHand;
        [SerializeField]
        private HumanBodyBones _leftHandBone = HumanBodyBones.LeftHand;
        [SerializeField]
        private HumanBodyBones _kickLegBone = HumanBodyBones.RightFoot;
        [SerializeField]
        private ParticleSystem _hitParticles;
        [SerializeField]
        private CapsuleCollider _physicalCollider;
        [SerializeField, Layer]
        private int _deadBodyLayer = 0;

        public UnityEvent PunchEvent;
        public UnityEvent KickEvent;
        public UnityEvent LightHitEvent;
        public UnityEvent HeavyHitEvent;
        public UnityEvent JumpEvent;
        public UnityEvent DeathEvent;

        public LookDirection LookDirection => _lookDirection;
        public IHealth Health => _health;
        public string Name { get; set; }
        public bool IsDead => _health != null && _health.CurrentHealth.CurrentValue <= 0;
        public bool IsGrounded => _isGroundedRx.Value;

        private readonly Subject<DamageData> _onDead = new Subject<DamageData>();


        private Animator _animator;

        private Vector2 _dpadState;
        // when a character perform some actions (punches or kicks) they can not move
        private bool _motionAllowed = true;
        private bool _actionsAllowed = true;

        private JumpDirection? _jumpDirection = null;
        private ReactiveProperty<bool> _isGroundedRx = new ReactiveProperty<bool>(false);
        private LookDirection _lookDirection = LookDirection.Right;
        private readonly ReactiveProperty<bool> _isInBlockMode = new ReactiveProperty<bool>(false);
        private float _midAirHeight;

        private CapsuleCollider _leftHandCollider;
        private CapsuleCollider _rightHandCollider;
        private CapsuleCollider _rightLegCollider;

        private readonly Collider[] _enemyColliders = new Collider[2];

        private IHealth _health;


        private void Awake()
        {
            if (TryGetComponent<ICharacterControlProvider>(out var controlProvider))
            {
                ICharacterControl characterControl = controlProvider.GetCharacterControl();
                characterControl.OnDPad.Subscribe(OnDPad).AddTo(this);
                characterControl.OnAction.Subscribe(OnAction).AddTo(this);
                characterControl.OnJump.Subscribe(OnJump).AddTo(this);
                characterControl.OnBlock.Subscribe(OnBlock).AddTo(this);
            }
            _isInBlockMode.Subscribe(on =>
            {
                if (!IsDead && _animator != null)
                {
                    _animator.SetLayerWeight(BlockingLayer, on ? 1f : 0f);
                }
            }).AddTo(this);
            if (TryGetComponent<IHealth>(out var health))
            {
                if (health is BlockingHealth blockingHealth)
                {
                    blockingHealth.SetBlockRx(_isInBlockMode);
                }
                health.OnDamage.Subscribe(OnDamageTaken).AddTo(this);
                _health = health;
            }
            _onDead.Subscribe(HandleDeath).AddTo(this);
            _isGroundedRx.Subscribe(grounded => SetAnimBoolParam(s_animIsGroundedParam, grounded)).AddTo(this);
        }

        private void Update()
        {
            if (CanMove() && _dpadState.x != 0)
            {
                bool faceIsRight = _lookDirection is LookDirection.Right;
                bool movingRight = _dpadState.x > 0;
                if (_isInBlockMode.Value)
                {
                    int dirMultiplier = faceIsRight ^ movingRight ? -1 : 1;
                    if (_animator != null)
                    {
                        _animator.SetFloat(s_animLocomotionParam, dirMultiplier * _dpadState.x);
                    }
                }
                else
                {
                    // we are not in block mode so we should flip character if they look opposite direction
                    if (faceIsRight ^ movingRight)
                    {
                        // should flip
                        if (faceIsRight)
                        {
                            FlipLeft();
                        }
                        else
                        {
                            FlipRight();
                        }
                    }
                    else
                    {
                        // go on
                        if (_animator != null)
                        {
                            _animator.SetFloat(s_animLocomotionParam, Mathf.Abs(_dpadState.x));
                        }
                    }
                }
            }
            else
            {
                if (_animator != null)
                {
                    _animator.SetFloat(s_animLocomotionParam, 0);
                    if (!IsDead && IsLanding())
                    {
                        TriggerAnimator(s_animLandingTrigger);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (IsDead)
            {
                return;
            }
            if (_jumpDirection != null)
            {
                _rigidBody.velocity = Vector3.zero;
                addJumpForce();
                _jumpDirection = null;
            }
            else if (CanMove())
            {
                addMotionForce();
            }
            _midAirHeight = GetHeight();
            _isGroundedRx.Value = _midAirHeight < 0.01f;

            void addJumpForce()
            {
                Vector2 jumpForce = _jumpDirection switch
                {
                    JumpDirection.Right => _jumpImpulse,
                    JumpDirection.Left => Vector3.Scale(_jumpImpulse, new(-1f, 1f, 1f)),
                    _ => new Vector3(0, _jumpImpulse.y)
                };
                _rigidBody.AddForce(jumpForce, ForceMode.VelocityChange);
            }

            void addMotionForce()
            {
                float horSpeed = 0f;
                if (_lookDirection is LookDirection.Left or LookDirection.Right && _dpadState.x != 0)
                {
                    bool movingRight = _dpadState.x > 0;
                    int speedDirMultiplier = movingRight ? 1 : -1;
                    horSpeed = _locomotionSpeed * speedDirMultiplier;
                    if (_isInBlockMode.Value)
                    {
                        horSpeed *= _blockModeSpeedFactor;
                    }
                }
                _rigidBody.velocity = _rigidBody.velocity.WithX(horSpeed);
            }
        }

        /// <summary>
        /// Sets custom model for the player.
        /// </summary>
        /// <param name="model"> model game object to set </param>
        /// <param name="initialLookDir"> initial look direction </param>
        public void SetModel(GameObject model, HorizontalDirection initialLookDir)
        {
            _modelPivotTransform.DestroyAllChildObjects();
            _modelPivotTransform.localRotation = initialLookDir is HorizontalDirection.Left ? _leftRotation : _rightRotation;
            model.transform.parent = _modelPivotTransform;
            model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _lookDirection = initialLookDir is HorizontalDirection.Left ? LookDirection.Left : LookDirection.Right;
            _animator = TuneAnimator(model.GetComponentInChildren<Animator>());
            model.SetLayerRecursively(gameObject.layer);

            _leftHandCollider = getHittingCollider(_leftHandBone);
            _rightHandCollider = getHittingCollider(_rightHandBone);
            _rightLegCollider = getHittingCollider(_kickLegBone);

            CapsuleCollider getHittingCollider(HumanBodyBones bone)
            {
                var boneTrans = _animator.GetBoneTransform(bone);
                CapsuleCollider col = boneTrans != null ? boneTrans.GetComponentInChildren<CapsuleCollider>() : null;
                if (col != null)
                {
                    col.isTrigger = true;
                    // We don't need it enabled. It creates unwanted events
                    col.enabled = false;
                }
                return col;
            }
        }

        public void AllowMotion()
        {
            _motionAllowed = !IsDead;
        }

        public void ProhibitMotion()
        {
            _motionAllowed = false;
        }

        public void AllowActions()
        {
            _actionsAllowed = !IsDead;
        }

        public void ProhibitActions()
        {
            _actionsAllowed = false;
        }

        public void FinishTurning()
        {
            _lookDirection = _lookDirection switch
            {
                LookDirection.SwitchingToLeft => LookDirection.Left,
                LookDirection.SwitchingToRight => LookDirection.Right,
                _ => _lookDirection
            };
            _modelPivotTransform.rotation = _lookDirection == LookDirection.Left ? _leftRotation : _rightRotation;
        }

        private void OnDPad(Vector2 dpad)
        {
            _dpadState = dpad;
        }

        private void OnAction(Unit _)
        {
            if (!CanAct())
            {
                return;
            }
            ProhibitMotion();
            ProhibitActions();
            if (_dpadState.y > 0)
            {
                TriggerAnimator(s_animKickTrigger);
                KickEvent?.Invoke();
            }
            else
            {
                TriggerAnimator(s_animPunchTrigger);
                PunchEvent?.Invoke();
            }
        }

        private void OnJump(Unit _)
        {
            if (!CanJump())
            {
                _jumpDirection = null;
                return;
            }
            TriggerAnimator(s_animJumpTrigger);
            JumpEvent?.Invoke();
            _jumpDirection = _dpadState.x switch
            {
                < 0 => JumpDirection.Left,
                > 0 => JumpDirection.Right,
                _ => JumpDirection.Up
            };
        }

        public void OnBlock(bool on) => _isInBlockMode.Value = on;


        private Animator TuneAnimator(Animator animator)
        {
            if (animator == null)
            {
                return null;
            }
            animator.runtimeAnimatorController = _animationController;
            animator.applyRootMotion = false;
            FighterAnimationEventsHandler handler = animator.gameObject.GetOrAddComponent<FighterAnimationEventsHandler>();
            handler.Init(
                HandleEntegingIdle,
                AllowMotion,
                ProhibitMotion,
                AllowActions,
                ProhibitActions,
                FinishTurning,
                HandleExitingGettingHit,
                AttackRightHand,
                AttackLeftHand,
                AttackRightLeg,
                AttackLeftLeg
            );
            return animator;
        }

        private void HandleEntegingIdle()
        {
            AllowMotion();
            AllowActions();
            FinishTurning();
        }


        private void HandleExitingGettingHit()
        {
            AllowMotion();
            AllowActions();
            FinishTurning();
        }

        private void FlipRight()
        {
            _lookDirection = LookDirection.SwitchingToRight;
            TriggerAnimator(s_animFlipTrigger);
        }

        private void FlipLeft()
        {
            _lookDirection = LookDirection.SwitchingToLeft;
            TriggerAnimator(s_animFlipTrigger);
        }

        private float GetHeight()
        {
            Vector3 thisPosition = transform.position;
            bool hit = Physics.SphereCast(
                thisPosition + Vector3.up * (_groundDetectionDistance + 0.001f),
                _physicalCollider.radius,
                Vector3.down,
                out var hitInfo,
                GroundDetectionMaxDistance,
                _groundLayers,
                QueryTriggerInteraction.Ignore
            );
            return hit ? (thisPosition - hitInfo.point).y : 9000;
        }

        private bool CanMove() => !IsDead && _motionAllowed && IsGrounded && !IsTurning();

        private bool CanAct() => !IsDead && _actionsAllowed && !IsTurning();

        private bool CanJump() => CanAct() && IsGrounded && !IsTurning();

        private bool IsTurning()
        {
            return _lookDirection is LookDirection.SwitchingToLeft or LookDirection.SwitchingToRight;
        }

        private bool IsLanding()
        {
            // We are falling but not too high above ground
            return _rigidBody.velocity.y < 0 && !IsGrounded && _midAirHeight < _landingHeight;
        }

        public void AttackLeftHand() => Damage(_leftHandCollider, 10f);

        public void AttackRightHand() => Damage(_rightHandCollider, 10f);

        public void AttackLeftLeg()
        {
        }

        public void AttackRightLeg() => Damage(_rightLegCollider, 10f);

        void Damage(CapsuleCollider limb, float damage)
        {
            if (limb == null)
            {
                return;
            }
            var limbTrans = limb.transform;

            Vector3 capsuleOrientation = limb.direction == 0 ? Vector3.right : (limb.direction == 1 ? Vector3.up : Vector3.forward);
            var capsuleCenter = limb.center;
            Vector3 p1 = capsuleCenter + capsuleOrientation * (limb.height / 2 - limb.radius);
            Vector3 p2 = capsuleCenter - capsuleOrientation * (limb.height / 2 - limb.radius);
            p1 = limbTrans.TransformPoint(p1);
            p2 = limbTrans.TransformPoint(p2);
            Debug.DrawLine(p1, p2, Color.red, 10);
            int hits = Physics.OverlapCapsuleNonAlloc(
                p1,
                p2,
                limb.radius,
                _enemyColliders,
                _enemiesLayerMask,
                QueryTriggerInteraction.Ignore
            );
            for (int i = 0; i < hits; i++)
            {
                var enemyCol = _enemyColliders[i];
                Health health = enemyCol.GetComponentInParent<Health>();
                if (health != null)
                {
                    Vector3 forceDir = Vector3.right * (_lookDirection is LookDirection.Right ? 1 : -1);
                    health.Damage(damage, p1, forceDir, -forceDir, enemyCol.attachedRigidbody);
                }
            }
        }

        private void OnDamageTaken(DamageData damageData)
        {
            ProhibitActions();
            ProhibitMotion();
            if (_hitParticles != null)
            {
                // Set the position of the particle system to where the hit was sustained.
                _hitParticles.transform.position = damageData.HitPosition;
                _hitParticles.transform.rotation = Quaternion.FromToRotation(Vector3.forward, damageData.HitNormal);

                // And play the particles.
                _hitParticles.Play();
                if (damageData.Damage > 5)
                {
                    HeavyHitEvent?.Invoke();
                }
                else
                {
                    LightHitEvent?.Invoke();
                }
            }
            if (damageData.IsFatal)
            {
                _onDead.OnNext(damageData);
            }
            else
            {
                TriggerAnimator(s_animGettingHitTrigger);
            }
        }

        private void HandleDeath(DamageData dd)
        {
            DeathEvent?.Invoke();
            SetAnimBoolParam(s_animIsDeadParam, true);
            if (_rigidBody && Vector3.zero != dd.ImpactForce)
            {
                _rigidBody.AddForceAtPosition(dd.ImpactForce, dd.HitPosition);
            }
            _physicalCollider.gameObject.layer = _deadBodyLayer;
        }

        private void TriggerAnimator(int triggerHash)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerHash);
            }
        }

        private void SetAnimBoolParam(int boolParamHash, bool val)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolParamHash, val);
            }
        }
    }
}
