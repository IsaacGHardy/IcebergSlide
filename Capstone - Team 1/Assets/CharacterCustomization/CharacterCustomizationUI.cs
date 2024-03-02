using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] private CharacterCustomization characterCustomization;
    [SerializeField] private Button vsAiButton;
    [SerializeField] private Button vsFriendButton;
    [SerializeField] private Button AIEasierButton;
    [SerializeField] private Button AIHarderButton;
    [SerializeField] private TextMeshProUGUI AIDifficultyText;
    public static Hat XHAT;
    public static Hat OHAT;
    public static bool IS_AI_GAME;
    public static int AI_DIFFICULTY;
    private bool vsAi = true;
    private AIDifficulty difficulty = AIDifficulty.IceMaster;
    enum AIDifficulty
    {
        Eggling = 0,
        WaddleWarrior = 1,
        IceMaster = 2,
        ArcticLegend = 3,
        EmperorOfTheIce = 4
    }
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
        AIDifficultyText.gameObject.SetActive(vsAi);
        AIEasierButton.gameObject.SetActive(vsAi);
        AIHarderButton.gameObject.SetActive(vsAi);


    }

    public void startGame()
    {
        XHAT = characterCustomization.p1Hat;
        OHAT = characterCustomization.p2Hat;
        IS_AI_GAME = vsAi;
        AI_DIFFICULTY = (int)difficulty;
        Debug.Log(AI_DIFFICULTY);
        SceneManager.LoadScene("GameScene");
    }

    public void p1Rand()
    {
        characterCustomization.setRandomHat(true);
    }

    public void p2Rand()
    {
        characterCustomization.setRandomHat(false);
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void AiHarder()
    {
        if(difficulty == AIDifficulty.EmperorOfTheIce)
        {
            difficulty = AIDifficulty.Eggling;
        }
        else
        {
            ++difficulty;
        }
        setAIText(difficulty);
    }

    public void AiEasier()
    {
        if (difficulty == AIDifficulty.Eggling)
        {
            difficulty = AIDifficulty.EmperorOfTheIce;
        }
        else
        {
            --difficulty;
        }
        setAIText(difficulty);
    }

    private void setAIText(AIDifficulty diff)
    {
        switch(diff)
        {
            case AIDifficulty.Eggling:
                AIDifficultyText.text = "Eggling";
                break;
            case AIDifficulty.WaddleWarrior:
                AIDifficultyText.text = "Waddle Warrior";
                break;
            case AIDifficulty.IceMaster:
                AIDifficultyText.text = "Ice Master";
                break;
            case AIDifficulty.ArcticLegend:
                AIDifficultyText.text = "Arctic Legend";
                break;
            case AIDifficulty.EmperorOfTheIce:
                AIDifficultyText.text = "Emperor Of The Ice";
                break;
        }
    }

}
