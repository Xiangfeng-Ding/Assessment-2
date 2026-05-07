using UnityEngine;
using StarterAssets;

public class RotatingPlatform : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转速度（度/秒）：正数=顺时针，负数=逆时针")]
    public float rotateSpeed = 60f;
    [Tooltip("旋转轴，罗盘上下转固定用 Vector3.up")]
    public Vector3 rotateAxis = Vector3.up;

    // 记录上一帧的旋转，计算位移差
    private Quaternion _lastFrameRotation;
    // 记录站在平台上的玩家
    private CharacterController _playerOnPlatform;

    void Start()
    {
        _lastFrameRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        // 1. 旋转平台
        transform.Rotate(rotateAxis, rotateSpeed * Time.fixedDeltaTime);

        // 2. 如果有玩家站在上面，带动玩家一起旋转
        if (_playerOnPlatform != null)
        {
            // 计算这一帧和上一帧的旋转差
            Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(_lastFrameRotation);

            // 计算玩家所在位置，因为旋转产生的位移
            Vector3 playerPosition = _playerOnPlatform.transform.position;
            Vector3 pivot = transform.position;
            Vector3 offset = playerPosition - pivot;
            Vector3 rotatedOffset = rotationDelta * offset;
            Vector3 moveDelta = rotatedOffset - offset;

            // 给玩家叠加位移，让玩家跟着平台转
            _playerOnPlatform.Move(moveDelta);
        }

        // 更新上一帧的旋转
        _lastFrameRotation = transform.rotation;
    }

    // 玩家进入Trigger=站在平台上
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerOnPlatform = other.GetComponent<CharacterController>();
        }
    }

    // 玩家离开Trigger=离开平台
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerOnPlatform = null;
        }
    }
}