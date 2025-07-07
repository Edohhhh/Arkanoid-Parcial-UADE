using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumericDisplay : MonoBehaviour
{
    public GameObject digitPrefab;                // Prefab que debe tener un componente Image
    public Transform digitsParent;                // Contenedor donde instanciar los dígitos
    public Sprite[] digitSprites;                 // Sprites del 0 al 9
    public int digitCount = 5;                    // Cuántos dígitos mostrar como máximo

    private Image[] digitImages;

    private void Awake()
    {
        SetupDigits(digitCount);
    }

    private void SetupDigits(int count)
    {
        digitImages = new Image[count];

        for (int i = 0; i < count; i++)
        {
            GameObject digit = Instantiate(digitPrefab, digitsParent);
            digit.transform.localScale = Vector3.one;

            Image img = digit.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogError("❌ El digitPrefab no tiene un componente Image.");
                return;
            }

            digitImages[i] = img;
            img.enabled = false;
        }
    }

    public void SetNumber(int number)
    {
        if (number < 0) number = 0;

        string str = number.ToString();
        if (str.Length > digitImages.Length)
        {
            Debug.LogWarning($"⚠️ Número {number} demasiado largo para el display (max {digitImages.Length} dígitos). Se truncará.");
            str = str.Substring(str.Length - digitImages.Length);
        }

        for (int i = 0; i < digitImages.Length; i++)
        {
            if (i < str.Length)
            {
                int digit = str[str.Length - 1 - i] - '0';
                digitImages[i].sprite = digitSprites[digit];
                digitImages[i].enabled = true;
            }
            else
            {
                digitImages[i].enabled = false;
            }
        }
    }

    public void UpdateDisplay(int value)
    {
        SetNumber(value);
    }
}
