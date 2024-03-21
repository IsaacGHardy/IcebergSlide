using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

// source for clicking ability: https://www.youtube.com/watch?v=kkkmX3_fvfQ&ab_channel=Andrew


public class Penguin : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject penguin;
    public GameObject head;
    public GameObject hair;
    public QuixoClass Game;
    public int row = 0;
    public int col = 0;
    public char face;
    public char oldFace;
    private Point _toPoint;
    public static Point clickedPenguin = new Point(-1, -1);
    [SerializeField] public SoundEffect soundEffect;
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

    public Hat hat;
    public void resetTarget()
    {
        toPoint = new Point(0, 0); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Game.isLocked && Game.canPickPiece(row, col))
        {
            soundEffect.playPoke();
            run();
        }
    }

    public void run(bool ai = false)
    {
        if (Game.canPickPiece(row, col))
        {
            if (!Game.moveInProgress)
            {
                Game.moveInProgress = true;
                Game.from = loc();
                Game.poss = Game.GetPossibleMoves();
                face = face == '_' ? Game.isXTurn ? 'X' : 'O' : face;
                setHat();
                Play("Bounce");
                clickedPenguin = new Point(this.row, this.col);
            }
            else if (Game.from.eq(loc()))
            {
                Game.from = new Point(0, 0);
                Game.moveInProgress = false;
                face = oldFace;
                setHat();
                soundEffect.stopAllsounds();
                soundEffect.playPoke();
                Play("Idle_A");
                clickedPenguin = new Point(-1, -1);
            }
            else
            {
                Game.to = loc();
                clickedPenguin = new Point(-1, -1);
                if (Game.IsValidMove())
                {
                    soundEffect.stopAllsounds();
                    soundEffect.playPoke();
                    Game.isLocked = true;
                    Game.Data(Game.from).oldFace = Game.Data(Game.from).face;
                    Game.passMove(ai);
                }
            }
        }
    }

    public void setHat(){
        // remove hat if penguin has been un-selected before its first time being moved
        if (face == '_')
        {
            if (hat != null)
            {
                hat.remove();
                hat = null;
            }
            return;
        }
        GameObject newHat;
        // if penguin has a hat already, leave it alone, otherwise, give it a hat that
        // matches the curent player's turn
        if (Game.isXTurn && hat == null && Game.xhat != null){
            newHat = Instantiate(Game.xhat, Game.getPos(loc()), Quaternion.identity);
            hat = newHat.GetComponent<Hat>();
            hat.Setup(this, head);
        }
        else if (!Game.isXTurn && hat == null && Game.ohat != null){
            newHat = Instantiate(Game.ohat, Game.getPos(loc()), Quaternion.identity);
            hat = newHat.GetComponent<Hat>();
            hat.Setup(this, head);
        }
    }

    public void onlineSetHat()
    {
        GameObject newHat;
        // if penguin has a hat already, leave it alone, otherwise, give it a hat that
        // matches the curent player's turn
        if (Game.isXTurn && hat == null && Game.xhat != null)
        {
            newHat = Instantiate(Game.xhat, Game.getPos(loc()), Quaternion.identity);
            hat = newHat.GetComponent<Hat>();
            hat.Setup(this, head);
        }
        else if (!Game.isXTurn && hat == null && Game.ohat != null)
        {
            newHat = Instantiate(Game.ohat, Game.getPos(loc()), Quaternion.identity);
            hat = newHat.GetComponent<Hat>();
            hat.Setup(this, head);
        }
    }

    public void setHat(Hat hat)
    {
        Hat newHat;
        newHat = Instantiate(hat, this.transform.position, Quaternion.identity);
        hat = newHat.GetComponent<Hat>();
        hat.Setup(this, head);
    }
    public Point loc() { return new Point(row, col); }

    public void Face(char f)
    {
        if (f == '_') return; // Do nothing if the face character is '_'
        
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Game.gameOver && !Game.isLocked)
        {
            if (Game.canPickPiece(row, col))
            {
                if (Game.moveInProgress)
                {
                    if ((this.row == clickedPenguin.row && (this.col == 0 || this.col == 4))
                        || (this.col == clickedPenguin.col && (this.row == 0 || this.row == 4)))
                    {
                        Play("Bounce");
                        soundEffect.playBloop3();
                    }
                }
                else
                {
                    Play("Bounce");
                    soundEffect.playBloop3();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Game.gameOver && !Game.isLocked)
        {
            if (this.row == Penguin.clickedPenguin.row && this.col == Penguin.clickedPenguin.col)
            {
                Play("Bounce");
                soundEffect.stopBloop3();
            }
            else
            {
                Play("Idle_A");
                soundEffect.stopAllsounds();
            }
        }
    }
}