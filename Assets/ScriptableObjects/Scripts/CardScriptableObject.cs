using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/ScriptableObjects/Ressource", menuName = "Card")]
public class CardScriptableObject : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
}
