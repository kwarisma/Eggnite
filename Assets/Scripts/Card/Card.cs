using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public CardProfile Profile;
    public SpriteRenderer FrontSprite;

    bool canClick = true;

    public void Init()
    {
        FrontSprite.sprite = Profile.Sprite;
        FrontSprite.DOFade(1, 0);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f);
        transform.DORotate(new Vector3(0, 0, 0), 2);
    }

    public void ResetCard()
    {
        transform.DORotate(new Vector3(0, 0, 0), 0.5f);
    }

    public void OnMouseDown()
    {
        FlipCard();
    }

    public void HideCard(Transform hidePosition)
    {
        transform.DOPunchRotation(new Vector3(0, 0, 360), 0.75f,5);
        transform.DOScale(Vector3.zero, 0.75f);
        transform.DOMove(hidePosition.position, 0.75f).OnComplete(()=> CheckWin());
        CardManager.Instance.RemoveCard(Profile.Name);
    }

    private void FlipCard()
    {
        if (!canClick)
            return;
        canClick = false;
        SoundManager.Instance.PlaySoundFX(0);
        FrontSprite.DOFade(1, 0);
        transform.DORotate(new Vector3(0, 180, 0), 0.5f).OnComplete(() => Check());
    }

    private void Check()
    {
        GameManager.Instance.CheckCard(this);
        canClick = true;
    }

    public void CheckWin()
    {
        Destroy(this.gameObject);
        GameManager.Instance.CheckWin();
    }
}
