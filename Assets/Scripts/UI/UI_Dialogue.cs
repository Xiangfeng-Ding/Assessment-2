using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{


    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI hintText;

    //  [SerializeField] private Image speakerIconImage; // 说话者头像


    [Header("打字机设置")]
    [SerializeField] private float intervalTimeBetweenWord = 0.05f;


    // 对话状态
    private DialogueData currentDialogue; // 当前对话数据（含角色信息）
    private int currentSentenceIndex; // 当前句子索引
    private Coroutine typewriterCoroutine;
    private bool isTyping;
    private string currentFullText;
    public bool isDialogueActive;// 对话是否激活

    // 记录当前是和哪个NPC在对话
    public Npc_Dialogue currentSpeakingNpc;

    private void OnEnable()
    {

    }
    private void Awake()
    {
        isDialogueActive = false;

    }

    // 外部调用：初始化指定角色的对话
    public void SetUIDialogue(DialogueData dialogue, Npc_Dialogue npc)
    {
        currentSpeakingNpc = npc;//记录是谁在说话
        currentDialogue = dialogue;
        currentSentenceIndex = 0;
   
        ShowCurrentSentence();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isTyping)
            {
                StopTypeWriter(); // 加速打字
            }
            else
            {
                // 切换下一句
                if (currentSentenceIndex < currentDialogue.sentences.Length - 1)
                {
                    currentSentenceIndex++;
                    ShowCurrentSentence();
                }
                // 对话结束
                else
                {
                    EndDialogue();

                }
            }
        }
    }

    // 显示当前句子
    public void ShowCurrentSentence()
    {
        StopTypeWriter();

        currentFullText = currentDialogue.sentences[currentSentenceIndex];
        dialogueText.text = "";
        isTyping = true;
        typewriterCoroutine = StartCoroutine(TypewriterCoroutine());
        hintText.text = "Click to speed up";
    }

    private void StopTypeWriter()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        dialogueText.text = currentFullText;
        isTyping = false;
        hintText.text = "Click to continue";
    }
    // 打字机协程
    private IEnumerator TypewriterCoroutine()
    {
        int wordIndex = 0;
        while (wordIndex < currentFullText.Length)
        {
            dialogueText.text += currentFullText[wordIndex];
            wordIndex++;
            yield return new WaitForSecondsRealtime(intervalTimeBetweenWord);
        }
        isTyping = false;
        hintText.text = "Click to continue";
    }



    // 结束对话
    private void EndDialogue()
    {
        isDialogueActive = false;
        currentSentenceIndex = 0;
      

        gameObject.SetActive(false);
        UIManager.instance.SwitchTo(UIManager.instance.gamePanel);


    }
}
