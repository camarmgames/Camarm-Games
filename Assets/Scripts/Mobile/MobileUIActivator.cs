using UnityEngine;

public class MobileUIActivator: MonoBehaviour
{
    [Header("UI for mobile")]
    public GameObject mobileUI;

    private void Start()
    {

        bool isMobile = MobilePlatformDetector.IsMobile();

        mobileUI.SetActive(isMobile);
    }

}
