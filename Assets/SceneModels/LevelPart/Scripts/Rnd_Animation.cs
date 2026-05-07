using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ithappy
{
    public class Rnd_Animation : MonoBehaviour
    {

        Animator animator;
        float offsetAnim;

        [SerializeField] string titleAnim;


        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            offsetAnim = Random.Range(0f, 1f);


            if (animator != null && animator.HasState(0, Animator.StringToHash("你的动画名")))
            {
                animator.Play("你的动画名");
            }
            else
            {
              //  Debug.LogError($"动画状态不存在！请检查动画名拼写和控制器：{gameObject.name}", this);
            }
        }
    }
}
