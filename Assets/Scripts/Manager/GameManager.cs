using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<LevelStatus> Levels;
    public bool InGame;

    public Text ScoreTxt, ComboTxt, WinScoreTxt, MaxComboTxt,TimerTxt;
    public Image TimerBar;

    public int Level;
    public int Score, Combo, MaxCombo;
    public float ComboValue,TimerValue=60;
    public Transform ComboBar;
    public GameObject MainScreen,WinScreen, LoseScreen, GameplayScreen;
    public Animation TimerMotion;

    [SerializeField]
    private Transform hideCardPosition;
    private int valideCardCount;
    private Card SelectCard;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void SelectLevel(int level)
    {
        Level = level;
        InitValue();
        valideCardCount = PlayerPrefs.GetInt("valideCardCount_" + Level, 0);
        CardManager.Instance.Init(Levels[level - 1]);
        MainScreen.SetActive(false);
        GameplayScreen.SetActive(true);
    }

    private void Update()
    {
        if (!InGame)
            return;
        if (TimerValue >= 0)
        {
            TimerValue -= Time.deltaTime;
            PlayerPrefs.SetFloat("TimerValue_" + Level, TimerValue);
            TimerTxt.text = TimerValue.ToString("00");
            float fillAmount = TimerValue / 60;
            if (fillAmount > 1)
                fillAmount = 1;
            TimerBar.fillAmount = fillAmount;
        }
        else
        {
            Lose();
        }

        if (ComboValue >= 0)
        {
            ComboValue -= Time.deltaTime*0.05f;
            PlayerPrefs.SetFloat("ComboValue" + Level, ComboValue);
            ComboBar.DOScaleX(ComboValue, 0f);
        }
        else
        {
            RemoveComboStrike();
        }
    }

    private void Lose()
    {
        CardManager.Instance.ResetStage();
        GameplayScreen.SetActive(false);
        WinScoreTxt.DOText(Score.ToString(), 0.5f);
        MaxComboTxt.DOText("X " + MaxCombo, 0.5f);
        LoseScreen.SetActive(true);
        InGame = false;
        ResetValue();
    }

    public void CheckWin()
    {
        if (CardManager.Instance.IsVide(valideCardCount))
            Win();
    }

    private void Win()
    {
        CardManager.Instance.ResetStage();
        GameplayScreen.SetActive(false);
        WinScoreTxt.DOText(Score.ToString(), 0.5f);
        MaxComboTxt.DOText("X "+MaxCombo, 0.5f);
        WinScreen.SetActive(true);
        InGame = false;
        ResetValue();
    }

    public void CheckCard(Card card)
    {
        if (SelectCard)
        {
            if (SelectCard.Profile == card.Profile && SelectCard != card)
            {
                SelectCard.HideCard(hideCardPosition);
                card.HideCard(hideCardPosition);
                valideCardCount += 2;
                PlayerPrefs.SetInt("valideCardCount_" + Level, valideCardCount);
                AddScore();
                SelectCard = null;
            }
            else
            {
                SoundManager.Instance.PlaySoundFX(2);
                SelectCard.ResetCard();
                card.ResetCard();
                SelectCard = null;
            }  
        }
        else
            SelectCard = card;
 
    }

    private void AddScore()
    {
        SoundManager.Instance.PlaySoundFX(1);
        Score += (1*Combo);
        TimerValue += 5;
        TimerMotion.Play();
        ScoreTxt.DOText(Score.ToString(), 0.5f);
        PlayerPrefs.SetInt("Score_" + Level, Score);
        ScoreTxt.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
        AddComboStrike();
    }

    private void AddComboStrike()
    {
        ComboValue += 0.4f;
        ComboBar.DOScaleX(ComboValue, 1);
        if (ComboValue >= 1)
        {
            ComboValue = 0.3f;
            PlayerPrefs.SetFloat("ComboValue" + Level, ComboValue);
            ComboBar.DOScaleX(ComboValue, 1);
            Combo++;
            PlayerPrefs.SetInt("Combo_" + Level, Combo);
            ComboTxt.DOText(Combo.ToString(), 0.5f);
            ComboTxt.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
            if (MaxCombo <= Combo)
            {
                MaxCombo = Combo;
                PlayerPrefs.SetInt("MaxCombo" + Level, MaxCombo);
            }
        }
    }

    private void RemoveComboStrike()
    {
        if (Combo > 1)
        {
            ComboValue = 1;
            Combo--;
            ComboTxt.DOText(Combo.ToString(), 0.5f);
            ComboTxt.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
           
        }
    }

    private void InitValue()
    {
        Combo = PlayerPrefs.GetInt("Combo_" + Level, 1);
        ComboTxt.DOText(Combo.ToString(), 0);

        TimerValue = PlayerPrefs.GetFloat("TimerValue_" + Level, 60);
        TimerTxt.text = TimerValue.ToString("00");

        MaxCombo = PlayerPrefs.GetInt("MaxCombo" + Level, 1);

        Score = PlayerPrefs.GetInt("Score_" + Level, 0);
        ScoreTxt.DOText(Score.ToString(), 1);

        ComboValue = PlayerPrefs.GetFloat("ComboValue_" + Level, 0);

    }


    private void ResetValue()
    {
        PlayerPrefs.DeleteKey("valideCardCount_" + Level);
        PlayerPrefs.DeleteKey("Combo_" + Level);
        PlayerPrefs.DeleteKey("MaxCombo_" + Level);
        PlayerPrefs.DeleteKey("Score_" + Level);
        PlayerPrefs.DeleteKey("ComboValue_" + Level);
        PlayerPrefs.DeleteKey("TimerValue_" + Level);

    }



}

[System.Serializable]
public class LevelStatus
{
    public int Index;
    public Transform Pivot;
    public int Width, Length;
    public float PaddingVertical, PaddingHorizontal;
    public Card CardPrefab;
}
