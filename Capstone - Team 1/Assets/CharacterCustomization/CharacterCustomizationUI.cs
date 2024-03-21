using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private Button p1ForButton;
    [SerializeField] private Button p1BackButton;
    [SerializeField] private Button p2ForButton;
    [SerializeField] private Button p2BackButton;
    [SerializeField] private Button p1RandButton;
    [SerializeField] private Button p2RandButton;
    [SerializeField] private EventTrigger p1Rotate;
    [SerializeField] private EventTrigger p2Rotate;
    [SerializeField] private Button p1IsAI;
    [SerializeField] private Button p2IsAI;

    public static Hat XHAT;
    public static Hat OHAT;
    public static bool IS_AI_GAME;
    public static int AI_DIFFICULTY;
    private bool vsAi = true;
    private bool isP1 = true;
    private AIDifficulty difficulty = AIDifficulty.IceMaster;
    private Rotate P1RotateScript;
    private Rotate P2RotateScript;

    private void Awake()
    {
        StartCoroutine(waitForInitialization());
    }

    IEnumerator waitForInitialization()
    {
        while(p2ForButton == null || p2BackButton == null || p2RandButton == null || p2Rotate == null) {
            yield return null;
        }


        P1RotateScript = p1Rotate.GetComponent<Rotate>();
        P2RotateScript = p2Rotate.GetComponent<Rotate>();

        disableP2Buttons();
        yield return new WaitForSeconds(1);

        StartCoroutine(aiCustomization());
    }

    IEnumerator aiCustomization()
    {
        while (vsAi)
        {
            if (isP1) { P2RotateScript.setPointerExit(); }
            if (!isP1) { P1RotateScript.setPointerExit(); }
            int random = Random.Range(1, 5);
            switch(random)
            {
                case 1:
                    if (isP1)
                    {
                        p2For();
                    }
                    else
                    {
                        p1For();
                    }
                    break;
                case 2:
                    if (isP1)
                    {
                        p2Back();
                    }
                    else
                    {
                        p1Back();
                    }
                    break;
                case 3:
                    if (isP1)
                    {
                        p2Rand();
                    }
                    else
                    {
                        p1Rand();
                    }
                    break;
                case 4:
                    if (isP1)
                    {
                        P2RotateScript.setPointerEnter();
                    }
                    else
                    {
                        P1RotateScript.setPointerEnter();
                    }
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }
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
        isP1 = !isP1;
        if(vsAi)
        {
            if(isP1)
            {
                P1RotateScript.setPointerExit();
                enableP1Buttons();
                disableP2Buttons();
            }
            else
            {
                P2RotateScript.setPointerExit();
                enableP2Buttons();
                disableP1Buttons();
            }
        }
    }

    public void playAgainst()
    {
        vsAi = !vsAi;
        vsAiButton.gameObject.SetActive(vsAi);
        vsFriendButton.gameObject.SetActive(!vsAi);
        AIDifficultyText.gameObject.SetActive(vsAi);
        AIEasierButton.gameObject.SetActive(vsAi);
        AIHarderButton.gameObject.SetActive(vsAi);
        if (vsAi) { StartCoroutine(aiCustomization()); }
        else {
            P1RotateScript.setPointerExit();
            P2RotateScript.setPointerExit();
        }

        if(!vsAi)
        {
            enableP1Buttons();
            enableP2Buttons();
        }
        else
        {
            if(isP1)
            {
                enableP1Buttons();
                disableP2Buttons();
            }
            else
            {
                enableP2Buttons();
                disableP1Buttons();
            }
        } 
    }

    public void startGame()
    {
        XHAT = characterCustomization.p1Hat;
        OHAT = characterCustomization.p2Hat;
        IS_AI_GAME = vsAi;
        QuixoClass.isPlayer1 = isP1;
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

    public void disableP1Buttons()
    {
        p1ForButton.interactable = false;
        p1BackButton.interactable = false;
        p1RandButton.interactable = false;
        p1Rotate.enabled = false;
        p1IsAI.gameObject.SetActive(true);
    }

    public void enableP1Buttons()
    {
        p1ForButton.interactable = true;
        p1BackButton.interactable = true;
        p1RandButton.interactable = true;
        p1Rotate.enabled = true;
        p1IsAI.gameObject.SetActive(false);

    }
    public void disableP2Buttons()
    {
        p2ForButton.interactable = false;
        p2BackButton.interactable = false;
        p2RandButton.interactable = false;
        p2Rotate.enabled= false;
        p2IsAI.gameObject.SetActive(true);
    }

    public void enableP2Buttons()
    {
        p2ForButton.interactable = true;
        p2BackButton.interactable = true;
        p2RandButton.interactable = true;
        p2Rotate.enabled = true;
        p2IsAI.gameObject.SetActive(false);
    }

}
