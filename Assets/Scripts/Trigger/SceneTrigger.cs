using Cinemachine;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        SwitchCamera,
        MovePlayer
    }

    [Header("触发类型")]
    public TriggerType triggerType;

    [Header("【切换相机】专用设置")]
    public CinemachineVirtualCamera targetCamera;
    public bool restoreCameraOnExit = true;

    [Header("【移动玩家】专用设置")]
    public int levelNum;
    public Transform[] targetMovePositions;
    public GameObject[] levels;
    public bool teleportInstantly = true;

    private CinemachineVirtualCamera defaultCamera;
    private bool isTriggered;

    public Transform player;
    public CharacterController playerCc;
    private Transform targetMovePos;
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
      
    }

    void OnTriggerEnter(Collider other)
    {
        // 不仅要检查Tag，还要检查是不是真正的Player（有CharacterController）
        if (isTriggered || !other.CompareTag("Player")) return;

        TimeScoreManager.Instance.currentSceneTrigger = this;

        isTriggered = true;

        switch (triggerType)
        {
            case TriggerType.SwitchCamera:
                SwitchToTargetCamera();
                break;
            case TriggerType.MovePlayer:
                if (targetMovePositions != null)
                    targetMovePos = targetMovePositions[levelNum];
                break;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isTriggered || !other.CompareTag("Player")) return;
        isTriggered = false;

        if (triggerType == TriggerType.SwitchCamera && restoreCameraOnExit)
        {
            RestoreDefaultCamera();
        }
    }

    void SwitchToTargetCamera()
    {
        if (targetCamera == null) return;
        targetCamera.Priority = 100;
   
    }

    void RestoreDefaultCamera()
    {
        if (targetCamera != null)
        {
            targetCamera.Priority = 0;

        }
    }

    // 专门适配CharacterController的移动方法  放到下一关的button里
    public void MovePlayerToTarget()
    {
        foreach (var a in levels)
            a.gameObject.SetActive(false);

        levels[levelNum + 1].gameObject.SetActive(true);

        Debug.Log(levels[levelNum + 1].gameObject.name);
        // 先禁用CharacterController，再改位置，最后启用
        playerCc.enabled = false;

        player.position = targetMovePos.position;
        player.rotation = targetMovePos.rotation;

        playerCc.enabled = true;

        Debug.Log($"玩家已移动到：{targetMovePos.name}");
    }
}