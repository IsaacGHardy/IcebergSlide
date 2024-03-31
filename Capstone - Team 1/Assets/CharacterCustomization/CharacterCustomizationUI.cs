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
    [SerializeField] private TMP_InputField p1Name;
    [SerializeField] private TMP_InputField p2Name;
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
    public static bool IS_AI_GAME = true;
    public static int AI_DIFFICULTY;
    private bool vsAi = true;
    private bool isP1 = true;
    private AIDifficulty difficulty = AIDifficulty.IceMaster;
    private Rotate P1RotateScript;
    private Rotate P2RotateScript;
    private string[] aiNames = new string[]
{
    "Frosty",
    "Blizzard",
    "Pebbles",
    "Chilly",
    "Waddle",
    "Squawk",
    "Flurry",
    "Iceberg",
    "Squeak",
    "Glide",
    "Puff",
    "Flipper",
    "Snowy",
    "Slide",
    "Igloo",
    "Tundra",
    "Bubbles",
    "Frost",
    "Pingu",
    "Glacier"
};

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

        vsAi = IS_AI_GAME;
        p1Name.text = EndGame.p1Name;
        p2Name.text = EndGame.p2Name;

        p1Name.onEndEdit.AddListener(defaultName);
        p2Name.onEndEdit.AddListener(defaultName);

        if (vsAi) { 
            disableP2Buttons();
        }
        else
        {
            enableP2Buttons();
            vsAiButton.gameObject.SetActive(vsAi);
            vsFriendButton.gameObject.SetActive(!vsAi);
            AIDifficultyText.gameObject.SetActive(vsAi);
            AIEasierButton.gameObject.SetActive(vsAi);
            AIHarderButton.gameObject.SetActive(vsAi);
        }
        yield return new WaitForSeconds(1);



        StartCoroutine(aiCustomization());
    }
    IEnumerator aiCustomization()
    {
        while (vsAi)
        {
            if (isP1) { P2RotateScript.setPointerExit(); }
            if (!isP1) { P1RotateScript.setPointerExit(); }
            int random = Random.Range(1, 6);
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
                case 5:
                    generateRandName();
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    private void generateRandName()
    {
        int random = Random.Range(0, 20);
        if(isP1)
        {
            p2Name.text = aiNames[random];
        }
        else
        {
            p1Name.text = aiNames[random];
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
        swapNicknames();
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

    private void swapNicknames()
    {
        string temp = p1Name.text;
        p1Name.text = p2Name.text;
        p2Name.text = temp;
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
        EndGame.p1Name = p1Name.text;
        EndGame.p2Name = p2Name.text;
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
        p1Name.interactable = false;
        p1IsAI.gameObject.SetActive(true);
    }
    public void enableP1Buttons()
    {
        p1ForButton.interactable = true;
        p1BackButton.interactable = true;
        p1RandButton.interactable = true;
        p1Rotate.enabled = true;
        p1Name.interactable = true;
        p1IsAI.gameObject.SetActive(false);

    }
    public void disableP2Buttons()
    {
        p2ForButton.interactable = false;
        p2BackButton.interactable = false;
        p2RandButton.interactable = false;
        p2Rotate.enabled= false;
        p2Name.interactable = false;
        p2IsAI.gameObject.SetActive(true);
    }

    public void enableP2Buttons()
    {
        p2ForButton.interactable = true;
        p2BackButton.interactable = true;
        p2RandButton.interactable = true;
        p2Rotate.enabled = true;
        p2Name.interactable = true;
        p2IsAI.gameObject.SetActive(false);
    }

    private void defaultName(string text)
    {
        bool isP1Text = (EventSystem.current.currentSelectedGameObject.name == "P1Text");
        if (text == "" || text == (isP1Text ? p2Name.text : p1Name.text))
        {
            if (isP1Text)
            {
                p1Name.text = "Player 1";
            }
            else
            {
                p2Name.text = "Player 2";
            }
        }
    }

    private void OnDestroy()
    {
        if (p1Name != null) { p1Name.onEndEdit.RemoveAllListeners(); }
        if (p2Name != null) { p2Name.onEndEdit.RemoveAllListeners(); }
    }

}
