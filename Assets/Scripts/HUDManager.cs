using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    [Header("Barras")]
    public Slider hpSlider;
    public Slider xpSlider;
    
    [Header("Textos")]
    public TMP_Text levelText;
    public TMP_Text hpText;

    [Header("Velocidade")]
    public float fillSpeed = 2f;        
    public float hpAnimationSpeed = 5f; 

    private Queue<int> levelQueue = new Queue<int>(); 
    private float finalTargetXP = 0f;
    private bool isAnimatingXP = false;
    private Coroutine hpCoroutine;

    // A Trava
    private bool isLockedAtMax = false;

    void Start()
    {
        if (xpSlider != null) xpSlider.value = 0;
        if (levelText != null) levelText.text = "LEVEL 1";
    }

    // --- NOVA FUNÇÃO DE SEGURANÇA MÁXIMA ---
    // LateUpdate roda depois de tudo. Ninguém ganha dele.
    void LateUpdate()
    {
        if (isLockedAtMax)
        {
            if (levelText != null && levelText.text != "MAX LEVEL")
            {
                levelText.text = "MAX LEVEL";
                levelText.color = Color.yellow;
            }
            if (xpSlider != null && xpSlider.value < 1f)
            {
                xpSlider.value = 1f;
            }
        }
    }
    // ----------------------------------------

    public void UpdateHealthBar(float current, float max)
    {
        if (hpText != null) hpText.text = "HP " + (int)current + "/" + (int)max;

        if (hpSlider != null)
        {
            float targetValue = current / max;
            if (hpCoroutine != null) StopCoroutine(hpCoroutine);
            hpCoroutine = StartCoroutine(AnimateHP(targetValue));
        }
    }

    IEnumerator AnimateHP(float target)
    {
        while (Mathf.Abs(hpSlider.value - target) > 0.001f)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, target, Time.unscaledDeltaTime * hpAnimationSpeed);
            yield return null;
        }
        hpSlider.value = target;
    }

    public void QueueLevelUp(int newLevelNumber)
    {
        if (isLockedAtMax) return; 
        levelQueue.Enqueue(newLevelNumber);
        CheckQueue();
    }

    public void SetFinalTargetXP(float current, float max)
    {
        if (isLockedAtMax) return;
        finalTargetXP = current / max;
        CheckQueue();
    }

    void CheckQueue()
    {
        if (!isAnimatingXP && !isLockedAtMax) StartCoroutine(ProcessAnimationQueue());
    }

    IEnumerator ProcessAnimationQueue()
    {
        isAnimatingXP = true;
        
        while (levelQueue.Count > 0)
        {
            if (isLockedAtMax) yield break;

            int nextLevel = levelQueue.Dequeue();
            
            if (xpSlider != null) 
            {
                while (xpSlider.value < 0.999f)
                {
                    if (isLockedAtMax) yield break;
                    xpSlider.value = Mathf.MoveTowards(xpSlider.value, 1f, fillSpeed * Time.unscaledDeltaTime);
                    yield return null;
                }
                xpSlider.value = 1f;
                yield return new WaitForSecondsRealtime(0.05f); 
                xpSlider.value = 0f; 
            }
            
            // Só muda o texto se não estiver travado
            if (levelText != null && !isLockedAtMax) levelText.text = "LEVEL " + nextLevel;
        }

        if (xpSlider != null)
        {
            while (Mathf.Abs(xpSlider.value - finalTargetXP) > 0.001f)
            {
                if (isLockedAtMax) yield break;
                xpSlider.value = Mathf.MoveTowards(xpSlider.value, finalTargetXP, fillSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            xpSlider.value = finalTargetXP;
        }
        
        isAnimatingXP = false;
    }

    // --- FUNÇÃO BLINDADA ATUALIZADA ---
    public void ShowMaxLevelVisuals()
    {
        // 1. Ativa a trava
        isLockedAtMax = true;

        // 2. LIMPEZA TOTAL: Joga fora qualquer "Level 4", "Level 5" que esteja na fila
        levelQueue.Clear(); 
        StopAllCoroutines();
        
        // 3. Força os valores imediatamente
        if (xpSlider != null) xpSlider.value = 1f;
        
        if (levelText != null) 
        {
            levelText.text = "MAX LEVEL";
            levelText.color = Color.yellow; 
            levelText.ForceMeshUpdate(); 
        }
    }

    
}