using StarterAssets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public ThirdPersonController player;


    public GameObject[] UIelment;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject dialoguePanel;
    public GameObject settlePanel; // 结算面板
    public GameObject pausePanel;

    
    [HideInInspector] public UI_Game gameUIScript;
    [HideInInspector] public UI_Dialogue dialogueUIScript;

    public Button settleNextButton;
    public Button settleRestartButton;


    private bool _isPaused=false;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        gameUIScript = gamePanel.GetComponent<UI_Game>();
        dialogueUIScript = dialoguePanel.GetComponent<UI_Dialogue>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        settleNextButton.gameObject.SetActive(true);
        settleRestartButton.gameObject.SetActive(false);
        SwitchTo(startPanel);

    }

    // Update is called once per frame
    void Update()
    {
        // 仅在游戏面板时，按ESC切换暂停/继续
        if (gamePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    //极简暂停/继续切换方法
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            // 暂停逻辑
            Time.timeScale = 0f; // 时间暂停
            pausePanel.SetActive(true); // 显示暂停面板
            Cursor.lockState = CursorLockMode.None; // 显示鼠标
            Cursor.visible = true;
            player.ifPlayerCanMove = false; // 禁止玩家移动
        }
        else
        {
            // 继续逻辑
            Time.timeScale = 1f; // 时间恢复
            pausePanel.SetActive(false); // 隐藏暂停面板
            Cursor.lockState = CursorLockMode.Locked; // 隐藏鼠标
            Cursor.visible = false;
            player.ifPlayerCanMove = true; // 允许玩家移动
        }
    }

    public void SwitchTo(GameObject targetPanel)
    {
        foreach (GameObject go in UIelment)
        {
            go.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.ifPlayerCanMove = false;

        if (targetPanel==gamePanel)
        {
           
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.ifPlayerCanMove=true;
        }

        if (targetPanel == dialoguePanel)
            player.ifPlayerCanMove = false;
            


        targetPanel.SetActive(true);
    }

    public void SetRestartButton()

    {
        settleNextButton.gameObject.SetActive(false);
        settleRestartButton.gameObject.SetActive(true);

    }
    public void QuitTheGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // 编辑器下停止播放
#else
        Application.Quit(); // 打包后退出游戏
#endif
    }


}
