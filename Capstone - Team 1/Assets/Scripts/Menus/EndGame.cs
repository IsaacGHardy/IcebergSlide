using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Penguin p1;
    [SerializeField] private Penguin p2;
    [SerializeField] private Hat p1Hat;
    [SerializeField] private Hat p2Hat;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject newTeamsButton;

    [SerializeField] private TextMeshProUGUI WinnerText;
    private bool vsAi = CharacterCustomizationUI.IS_AI_GAME;
    private bool isP1 = QuixoClass.isPlayer1;
    public static string p1Name = "Player 1";
    public static string p2Name = "Player 2";
    public static bool isFromTutorial = false;

    private void Awake()
    {
        if(CharacterCustomizationUI.XHAT != null) { p1.setHat(CharacterCustomizationUI.XHAT); }
        else { p1.setHat(p1Hat); }
        if (CharacterCustomizationUI.OHAT != null) { p2.setHat(CharacterCustomizationUI.OHAT); }
        else { p2.setHat(p2Hat); }

        if(isFromTutorial)
        {
            playButton.SetActive(false);
            newTeamsButton.SetActive(false);
        }

        if (QuixoClass.isXWin && QuixoClass.isOWin)
        {
            p1.Play("Fear");
            p2.Play("Fear");
            WinnerText.text = "You Tied!";
        }
        else if(QuixoClass.isXWin)
        {
            p1.Play("Jump");
            p2.Play("Death");
            if(vsAi)
            {
                if(isP1)
                {
                    WinnerText.text = "You win!";
                }
                else {
                    WinnerText.text = "You Lose!";
                }
            }
            else
            {
                WinnerText.text = $"{p1Name} Wins!";
            }
        }
        else if (QuixoClass.isOWin)
        {
            p1.Play("Death");
            p2.Play("Jump");
            if (vsAi)
            {
                if (isP1)
                {
                    WinnerText.text = "You Lose!";
                }
                else
                {
                    WinnerText.text = "You Win!";
                }
            }
            else
            {
                WinnerText.text = $"{p2Name} Wins!";
            }
        }
    }

    public void playAgain()
    {
        isFromTutorial = false;
        SceneManager.LoadScene("GameScene");
    }

    public void newTeams()
    {
        isFromTutorial = false;
        SceneManager.LoadScene("TeamSelection");
    }

    public void toMenu()
    {
        isFromTutorial = false;
        SceneManager.LoadScene("Main Menu");
    }
}
