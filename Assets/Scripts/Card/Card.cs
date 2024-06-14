using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Card class represents an individual card in the game with its behavior and interactions.
public class Card : MonoBehaviour
{
    #region Public Variables

    public CardProfile Profile; // Profile of the card, including sprite and other properties
    public SpriteRenderer FrontSprite; 

    #endregion

    #region Private Variables

    bool canClick = true; // Flag to check if the card can be clicked

    #endregion

    #region Public Methods

    // Initialize the card with its profile and set up its visual appearance
    public void Init()
    {
        FrontSprite.sprite = Profile.Sprite;
        FrontSprite.DOFade(1, 0);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f);
        transform.DORotate(new Vector3(0, 0, 0), 2);
    }

    // Reset the card's rotation
    public void ResetCard()
    {
        transform.DORotate(new Vector3(0, 0, 0), 0.5f);
    }

    public void OnMouseDown()
    {
        FlipCard();
    }

    // Hide the card by moving it to the hide position and scaling it down
    public void HideCard(Transform hidePosition)
    {
        transform.DOPunchRotation(new Vector3(0, 0, 360), 0.75f, 5); 
        transform.DOScale(Vector3.zero, 0.75f); 
        transform.DOMove(hidePosition.position, 0.75f).OnComplete(() => CheckWin()); // Move to hide position and check win condition
        CardManager.Instance.RemoveCard(Profile.Name); 
    }

    // Check if the game is won and destroy the card
    public void CheckWin()
    {
        Destroy(this.gameObject);
        GameManager.Instance.CheckWin();
    }

    #endregion

    #region Private Methods

    // Flip the card to reveal it
    private void FlipCard()
    {
        if (!canClick)
            return;

        canClick = false; // Disable clicking while flipping
        SoundManager.Instance.PlaySoundFX(0); 
        FrontSprite.DOFade(1, 0); 
        transform.DORotate(new Vector3(0, 180, 0), 0.5f).OnComplete(() => Check()); 
    }

    // Check if the card matches another card
    private void Check()
    {
        GameManager.Instance.CheckCard(this);
        canClick = true; 
    }

    #endregion
}
