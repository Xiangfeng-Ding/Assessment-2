using UnityEngine;
using StarterAssets;

public class BounceMushroom : MonoBehaviour
{
    [Header("弹跳设置")]
    [Tooltip("弹跳力度（直接给速度，数值要大，建议20-30）")]
    public float bounceForce ; // 注意：变量名改成Force更直观，数值调大
    [Tooltip("玩家的标签，必须和玩家物体的Tag一致")]
    public string playerTag = "Player";

    private Animator _mushroomAnim;

    void Awake()
    {
        _mushroomAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider hit)
    {
   
        if (hit == null) return;



        //只在玩家向下移动时触发（避免从下往上顶也弹）
        ThirdPersonController playerController = hit.GetComponent<ThirdPersonController>();
        if (playerController == null) return;


        // 执行弹跳
        Debug.Log($"【蘑菇触发】给玩家施加弹跳力：{bounceForce}");
        playerController.Bounce(bounceForce);

    }
}