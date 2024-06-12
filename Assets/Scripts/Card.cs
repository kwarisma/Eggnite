using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public CardScriptableObject ScriptableObject;
    public SpriteRenderer FrontSprite;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        FrontSprite.sprite = ScriptableObject.Sprite;
        FrontSprite.DOFade(1, 0);
    }

    public void ResetCard()
    {
        transform.DORotate(new Vector3(0, 0, 0), 0.5f).OnComplete(() => FrontSprite.DOFade(0, 0));
    }

    public void OnMouseDown()
    {
        HideCard();
    }


    public void HideCard()
    {
        transform.DOPunchRotation(new Vector3(0, 0, 360), 1,5);
        transform.DOScale(Vector3.zero, 1);

    }



    private void FlipCard()
    {
        FrontSprite.DOFade(1, 0);
        transform.DORotate(new Vector3(0, transform.eulerAngles.y + 180, 0), 0.5f);
    }

    
}
