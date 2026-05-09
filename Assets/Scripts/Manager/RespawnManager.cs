using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    //[Header("重生点列表（按序号排序）")]
    //public Transform[] respawnPoints; // 按序号拖入，0=1号重生点，1=2号重生点，以此类推

    private void Awake()
    {
        Instance = this;
    }


    public void TeleportToRespawnPoint(Transform targetPos)
    {
   
        // 传送玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 关闭CharacterController避免传送冲突
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position=targetPos.position;

            //player.transform.position = respawnPoints[pointIndex].position;
            //player.transform.rotation = respawnPoints[pointIndex].rotation;

            if (controller != null) controller.enabled = true;
        }
    }
}