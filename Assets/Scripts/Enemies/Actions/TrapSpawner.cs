using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TrapSpawner: MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField, Tooltip("Prefab of the trap")]
    private GameObject trapPrefab;

    [SerializeField, Tooltip("Heigh of colocation from the floor")]
    private float trapHeightOffset = 0.1f;

    [SerializeField, Tooltip("Limit of traps")]
    public int limitTraps = 3;

    [SerializeField]
    private Animator animator;

    private Coroutine trapCoroutine;
    private bool isPlacingTrapAnimation = false;

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("isPlacingTrap", isPlacingTrapAnimation);
        }
    }
    public void StopTrapCoroutine()
    {
        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
            trapCoroutine = null;
            isPlacingTrapAnimation = false;
        }
    }
    public void PlaceRandomTrap()
    {
        if (trapPrefab == null || limitTraps <= 0) return;
            trapCoroutine = StartCoroutine(PlaceTrapRoutine());   
    }

    private IEnumerator PlaceTrapRoutine()
    {
        if(animator != null)
        {
            isPlacingTrapAnimation = true;

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("PlaceTrap"));

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            isPlacingTrapAnimation = false;
        }

        Vector3 trapPos = new Vector3(transform.position.x, transform.position.y + trapHeightOffset, transform.position.z);

        Trap trap = trapPrefab.GetComponent<Trap>();
        Trap.TrapType randomType = (Trap.TrapType)Random.Range(0, System.Enum.GetValues(typeof(Trap.TrapType)).Length);

        trap.trapType = randomType;
        trap.trapSpawner = this;

        Instantiate(trapPrefab, trapPos, transform.rotation);

        Debug.Log($"Trampa colocada: {randomType} en {transform.position}");
        limitTraps--;

        trapCoroutine = null;
    }

    public bool FinishPlacingTrap()
    {
        return trapCoroutine == null;
    }
}
