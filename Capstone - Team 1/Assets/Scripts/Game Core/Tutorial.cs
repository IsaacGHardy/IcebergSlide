using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorial;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] QuixoClass quixoClass;
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject closeBtn;



    new Camera camera;
    private void Awake()
    {
        tutorialRectTransform = tutorial.GetComponent<RectTransform>();
        new Camera();
        camera = Camera.main;
        tutorialText.text = tutorialTexts[0];
        quixoClass.startTutorial();
        CharacterCustomizationUI.AI_DIFFICULTY = '6';
        EndGame.isFromTutorial = true;

    }

    List<string> tutorialTexts = new List<string>
{
    //0
    "Start your turn by selecting a penguin outside the board.\n\n" +
    "Try the green penguin.",
    //1
    "Slide your selected penguin by clicking another penguin.\n\n" +
    "They'll fill the empty spot.",
    //2
    "Rival penguins are making their move!",
    //3
    "Choose the green penguin for your turn.\n\n" +
    "You can't slide opponent penguins!",
    //4
    "Select the blue penguin to end your turn.\n\n" +
    "Slide your penguin to the end of a row or column.",
    //5
    "Rival penguins are on the move!",
    //6
    "Win by lining up 5 penguins in a row, column, or diagonal.\n\n" +
    "Think of it as icy Tic Tac Toe!",
    //7
    //9
    "Find instructions and tips in the pause menu if you're puzzled.",
    //12
    "Propose a draw if victory seems unlikely.\n\n" +
    "Your opponent may not accept!",
    //13
    "You can offer a draw only when it's your turn.",
    //14
    "Think you're an ice master?\n\n" +
    "Close this and glide to victory!\n\n" +
    "Need help? Keep this open for guidance."
};
    List<int> indexesAdvanceOnCLick = new List<int>
    {
        0, 3
    };

    private int textIndex = 0;
    private int maxIndex = 0;
    private float moveSpeed = 5.0f;
    private bool readyForToMove = false;
    private bool tutorialIsGoing = true;
    private RectTransform tutorialRectTransform;
    [SerializeField] private Vector3 finalPosition;

    public void advanceTurn()
    {
        if(textIndex > 0) { backBtn.SetActive(true); }
        ++maxIndex;
        if (maxIndex == tutorialTexts.Count) { 
            tutorialIsGoing = false; 
            maxIndex = tutorialTexts.Count - 1;

        }
        else
        {
            textIndex = maxIndex;
            tutorialText.text = tutorialTexts[textIndex];
            nextBtn.SetActive(false);
            if (textIndex == maxIndex && maxIndex == tutorialTexts.Count - 1) { closeBtn.SetActive(true); }
            backBtn.SetActive(true);
        }
    }

    public void back()
    {
        --textIndex;
        if(textIndex == 0) { backBtn.SetActive(false); }
        closeBtn.SetActive(false);
        nextBtn.SetActive(true);
        tutorialText.text = tutorialTexts[textIndex];
    }

    public void next()
    {
        ++textIndex;
        if(textIndex == maxIndex && maxIndex == tutorialTexts.Count - 1) { closeBtn.SetActive(true); }
        else if(textIndex == maxIndex) { nextBtn.SetActive(false); }
        backBtn.SetActive(true);
        tutorialText.text = tutorialTexts[textIndex];
    }

    public void endTutorial()
    {
        quixoClass.resetMat(quixoClass.pieceToClick);
        tutorial.SetActive(false);
        CharacterCustomizationUI.IS_AI_GAME = true;
        quixoClass.isTutorial = false;
        quixoClass.AIgame = true;
        quixoClass.tutorialIsOver();
        StartCoroutine(moveCamera());

    }

    public void wasClicked()
    {
        readyForToMove = !readyForToMove;
        if(indexesAdvanceOnCLick.Contains(textIndex))
        {
            advanceTurn();
        }
    }

    private IEnumerator moveCamera()
    {
        Vector3 curr = camera.transform.position;
        float distance = Vector3.Distance(curr, finalPosition);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            camera.transform.position = Vector3.Lerp(curr, finalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position = finalPosition;
    }

    public bool isGoing() 
    { 
        return tutorialIsGoing;
    }

    public bool readyForTo()
    {
        return readyForToMove;
    }
}
