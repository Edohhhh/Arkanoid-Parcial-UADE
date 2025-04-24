using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumericDisplay : MonoBehaviour
{


    [Header("Referencias")]
    public GameObject digitPrefab;     
    public Transform digitParent;      

    [Header("Sprites del 0 al 9")]
    public Sprite[] digitSprites;     

    private List<GameObject> currentDigits = new List<GameObject>();


    private void OnEnable()
    {
        if (LivesManager.Instance != null)
        {
            LivesManager.Instance.livesDisplay = this;
            LivesManager.Instance.ForceRefresh();
        }
    }

    public void UpdateDisplay(int number)
    {
        
        foreach (var digit in currentDigits)
        {
            Destroy(digit);
        }
        currentDigits.Clear();

       
        string scoreStr = number.ToString();

        
        foreach (char c in scoreStr)
        {
            int digit = c - '0';

            GameObject go = Instantiate(digitPrefab, digitParent);
            go.GetComponent<Image>().sprite = digitSprites[digit];

            currentDigits.Add(go);
        }
    }
}
