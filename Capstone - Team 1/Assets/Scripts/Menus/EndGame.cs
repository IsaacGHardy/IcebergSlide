using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Penguin p1;
    [SerializeField] private Penguin p2;
    [SerializeField] private TextMeshProUGUI WinnerText;
    private bool vsAi = CharacterCustomizationUI.IS_AI_GAME;
    private bool isP1 = QuixoClass.isPlayer1;

    private void Awake()
    {
        p1.setHat(CharacterCustomizationUI.XHAT);
        p2.setHat(CharacterCustomizationUI.OHAT);
        
        if(QuixoClass.isXWin && QuixoClass.isOWin)
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
                WinnerText.text = "Player 1 Wins!";
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
                WinnerText.text = "Player 2 Wins!";
            }
        }
    }

    public void playAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void newTeams()
    {
        SceneManager.LoadScene("TeamSelection");
    }

    public void toMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
