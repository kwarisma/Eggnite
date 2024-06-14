using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Public Variable

    public static CardManager Instance;

    #endregion

    #region Private Variable

    [SerializeField]
    private Transform startPivot, cardParent;  // Transform for the starting position and parent container of the cards

    [SerializeField]
    private List<CardProfile> cardProfiles;  // List of card profiles to be used

    private Card cardPrefab;  // Prefab for creating card instances

    private int Width = 5;  // Width of the card grid
    private int Length = 6; // Length of the card grid
    private int Stage;
    private float PaddingVertical,PaddingHorizontal;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    #endregion

    #region Public Methode
    // Initialize the card manager
    public void Init(LevelStatus levelStatus)
    {
        Stage = levelStatus.Index;
        startPivot = levelStatus.Pivot;
        Width = levelStatus.Width;
        Length = levelStatus.Length;
        cardPrefab = levelStatus.CardPrefab;
        PaddingVertical = levelStatus.PaddingVertical;
        PaddingHorizontal = levelStatus.PaddingHorizontal;

        if(PlayerPrefs.HasKey("Stage_" + Stage))
        {
            StartCoroutine(InitCard(LoadCard(),false));
        }
        else
           StartCoroutine(InitCard(GetRandomCard(),true));
    }

    public void ResetStage()
    {
        PlayerPrefs.DeleteKey("Stage_" + Stage);
    }

    public bool IsVide(int cardCount)
    {
        return cardCount >= (Width * Length);
    }

    public void RemoveCard(string cardName)
    {
        string loadCardString = PlayerPrefs.GetString("Stage_" + Stage, "");
        int index = loadCardString.IndexOf(cardName);
        loadCardString = loadCardString.Remove(index, cardName.Length);
        loadCardString = loadCardString.Insert(index, "null");
        PlayerPrefs.SetString("Stage_" + Stage, loadCardString);
    }

    #endregion

    #region Private Methode
    // Initialize the cards on the grid
    private IEnumerator InitCard(List<CardProfile> currentProfiles ,bool useSave)
    {
        int index = 0;
        float x = startPivot.position.x;
        float y = startPivot.position.y;

        for (int i = 0; i < Width; i++)
        {
            x = startPivot.position.x;
            for (int j = 0; j < Length; j++)
            {
                if (currentProfiles[index])
                {
                    CreateCard(new Vector3(x, y, 1), currentProfiles[index]);
                    if(useSave)
                         SaveCard(currentProfiles[index].Name);
                    yield return new WaitForSeconds(0.1f);
                }
                x += PaddingHorizontal;  // Move to the next position on the x-axis
                index++;
            }
            y -= PaddingVertical;  // Move to the next position on the y-axis
        }

        GameManager.Instance.InGame = true;
    }

    // Create a card at the specified position with the given profile
    private void CreateCard(Vector3 cardPosition, CardProfile profile)
    {
        Card newCard = Instantiate(cardPrefab, cardPosition, Quaternion.identity);
        newCard.Profile = profile;
        newCard.Init();
        newCard.transform.SetParent(cardParent);
    }

    private void SaveCard(string cardName)
    {
        string loadCardString = PlayerPrefs.GetString("Stage_" + Stage, "");
        loadCardString += cardName + "+";
        PlayerPrefs.SetString("Stage_" + Stage,loadCardString);
    }

    private List<CardProfile> LoadCard()
    {
        List<string> cardStrings = PlayerPrefs.GetString("Stage_" + Stage, "").Split("+").ToList();
        List<CardProfile> res = new List<CardProfile>();
        foreach (string cardString in cardStrings)
        {
            if (cardString == "null")
            {
                res.Add(null);
            }
            else
                res.Add(cardProfiles.Find(cp => cp.Name == cardString));
        }
        return res;
    }

    // Generate a list of random card profiles
    private List<CardProfile> GetRandomCard()
    {
        List<CardProfile> randomCard = new List<CardProfile>();
        List<CardProfile> res = new List<CardProfile>();

        for (int i = 0; i < (Width * Length) / 2; i++)
        {
            int index = Random.Range(0, cardProfiles.Count);
            randomCard.Add(cardProfiles[index]);
            res.Add(cardProfiles[index]);
            cardProfiles.RemoveAt(index);  
        }

        while (randomCard.Count > 0)
        {
            int index = Random.Range(0, randomCard.Count);
            res.Add(randomCard[index]);
            randomCard.RemoveAt(index);
        }
        return res;
    }

    #endregion
}
