using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

// source for clicking ability: https://www.youtube.com/watch?v=kkkmX3_fvfQ&ab_channel=Andrew


public class Penguin : MonoBehaviour, IPointerClickHandler
{
    public GameObject penguin;
    public QuixoClass Game;
    public int row = 0;
    public int col = 0;
    public char face = '_';
    public Material xmat; // the color of a penguin owned by the x player
    public Material omat; // the color of a penguin owned by the y player
    private Point _toPoint;
    public Point toPoint
    {
        get { return _toPoint; }
        set
        {
            _toPoint = value;
            _toPos = Game.getPos(_toPoint); // Ensure _toPos is updated whenever toPoint changes
        }
    }
    private Vector3 _toPos;
    public Vector3 toPos
    {
        get { return _toPos; } 
    }

    public void resetTarget()
    {
        toPoint = new Point(0, 0); 
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game.canPickPiece(row, col))
        {
            //UnityEngine.Debug.Log($"Selected block ({row},{col})");
            if (!Game.moveInProgress)
            {
                //penguin.SetActive(false);
                Game.moveInProgress = true;
                Game.from = loc();
                Game.poss = Game.GetPossibleMoves();
            }
            else
            {
                Game.to = loc();
                if (Game.IsValidMove())
                {
                    Game.makeMove();
                }
            }
        }
        
    }

    public Point loc() { return new Point(row, col); }

    public void Face(char f)
    {
        if (f == '_') return; // Do nothing if the face character is '_'
        //penguin.GetComponent<MeshRenderer>().material = f == 'X' ? xmat : omat;
        face = f;
        
    }

    public void snap()
    {
        penguin.transform.position = toPos;
        row = _toPoint.row;
        col = _toPoint.col;
        resetTarget();
    }
    public void step(float spd)
    {
        penguin.transform.position = Vector3.MoveTowards(penguin.transform.position, toPos, spd * Time.deltaTime);
    }
    public void step(float spd, Point tp)
    {
        Vector3 target = Game.getPos(tp);
        penguin.transform.position = Vector3.MoveTowards(penguin.transform.position, target, spd * Time.deltaTime);
    }
    public void step(float spd, Vector3 target)
    {
        penguin.transform.position = Vector3.MoveTowards(penguin.transform.position, target, spd * Time.deltaTime);
    }

    public float dist()
    {
        return Vector3.Distance(penguin.transform.position, toPos);
    }
    public void Play(string animation){
        Animator animator = penguin.GetComponent<Animator>();
        if(animator.HasState(0, Animator.StringToHash(animation))){
            animator.Play(animation);
            
        }
    }
}