using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Score, Combo,MaxCombo;

    public Text ScoreTxt, ComboTxt,WinScoreTxt,MaxComboTxt;

    public Card SelectCard;

    [SerializeField]
    private Transform hideCardPosition;

    public GameObject WinScreen,LoseScreen,GameplayScreen;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        CardManager.Instance.Init();
    }

    public void CheckWin()
    {
        if (CardManager.Instance.IsVide())
            Win();
    }

    public void Win()
    {
        CardManager.Instance.ResetStage();
        GameplayScreen.SetActive(false);
        WinScoreTxt.DOText(Score.ToString(), 0.5f);
        WinScreen.SetActive(true);
    }

    public void CheckCard(Card card)
    {
        if (SelectCard)
        {
            if (SelectCard.Profile == card.Profile&& SelectCard!=card)
            {
                SelectCard.HideCard(hideCardPosition);
                card.HideCard(hideCardPosition);
                AddScore();
                SelectCard = null;
            }
            else
            {
                SelectCard.ResetCard();
                card.ResetCard();
                SelectCard = null;
            }
        }
        else
            SelectCard = card;
 
    }

    public void AddScore()
    {
        Score += 1;
        ScoreTxt.DOText(Score.ToString(), 0.5f);
        ScoreTxt.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
    }
}
