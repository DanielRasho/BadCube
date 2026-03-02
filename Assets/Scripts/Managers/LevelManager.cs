using System;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int coinsRequired;

    [SerializeField] private GameObject victoryUI;
    private int coins = 0;

    // ---- COINS -----
    public int Coins
    {
        get { return coins; }
    }

    public void AddCoin()
    {
        if (coins < coinsRequired)
            coins++;
        Debug.Log("Coin Added: " + coins);
    }

    private void FixedUpdate()
    {
        if (coins == coinsRequired)
        {
            victoryUI.SetActive(true);
        }
    }
}
