using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{


    [Header("角色信息")]
    public string speakerName; // 说话者名字
    public Sprite speakerIcon; // 说话者头像
    public bool isPlayer; // 是否是主角说的话

    [Header("对话内容")]
    [TextArea(2, 10)]//文本框默认显示 2 行高度（不会挤成 1 行，方便看内容)文本框最多可扩展到 10 行高度（超过后自动出现滚动条，避免占用过多编辑器空间）
    public string[] sentences; // 该角色的台词

}
