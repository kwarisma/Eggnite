using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField]
    private Transform startPivot, cardParent;  // Transform for the starting position and parent container of the cards

    [SerializeField]
    private List<CardProfile> cardProfiles;  // List of card profiles to be used

    [SerializeField]
    private Card cardPrefab;  // Prefab for creating card instances

    public int Width = 5;  // Width of the card grid
    public int Length = 6; // Length of the card grid
    public int Stage;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    // Initialize the card manager
    public void Init()
    {
        if(PlayerPrefs.HasKey("Stage_" + Stage))
        {
            InitCard(LoadCard(),false);
        }
        else
            InitCard(GetRandomCard(),true);
    }

    public void ResetStage()
    {
        PlayerPrefs.DeleteKey("Stage_" + Stage);
    }

    public bool IsVide()
    {
        return cardParent.childCount <= 1;
    }

    // Initialize the cards on the grid
    private void InitCard(List<CardProfile> currentProfiles ,bool useSave)
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
                }
                x += 2;  // Move to the next position on the x-axis
                index++;
            }
            y -= 2;  // Move to the next position on the y-axis
        }
    }

    // Create a card at the specified position with the given profile
    private void CreateCard(Vector3 cardPosition, CardProfile profile)
    {
        Card newCard = Instantiate(cardPrefab, cardPosition, Quaternion.identity);
        newCard.Profile = profile;
        newCard.Init();
        newCard.transform.SetParent(cardParent);
    }

    public void RemoveCard(string cardName)
    {
        string loadCardString = PlayerPrefs.GetString("Stage_" + Stage, "");
        int index = loadCardString.IndexOf(cardName);
        loadCardString=loadCardString.Remove(index, cardName.Length);
        loadCardString=loadCardString.Insert(index, "null");
        PlayerPrefs.SetString("Stage_" + Stage, loadCardString);
    }

    public void SaveCard(string cardName)
    {
        string loadCardString = PlayerPrefs.GetString("Stage_" + Stage, "");
        loadCardString += cardName + "+";
        PlayerPrefs.SetString("Stage_" + Stage,loadCardString);
    }

    public List<CardProfile> LoadCard()
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
}
