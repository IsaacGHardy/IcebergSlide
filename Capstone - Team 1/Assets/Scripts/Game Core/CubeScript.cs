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
        penguin.run();
    }

    public Point loc() { return new Point(row, col); }
}
