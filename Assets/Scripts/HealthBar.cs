using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;
//Brièvement inspiré de ceci: https://medium.com/@Brian_David/ui-slider-in-unity-using-coroutines-and-mathf-lerp-a93fae02f07f
public class HealthBar : MonoBehaviour
{
    float step = 5f;
    Slider healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    void Start()
    {
        healthBar = GetComponent<Slider>();
    }

    Coroutine coroutine;
    public void UpdateHealthPercent(float healthRemaining, bool gain = false)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if(gain)
        {
            coroutine = StartCoroutine(GainHealth(healthRemaining));
        }
        else
        {
            coroutine = StartCoroutine(ReduceHealth(healthRemaining));
        }
          
        healthText.text = healthRemaining.ToString() + "%";
    }
    
    IEnumerator ReduceHealth(float healthRemaining)
    {
        float newValue = healthRemaining / 100;
        while (healthBar.value > newValue)
        {
            yield return null;
            healthBar.value = Mathf.Lerp(healthBar.value, newValue, step * Time.deltaTime);
        }
        coroutine = null;
    }

    IEnumerator GainHealth(float healthRemaining)
    {
        float newValue = healthRemaining / 100;
        while (healthBar.value < newValue)
        {
            yield return null;
            healthBar.value = Mathf.Lerp(healthBar.value, newValue, step * Time.deltaTime);
        }
        coroutine = null;
    }

}