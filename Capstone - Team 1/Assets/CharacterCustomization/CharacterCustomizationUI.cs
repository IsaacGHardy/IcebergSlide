using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] private CharacterCustomization characterCustomization;
    [SerializeField] private Button vsAiButton;
    [SerializeField] private Button vsFriendButton;
    public static Hat XHat;
    public static Hat OHat;
    public static bool isAiGame;
    private bool vsAi = true;

    public void p1For()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.ForwardHat(true);
    }

    public void p1Back()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.BackwardHat(true);

    }

    public void p2For()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.ForwardHat(false);

    }

    public void p2Back()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.BackwardHat(false);
    }

    public void switchSides()
    {
        characterCustomization.switchSides();
    }

    public void playAgainst()
    {
        vsAi = !vsAi;
        vsAiButton.gameObject.SetActive(vsAi);
        vsFriendButton.gameObject.SetActive(!vsAi);
    }

    public void startGame()
    {
        XHat = characterCustomization.p1Hat;
        OHat = characterCustomization.p2Hat;
        isAiGame = vsAi;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void p1Rand()
    {
        characterCustomization.setRandomHat(true);
    }

    public void p2Rand()
    {
        characterCustomization.setRandomHat(false);
    }

}
