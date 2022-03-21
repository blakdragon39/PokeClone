using System.Collections;
using UnityEngine;

public class HPBar : MonoBehaviour {
    
    [SerializeField] private GameObject health;

    public void SetHP(float hpNormalized) {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP) {
        float currentHP = health.transform.localScale.x;
        float change = currentHP - newHP;

        while (currentHP - newHP > Mathf.Epsilon) {
            currentHP -= change * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }
        
        SetHP(newHP);
    }
}