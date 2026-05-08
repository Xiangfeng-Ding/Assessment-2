using UnityEngine;

public class Npc_Dialogue : MonoBehaviour
{
    public Npc npc;
    private UIManager UI;

    [Header("该NPC的对话列表（按顺序放就行，会自动筛选）")]
    public DialogueData[] allNpcDialogues; 


    private bool isTurningToPlayer;

    // 所有挂载本脚本的NPC共享，记录当前有多少个NPC在对话范围内
    private static int s_NpcInRangeCount = 0;
    // 记录当前NPC上一帧的范围状态，避免重复修改计数器、重复操作UI
    private bool m_WasInRangeLastFrame = false;



    void Start()
    {
        UI = UIManager.instance;

    }

    void Update()
    {

        // 主角在范围内 + 按F键触发对话
        // 先把当前范围状态存下来，避免重复调用方法
        bool isInRangeNow = npc.IfPlayerInTalkRange();

        //主角在范围内的逻辑
        if (isInRangeNow)
        {
            // 仅在刚进入范围的瞬间更新状态，不会每帧重复执行
            if (!m_WasInRangeLastFrame)
            {
                s_NpcInRangeCount++;
                // 只要有NPC进入范围，就显示对话提示
                UI.gameUIScript.SetTalkTripText(true);
            }

            //按F触发对话的逻辑，完全不动
            if (Input.GetKeyDown(KeyCode.F))
            {

                UI.SwitchTo(UI.dialoguePanel);
                npc.ChangeToIdleState();
                isTurningToPlayer = true;
                InitDialogue();
            }
        }
        // 主角不在范围内的逻辑 
        else
        {
            // 仅在刚离开范围的瞬间更新状态，不会每帧重复执行
            if (m_WasInRangeLastFrame)
            {
                s_NpcInRangeCount--;
                //只有所有NPC都离开范围时，才关闭提示
                if (s_NpcInRangeCount <= 0)
                {
                    UI.gameUIScript.SetTalkTripText(false);
                    s_NpcInRangeCount = 0;
                }
            }
        }

        // 最后更新上一帧状态，给下一帧用
        m_WasInRangeLastFrame = isInRangeNow;

        if (isTurningToPlayer)//平滑转向Player
        {
            Vector3 directionToPlayer = npc.player.transform.position - npc.transform.position;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                // 平滑旋转到目标方向
                npc.transform.rotation = Quaternion.Slerp(
                    npc.transform.rotation,
                    targetRotation,
                    npc.turnSpeed * Time.deltaTime
                );

                // 检查是否已经转得差不多了（角度差小于 1 度）
                if (Quaternion.Angle(npc.transform.rotation, targetRotation) < 1f)
                {
                    // 直接对齐到目标，停止旋转
                    npc.transform.rotation = targetRotation;
                    isTurningToPlayer = false;
                }
            }
        }
    }


    // 触发该NPC的对话
    public void InitDialogue()
    {
  
        DialogueData targetDialogue = null;
        foreach (DialogueData dialogue in allNpcDialogues)
        {
          
                targetDialogue = dialogue;
                break;
            
        }

       UIManager.instance.dialogueUIScript.SetUIDialogue(targetDialogue, this);

    }

   

}
