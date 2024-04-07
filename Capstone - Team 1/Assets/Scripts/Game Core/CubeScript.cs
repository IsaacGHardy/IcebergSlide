using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeScript : MonoBehaviour, IPointerClickHandler
{
    public int row, col;
    public QuixoClass Game;
    public Penguin penguin
    {
        get { return Game.gameBoard[row, col]; }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game.isTutorial)
        {
            if (Game.pieceToClick.eq(loc()))
            {
                penguin.soundEffect.playPoke();
                penguin.run();
                Game.tutorialPieceClick();
            }
        }
        else if (!Game.isLocked && Game.canPickPiece(row, col))
        {
            penguin.soundEffect.playPoke();
            penguin.run();
        }
    }

    public Point loc() { return new Point(row, col); }
}
