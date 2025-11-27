using UnityEngine;

public class TalkAnimationController : MonoBehaviour
{
    [SerializeField] Animator characterAnimator;
    [SerializeField] Animator mouthAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTalking()
    {
        characterAnimator.SetFloat("speed",1f);
        mouthAnimator.SetBool("talking",true);
    }

    public void StopTalking()
    {
        characterAnimator.SetFloat("speed", 0f);
        mouthAnimator.SetBool("talking", false);
    }
}
