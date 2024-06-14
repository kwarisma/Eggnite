using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

// GameManager is responsible for managing game states, level selections, and player interactions.
public class GameManager : MonoBehaviour
{
    #region Public Variables

    public static GameManager Instance; // Singleton instance of GameManager

    public List<LevelStatus> Levels; // List of level statuses
    public bool InGame; // Flag to check if the game is in progress

    // UI Elements
    public Text ScoreTxt, ComboTxt, WinScoreTxt, MaxComboTxt, TimerTxt;
    public Image TimerBar;

    // Game state variables
    public int Level;
    public int Score, Combo, MaxCombo;
    public float ComboValue, TimerValue = 60;
    public Transform ComboBar;
    public GameObject MainScreen, WinScreen, LoseScreen, GameplayScreen;
    public Animation TimerMotion;

    #endregion

    #region Private Variables

    [SerializeField]
    private Transform hideCardPosition; // Position to hide the card
    private int valideCardCount; // Valid card count
    private Card SelectCard; // Currently selected card

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!InGame)
            return;

        // Update timer
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

        // Update combo value
        if (ComboValue >= 0)
        {
            ComboValue -= Time.deltaTime * 0.05f;
            PlayerPrefs.SetFloat("ComboValue" + Level, ComboValue);
            ComboBar.DOScaleX(ComboValue, 0f);
        }
        else
        {
            RemoveComboStrike();
        }
    }

    #endregion

    #region Public Methods

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    // Select and initialize a level
    public void SelectLevel(int level)
    {
        Level = level;
        InitValue();
        valideCardCount = PlayerPrefs.GetInt("valideCardCount_" + Level, 0);
        CardManager.Instance.Init(Levels[level - 1]);
        MainScreen.SetActive(false);
        GameplayScreen.SetActive(true);
    }

    // Check if the player has won
    public void CheckWin()
    {
        if (CardManager.Instance.IsVide(valideCardCount))
            Win();
    }

    // Handle card selection and matching
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

    #endregion

    #region Private Methods

    // Handle losing the game
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

    // Handle winning the game
    private void Win()
    {
        CardManager.Instance.ResetStage();
        GameplayScreen.SetActive(false);
        WinScoreTxt.DOText(Score.ToString(), 0.5f);
        MaxComboTxt.DOText("X " + MaxCombo, 0.5f);
        WinScreen.SetActive(true);
        InGame = false;
        ResetValue();
    }

    // Add score and handle combo strikes
    private void AddScore()
    {
        SoundManager.Instance.PlaySoundFX(1);
        Score += (1 * Combo);
        TimerValue += 5;
        TimerMotion.Play();
        ScoreTxt.DOText(Score.ToString(), 0.5f);
        PlayerPrefs.SetInt("Score_" + Level, Score);
        ScoreTxt.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
        AddComboStrike();
    }

    // Add combo strike
    private void AddComboStrike()
    {
        ComboValue += 0.4f;
        ComboBar.DOScaleX(ComboValue, 0.5f);
        ComboBar.parent.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1f,5);
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

    // Remove combo strike
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

    // Initialize values at the start of a level
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

    // Reset values at the end of a level
    private void ResetValue()
    {
        PlayerPrefs.DeleteKey("valideCardCount_" + Level);
        PlayerPrefs.DeleteKey("Combo_" + Level);
        PlayerPrefs.DeleteKey("MaxCombo_" + Level);
        PlayerPrefs.DeleteKey("Score_" + Level);
        PlayerPrefs.DeleteKey("ComboValue_" + Level);
        PlayerPrefs.DeleteKey("TimerValue_" + Level);
    }

    #endregion
}

// LevelStatus is used to store information about each level
[System.Serializable]
public class LevelStatus
{
    public int Index; // Level index
    public Transform Pivot; // Level pivot point
    public int Width, Length; // Dimensions of the level
    public float PaddingVertical, PaddingHorizontal; // Padding values
    public Card CardPrefab; // Card prefab for the level
}
