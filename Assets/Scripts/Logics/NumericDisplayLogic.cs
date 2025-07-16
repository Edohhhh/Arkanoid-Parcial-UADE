using UnityEngine;
using UnityEngine.UI;

public class NumericDisplayLogic
{
    private Image[] digitImages;
    private Sprite[] digitSprites;

    public NumericDisplayLogic(GameObject digitPrefab, Transform digitsParent, Sprite[] sprites, int digitCount)
    {
        digitImages = new Image[digitCount];
        digitSprites = sprites;

        for (int i = 0; i < digitCount; i++)
        {
            GameObject digit = Object.Instantiate(digitPrefab, digitsParent);
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

    public void UpdateDisplay(int value)
    {
        SetNumber(value);
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

        int padding = digitImages.Length - str.Length;

        for (int i = 0; i < digitImages.Length; i++)
        {
            if (i < padding)
            {
                digitImages[i].enabled = false;
            }
            else
            {
                int digitIndex = i - padding;
                int digit = str[digitIndex] - '0';
                digitImages[i].sprite = digitSprites[digit];
                digitImages[i].enabled = true;
            }
        }
    }
}
