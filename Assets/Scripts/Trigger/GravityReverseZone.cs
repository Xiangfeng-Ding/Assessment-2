using UnityEngine;
using StarterAssets; // 引用原控制器的命名空间

public class GravityReverseZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 仅检测玩家并触发反转
        if (other.CompareTag("Player") && other.TryGetComponent(out ThirdPersonController controller))
        {
            controller.ToggleGravity();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 仅检测玩家并触发反转
        if (other.CompareTag("Player") && other.TryGetComponent(out ThirdPersonController controller))
        {
            controller.ToggleGravity();
        }
    }

}