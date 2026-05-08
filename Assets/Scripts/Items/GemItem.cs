using UnityEngine;

public class GemItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 检测到玩家触发
        if (other.CompareTag("Player"))
        {
            GemManager.Instance.AddGem(); // 增加宝石
            gameObject.SetActive(false); // 销毁/隐藏宝石
        }
    }
}