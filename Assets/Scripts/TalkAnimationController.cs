using UnityEngine;

public class TalkAnimationController : MonoBehaviour
{
    [SerializeField] Animator characterAnimator;
    [SerializeField] Animator mouthAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterAnimator.SetFloat("speed", 0.3f);
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

    public void StopTalkingMouth()
    {
        characterAnimator.SetFloat("speed", 0.3f);
        mouthAnimator.SetBool("talking", false);
    }

    public void StopTalking()
    {
        characterAnimator.SetFloat("speed", 0.3f);
        mouthAnimator.SetBool("talking", false);
    }
}
