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
        "You will always begin your turn by selecting a penguin on the outside of the board\n\n\n" +
        "Try selecting the green penguin to begin your turn",
        //1
        "Now slide your selected penguin by clicking on the blue penguin\n\n" +
        "The penguins will slide to fill the empty spot\n\n" +
        "Slide that penguin like a pro!",
        //2
        "It's time for the rival penguins to make their move!",
        //3
        "Once again, choose the green penguin to begin your turn\n\n" +
        "You can't slide penguins from the opposing team!" +
        //4
        "Select the blue penguin to complete your turn\n\n" +
        "You must always slide your penguin to the end of the same row or column\n\n" +
        "Your options will bounce to let you know where you can slide your penguin",
        //5
        "It's time for the rival penguins to make their move!",
        //6
        "To win, aim to line up 5 of your penguins in a row, column, or diagonal\n\n" +
        "It's like Tic Tac Toe but on ice!",
        //7
        "You can challenge the AI with different difficulty levels\n\n" +
        "Fancy a real challenge? Try battling the Emperor of the Ice.\n\n" +
        "Looking for a gentler opponent? Meet Eggling!",
        //8
        "Prefer to face off against a friend? We've got you covered!\n\n" +
        "Iceberg Slide supports both online and local multiplayer matches.",
        //9
        "Feeling puzzled? Check out the pause menu for handy instructions and tips.",
        //10
        "Don't feel like wearing astronaut gear? Choose a new hat in the character selector.\n\n" +
        "You can even come up with a cool nickname for your team!",
        //11 
        "Player 1 always takes the first slide.\n\n" +
        "Want to change things up? Select 'Swap Sides' in the character selector.",
        //12
        "If the game seems to be stuck, and victory is slipping away, propose a draw.\n\n" +
        "But there's no guarantee your opponent will accept!",
        //13
        "You can only offer a draw when it is your turn",
        //14
        "Think you've mastered the ice? Close this window and glide to victory on your own!\n\n" +
        "Still need assistance? Keep this window open, and we'll keep guiding you."
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
