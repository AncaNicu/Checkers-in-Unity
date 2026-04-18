using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    //facem o referinta la controller
    public GameObject controller;

    //de fiecare data cand se apasa pe o piesa, e va crea MovePlate
    //iar acea Move Plate tre sa aiba o referinta la piesa care a creat-o
    GameObject reference = null;

    //poz pe tabla
    int matrixX;
    int matrixY;

    //pt a vedea daca pe poz pe care vrea sa se mute piesa
    //este o alta piesa adversa, deci aceea ar fi o mutare de atac
    //false = mutare; true = atac
    public bool attack = false;

    //pt a pune move plate pe piesa pe care am apasat
    public bool crtPiece = false;

    public void Start()
    {
        if(attack)
        {
            //schimbam culoarea patratelului gol (MovePlate) in rosu
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            if(crtPiece)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.92f, 0.016f, 1.0f);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    //echivalenta cu a pune mouse ul pe un MovePlate
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //ia pozitia la care era piesa referinta
        int xPrev = reference.GetComponent<Chessman>().GetXBoard();
        int yPrev = reference.GetComponent<Chessman>().GetYBoard();

        //arata daca prin mutarea crt am obtinut un rege
        bool krownedWhitePiece = false;
        bool krownedBlackPiece = false;

        //daca piesa de mutat este un pion si a ajuns la marginea opusa => il incoronam
        GameObject p = controller.GetComponent<Game>().GetPosition(xPrev, yPrev);
        if(p.name == "white_pawn" && matrixY == 7)
        {
            krownedWhitePiece = true;
        }

        if(p.name == "black_pawn" && matrixY == 0)
        {
            krownedBlackPiece = true;
        }

        //daca e mutare de atac, tre sa scapam de piesa care era acolo inainte
        if(attack)
        {
            ExecuteSimpleAttackMove(xPrev, yPrev);
            ExecuteDoubleAttackMove(xPrev, yPrev);
        }

        if(!crtPiece)
        {
            //setam casuta unde a fost initial piesa crt ca fiind goala
            //deoarece am mutat o deja
            controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
                reference.GetComponent<Chessman>().GetYBoard());

            //setam noua pozitie a piesei ca fiind pozitia la care am mutat o
            reference.GetComponent<Chessman>().SetXBoard(matrixX);
            reference.GetComponent<Chessman>().SetYBoard(matrixY);

            //setam si noua poz pe tabla Unity -> translatam piesa
            reference.GetComponent<Chessman>().SetCoords();

            //pt a tine cont de poz crt a piesei, tre sa instiintam controllerul
            controller.GetComponent<Game>().SetPosition(reference);

            //daca avem pion incoronat
            if(krownedWhitePiece || krownedBlackPiece)
            {
                //ia piesa deja mutata, o distruge
                GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
                Destroy(cp);
                //setam poz ca fiind goala
                controller.GetComponent<Game>().SetPositionEmpty(matrixX, matrixY);
                //pune pe noua pozitie un rege de culoarea potrivita
                if (krownedWhitePiece)
                {
                    GameObject obj = controller.GetComponent<Game>().Create("white_king", matrixX, matrixY);
                    controller.GetComponent<Game>().SetPosition(obj);
                }
                else
                {
                    GameObject obj = controller.GetComponent<Game>().Create("black_king", matrixX, matrixY);
                    controller.GetComponent<Game>().SetPosition(obj);
                }
            }

            //pt a trece la urmatorul jucator
            controller.GetComponent<Game>().NextTurn();
        }

        //pt a distruge MovePlate-ul
        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    //pt a executa o mutare simpla de atac
    //=> distruge piesa adeversa
    public void ExecuteSimpleAttackMove(int xPrev, int yPrev)
    {
        //poz la care se afla piesa atacata
        int xAttacked;
        int yAttacked;
        //mutare de atac simpla
        if(matrixX == xPrev + 2 && matrixY == yPrev - 2)
        {
            xAttacked = xPrev + 1;
            yAttacked = yPrev - 1;

            DestroyPiece(xAttacked, yAttacked);
        }

        if(matrixX == xPrev - 2 && matrixY == yPrev + 2)
        {
            xAttacked = xPrev - 1;
            yAttacked = yPrev + 1;

            DestroyPiece(xAttacked, yAttacked);
        }

        if(matrixX == xPrev - 2 && matrixY == yPrev - 2)
        {
            xAttacked = xPrev - 1;
            yAttacked = yPrev - 1;

            DestroyPiece(xAttacked, yAttacked);
        }

        if(matrixX == xPrev + 2 && matrixY == yPrev + 2)
        {
            xAttacked = xPrev + 1;
            yAttacked = yPrev + 1;

            DestroyPiece(xAttacked, yAttacked);
        }
    }

    //pt a executa o mutare de atac dubla
    //distruge piesele adverse
    public void ExecuteDoubleAttackMove(int xPrev, int yPrev)
    {
        //poz la care se afla piesele de capturat la mutarea de atac dublu
        int xAttacked1, yAttacked1, xAttacked2, yAttacked2;
        //mutare dubla de atac
        if(matrixX == xPrev - 4 && matrixY == yPrev + 4)
        {
            xAttacked1 = xPrev - 3;
            yAttacked1 = yPrev + 3;

            xAttacked2 = xPrev - 1;
            yAttacked2 = yPrev + 1;

            DestroyPiece(xAttacked1, yAttacked1);
            DestroyPiece(xAttacked2, yAttacked2);
        }

        if(matrixX == xPrev + 4 && matrixY == yPrev + 4)
        {
            xAttacked1 = xPrev + 3;
            yAttacked1 = yPrev + 3;

            xAttacked2 = xPrev + 1;
            yAttacked2 = yPrev + 1;

            DestroyPiece(xAttacked1, yAttacked1);
            DestroyPiece(xAttacked2, yAttacked2);
        }

        if(matrixX == xPrev + 4 && matrixY == yPrev - 4)
        {
            xAttacked1 = xPrev + 3;
            yAttacked1 = yPrev - 3;

            xAttacked2 = xPrev + 1;
            yAttacked2 = yPrev - 1;

            DestroyPiece(xAttacked1, yAttacked1);
            DestroyPiece(xAttacked2, yAttacked2);
        }

        if(matrixX == xPrev - 4 && matrixY == yPrev - 4)
        {
            xAttacked1 = xPrev - 3;
            yAttacked1 = yPrev - 3;

            xAttacked2 = xPrev - 1;
            yAttacked2 = yPrev - 1;

            DestroyPiece(xAttacked1, yAttacked1);
            DestroyPiece(xAttacked2, yAttacked2);
        }
    }

    //pt a sterge o piesa
    public void DestroyPiece(int x, int y)
    {
        GameObject cp = controller.GetComponent<Game>().GetPosition(x, y);
        //distruge acea piesa
        Destroy(cp);
    }

    //pt a seta coord lui MovePlate
    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    //pt a seta un anumit ob (piesa) ca referinta
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    //pt ca celelalte ob sa poata accesa referinta MovePlate-ului
    public GameObject GetReference()
    {
        return reference;
    }
}
