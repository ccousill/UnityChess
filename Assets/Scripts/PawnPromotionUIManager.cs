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
    }

    public void HidePawnPromotionUI()
    {
        promotionText.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    //initiate pawn promotion in gameManager when an options is clicked
    public void click(string pieceType){
        GameManager.Instance.PawnPromotion(pieceType);
    }

}
