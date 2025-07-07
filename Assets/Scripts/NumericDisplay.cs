using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NumericDisplay : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] digits;

    [Header("Referencia visual")]
    public GameObject digitPrefab;
    public Transform digitParent;

    private List<GameObject> currentDigits = new();

    public void SetNumber(int value)
    {
        foreach (var digitGO in currentDigits)
            Destroy(digitGO);

        currentDigits.Clear();

        string str = Mathf.Max(0, value).ToString();

        foreach (char c in str)
        {
            GameObject newDigit = Instantiate(digitPrefab, digitParent);
            int index = c - '0';
            newDigit.GetComponent<Image>().sprite = digits[index];
            currentDigits.Add(newDigit);
        }
    }

    public void Refresh()
    {
        // placeholder opcional si querés refrescar en algún momento
    }
}
