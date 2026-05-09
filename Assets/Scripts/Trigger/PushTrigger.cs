using UnityEngine;
using StarterAssets;

public class PushTrigger : MonoBehaviour
{
    [Header("弹开设置")]
    [Tooltip("弹开力度，数值越大滑的越远（建议30-50）")]
    public float pushForce = 40f;
    [Tooltip("弹开方向：旋转物体即可调整，红色X轴箭头指向=弹开方向")]
    public Vector3 pushDirection => transform.right;
    [Tooltip("触发冷却时间，防止重复触发")]
    public float cooldown = 0.5f;

    private float _lastTriggerTime;

    private void OnTriggerEnter(Collider other)
    {
        // 只响应玩家，且冷却结束
        if (!other.CompareTag("Player") || Time.time - _lastTriggerTime < cooldown) return;

        // 获取玩家控制器
        ThirdPersonController playerController = other.GetComponent<ThirdPersonController>();
        if (playerController == null) return;

        // 【核心】给玩家一个有过程的推力，不是瞬间位移
        playerController.pushForce = pushDirection.normalized * pushForce;
        _lastTriggerTime = Time.time;

        Debug.Log($"【弹开触发】给玩家施加了{pushForce}的推力，方向：{pushDirection}");
    }
}