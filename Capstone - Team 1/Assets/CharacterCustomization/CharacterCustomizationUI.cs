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
    public static Hat XHAT;
    public static Hat OHAT;
    public static bool IS_AI_GAME;
    //[SerializeField] private GameData gameData;
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
        XHAT = characterCustomization.p1Hat;
        OHAT = characterCustomization.p2Hat;
        IS_AI_GAME = vsAi;
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
