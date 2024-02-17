using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PawnPromotionUIManager : MonoBehaviour
{
    [SerializeField] public Canvas promotionText;
    public void ShowPawnPromotionUI()
    {
        promotionText.gameObject.SetActive(true);
        Time.timeScale = 0;
        // Additional logic to handle UI display
    }

    public void HidePawnPromotionUI()
    {
        promotionText.gameObject.SetActive(false);
        Time.timeScale = 1;
        // Additional logic to handle UI hide
    }

    public void click(string pieceType){
        GameManager.Instance.PawnPromotion(pieceType);
    }

}
