using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("重力反转")]
        [Tooltip("当前是否处于重力反转状态")]
        public bool isGravityReversed = false;

        // 核心拆分：角色旋转立刻生效，摄像机旋转延迟+平滑生效
        private Quaternion _characterGravityRotation = Quaternion.identity; // 角色重力翻转旋转（立刻生效）
        private Quaternion _cameraGravityRotation = Quaternion.identity;    // 摄像机重力翻转旋转（延迟平滑生效）
        private Quaternion _yawRotation = Quaternion.identity;               // 左右转向的旋转（仅Y轴）

        // 摄像机翻转控制
        private Coroutine _delayCameraFlipCoroutine;
        [Tooltip("摄像机翻转延迟时间（秒），1f=1秒")]
        public float cameraFlipDelay = 1f;
        [Tooltip("摄像机平滑翻转的时长（秒），数值越大翻转越慢越顺滑")]
        public float cameraFlipSmoothTime = 0.5f;
        [Tooltip("是否正在平滑翻转中（防重复触发）")]
        private bool _isCameraFlipping = false;
        // 摄像机实际是否完成翻转（用于视角限制适配）
        private bool _isCameraFlipped = false;

        [Tooltip("角色模型子物体（带蒙皮网格的那个）")]
        public Transform playerModel;
        private bool hasRotatedOnce = false;
        private float _targetXAngle = 0f;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        public bool ifPlayerCanMove;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        [SerializeField] private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;
        private Quaternion _lastFrameRotation;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        [HideInInspector] public Vector3 pushForce;
        public float pushDamping = 8f;

        private void Awake()
        {

        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            ifPlayerCanMove = true;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // 初始化重力旋转变量
            _yawRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            _characterGravityRotation = Quaternion.identity;
            _cameraGravityRotation = Quaternion.identity;
            _isCameraFlipped = false;
        }

        private void Update()
        {
            if (!ifPlayerCanMove)
            {
                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, 0f);
                    _animator.SetFloat(_animIDMotionSpeed, 0f);
                }
                _animationBlend = 0f;
                return;
            }

            _hasAnimator = TryGetComponent(out _animator);
            pushForce = Vector3.Lerp(pushForce, Vector3.zero, pushDamping * Time.deltaTime);

            GroundedCheck();
            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            float finalOffset = isGravityReversed ? Mathf.Abs(GroundedOffset) : GroundedOffset;
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + finalOffset, transform.position.z);

            float checkRadius = isGravityReversed ? GroundedRadius * 1.2f : GroundedRadius;
            Grounded = Physics.CheckSphere(spherePosition, checkRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        // 【核心修改】按需求调整：翻转后鼠标操作反向
        private void CameraRotation()
        {
            // 翻转过程中锁定视角输入，避免画面混乱
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition && !_isCameraFlipping)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // 左右转向永远不变，不受翻转影响
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;

                // 【修改核心】移除鼠标Y轴反转逻辑，翻转后自然实现操作反向
                // 未翻转：鼠标上推→视角抬头，鼠标下拉→视角低头（正常）
                // 翻转后：鼠标上推→视角低头，鼠标下拉→视角抬头（反向，符合需求）
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // 视角上下限制和摄像机实际翻转状态绑定，避免翻转后视角卡死
            float currentTopClamp = _isCameraFlipped ? -BottomClamp : TopClamp;
            float currentBottomClamp = _isCameraFlipped ? -TopClamp : BottomClamp;

            // 角度限制
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, currentBottomClamp, currentTopClamp);

            // 摄像机最终旋转
            Quaternion cameraRot = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            CinemachineCameraTarget.transform.rotation = _cameraGravityRotation * cameraRot;
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 cameraRight = _mainCamera.transform.right;
            Vector3 cameraForward = _mainCamera.transform.forward;

            cameraRight.y = 0f;
            cameraForward.y = 0f;
            cameraRight.Normalize();
            cameraForward.Normalize();

            Vector3 targetMoveDir = (cameraRight * _input.move.x + cameraForward * _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                Vector3 adjustedMoveDir = _characterGravityRotation * targetMoveDir;
                float targetYaw = Mathf.Atan2(adjustedMoveDir.x, adjustedMoveDir.z) * Mathf.Rad2Deg;

                float currentYaw = _yawRotation.eulerAngles.y;
                float smoothedYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref _rotationVelocity,
                    RotationSmoothTime);
                _yawRotation = Quaternion.Euler(0f, smoothedYaw, 0f);
            }

            transform.rotation = _characterGravityRotation * _yawRotation;

            _controller.Move(targetMoveDir.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime +
                             pushForce * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            float currentGravity = isGravityReversed ? -Gravity : Gravity;

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if ((!isGravityReversed && _verticalVelocity < 0.0f) || (isGravityReversed && _verticalVelocity > 0.0f))
                {
                    _verticalVelocity = isGravityReversed ? 2f : -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity) * (isGravityReversed ? -1 : 1);
                    Grounded = false;
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (!isGravityReversed)
                {
                    if (_fallTimeoutDelta >= 0.0f)
                    {
                        _fallTimeoutDelta -= Time.deltaTime;
                    }
                    else
                    {
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDFreeFall, true);
                        }
                    }
                }
                else
                {
                    if (_verticalVelocity > 1f)
                    {
                        if (_fallTimeoutDelta >= 0.0f)
                        {
                            _fallTimeoutDelta -= Time.deltaTime;
                        }
                        else
                        {
                            if (_hasAnimator)
                            {
                                _animator.SetBool(_animIDFreeFall, true);
                            }
                        }
                    }
                    else
                    {
                        _fallTimeoutDelta = FallTimeout;
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDFreeFall, false);
                        }
                    }
                }

                _input.jump = false;
            }

            float terminalVel = isGravityReversed ? -_terminalVelocity : _terminalVelocity;
            if ((!isGravityReversed && _verticalVelocity < terminalVel) || (isGravityReversed && _verticalVelocity > terminalVel))
            {
                _verticalVelocity += currentGravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmos()
        {
            DrawGroundCheckGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGroundCheckGizmos();
        }

        private void DrawGroundCheckGizmos()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            float originalOffset = -GroundedOffset;
            Vector3 originalSpherePos = new Vector3(transform.position.x, transform.position.y + originalOffset, transform.position.z);
            Gizmos.DrawSphere(originalSpherePos, GroundedRadius);

            if (isGravityReversed)
            {
                Gizmos.color = new Color(0, 0.5f, 1f, 0.4f);
                float reversedOffset = Mathf.Abs(GroundedOffset);
                Vector3 reversedSpherePos = new Vector3(transform.position.x, transform.position.y + reversedOffset, transform.position.z);
                Gizmos.DrawSphere(reversedSpherePos, GroundedRadius);
            }
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        //外部调用切换重力
        public void ToggleGravity()
        {
            isGravityReversed = !isGravityReversed;
            _verticalVelocity = isGravityReversed ? 2f : -2f;

            // 角色重力旋转立刻生效
            _characterGravityRotation = isGravityReversed ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.identity;
            _yawRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            // 防重复触发：停止所有正在运行的翻转协程
            if (_delayCameraFlipCoroutine != null)
            {
                StopCoroutine(_delayCameraFlipCoroutine);
                _delayCameraFlipCoroutine = null;
            }
            _isCameraFlipping = false;

            // 启动延迟+平滑翻转协程
            _delayCameraFlipCoroutine = StartCoroutine(DelayAndSmoothFlipCameraCoroutine(cameraFlipDelay));

            // 重置动画状态
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, true);
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }
        }

        // 延迟+平滑翻转，同步更新摄像机翻转标记
        private IEnumerator DelayAndSmoothFlipCameraCoroutine(float delaySeconds)
        {
            // 第一步：等待指定的延迟时间
            yield return new WaitForSeconds(delaySeconds);

            // 第二步：准备平滑翻转
            _isCameraFlipping = true;
            _cinemachineTargetPitch = 0f; // 翻转前重置视角，避免翻转后视角混乱

            Quaternion startRot = _cameraGravityRotation;
            Quaternion targetRot = isGravityReversed ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.identity;
            // 目标翻转状态和最终视角同步
            bool targetFlipState = isGravityReversed;
            float elapsedTime = 0f;

            // 第三步：平滑插值旋转
            while (elapsedTime < cameraFlipSmoothTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / cameraFlipSmoothTime);
                t = Mathf.SmoothStep(0f, 1f, t); // 缓动曲线，更柔和
                _cameraGravityRotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            // 第四步：翻转完成，强制设置最终值，同步更新翻转标记
            _cameraGravityRotation = targetRot;
            _isCameraFlipped = targetFlipState; // 只有翻转完成后，才切换视角限制逻辑
            _isCameraFlipping = false;
            _delayCameraFlipCoroutine = null;
        }

        public void Bounce(float bounceForce)
        {
            _verticalVelocity = bounceForce;
        }

        // 物体禁用时停止所有协程，避免报错
        private void OnDisable()
        {
            if (_delayCameraFlipCoroutine != null)
            {
                StopCoroutine(_delayCameraFlipCoroutine);
                _delayCameraFlipCoroutine = null;
            }
            _isCameraFlipping = false;
        }
    }
}