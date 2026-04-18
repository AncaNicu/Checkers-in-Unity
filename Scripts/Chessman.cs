using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;//piesa are o referinta la controller
    public GameObject movePlate;//pt a putea muta o piesa

    //pozitiile
    private int xBoard = -1;
    private int yBoard = -1;

    //var pt a vedea care jucator trebuie sa mute acum (alb sau negru)
    private string player;

    //referinte pt toate sprite-urile care pot fi piese (cal, regina etc)
    public Sprite black_king, black_pawn;
    public Sprite white_king, white_pawn;

    //va fi apelata cand piesa va fi creata
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //pt a aseza piesa pe tabla la un x si la un y date (x si y drept coordonate ale scenei in Unity)
        SetCoords();

        //in fct de numele obiectului, se va alege imaginea sprite flosita pt acea piesa
        switch (this.name)
        {
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;

            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        //acum pozitiile sunt setate in fct de pozitia pe tabla in Unity
        //nu in fct de linie si de coloana pe tabla de joc
        float x = xBoard;
        float y = yBoard;

        //0.66 si -2.3 sunt gasite a.i. piesele sa fie asezate corect pe tabla
        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public string GetPlayer()
    {
        return player;
    }

    private void OnMouseUp()
    {
        Game sc = controller.GetComponent<Game>();
        string result = sc.GetWinner();
        //daca jocul nu s-a terminat
        if(!controller.GetComponent<Game>().IsGameOver())
        {
            //daca piesa pe care s-a apasat este a jucatorului crt
            if(controller.GetComponent<Game>().GetCurrentPlayer() == player)
            {
                //cand nu mai e mouse ul pe piesa, toate MovePlate urile
                //care l aveau ca referinta vor fi distruse
                DestroyMovePlates();

                //pt a instantia MovePlate urile pt noua piesa pe care am dat click
                InitiateMovePlates();
            }
        }
        //jocul s-a terminat
        else
        {
            sc.Winner(result);
        }

    }

    public void DestroyMovePlates()
    {
        //pt a accesa toate MovePlate-urile care exista in acest moment in joc
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        //distruge fiecare moveplate in parte
        for(int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    //pt a construi move plate urile pt piese
    //se tine cont de mutarile pe care le poate face fiecare tip de piesa
    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_king":
            case "white_king":
                KingMovePlate(xBoard, yBoard, false);
                MovePlateSpawn(xBoard, yBoard, false, true);
                break;
            case "black_pawn":
                BackwardsMovePlate(xBoard, yBoard, false);
                MovePlateSpawn(xBoard, yBoard, false, true);
                break;
            case "white_pawn":
                ForwardMovePlate(xBoard, yBoard, false);
                MovePlateSpawn(xBoard, yBoard, false, true);
                break;
            default:   
                break;
        }
    }

    //mutarile pieselor simple albe
    public int ForwardMovePlate(int x, int y, bool check)
    {
        int noOfMoves = 0;
        Game sc = controller.GetComponent<Game>();

        //mutare in fata cu o pozitie -> daca poz e libera si e pe tabla
        if(sc.PositionOnBoard(x - 1, y + 1))
        {
            if(sc.GetPosition(x - 1, y + 1) == null)
            {
                if(!check)
                {
                    MovePlateSpawn(x - 1, y + 1, false, false);
                }
                noOfMoves++;
            }
        }

        if(sc.PositionOnBoard(x + 1, y + 1))
        {
            if(sc.GetPosition(x + 1, y + 1) == null)
            {
                if(!check)
                {
                    MovePlateSpawn(x + 1, y + 1, false, false);
                }
                noOfMoves++;
            }
        }

        //mutare de atac -> daca pe diagonala, la o poz e piesa adversa, iar la 2 poz nu e nimic
        if(sc.PositionOnBoard(x - 1, y + 1) && sc.PositionOnBoard(x - 2, y + 2))
        {
            if(sc.GetPosition(x - 2, y + 2) == null && sc.GetPosition(x - 1, y + 1) != null)
            {
                if(sc.GetPosition(x - 1, y + 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x - 2, y + 2, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        if(sc.PositionOnBoard(x + 1, y + 1) && sc.PositionOnBoard(x + 2, y + 2))
        {
            if(sc.GetPosition(x + 2, y + 2) == null && sc.GetPosition(x + 1, y + 1) != null)
            {
                if(sc.GetPosition(x + 1, y + 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x + 2, y + 2, true, false);
                    }
                    noOfMoves++;
                }
            }
        }

        //mutare dubla de atac -> daca pe diagonala, la o poz si la 3 poz e o piesa adversa si la 2 poz si la 4 poz e gol
        if(sc.PositionOnBoard(x - 1, y + 1) && sc.PositionOnBoard(x - 2, y + 2) && sc.PositionOnBoard(x - 3, y + 3) && sc.PositionOnBoard(x - 4, y + 4))
        {
            if(sc.GetPosition(x - 1, y + 1) != null && sc.GetPosition(x - 2, y + 2) == null && 
                sc.GetPosition(x - 3, y + 3) != null && sc.GetPosition(x - 4, y + 4) == null)
            {
                if(sc.GetPosition(x - 3, y + 3).GetComponent<Chessman>().player != player &&
                    sc.GetPosition(x - 1, y + 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x - 4, y + 4, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        if(sc.PositionOnBoard(x + 1, y + 1) && sc.PositionOnBoard(x + 2, y + 2) && sc.PositionOnBoard(x + 3, y + 3) && sc.PositionOnBoard(x + 4, y + 4))
        {
            if(sc.GetPosition(x + 1, y + 1) != null && sc.GetPosition(x + 2, y + 2) == null && 
                sc.GetPosition(x + 3, y + 3) != null && sc.GetPosition(x + 4, y + 4) == null)
            {
                if(sc.GetPosition(x + 3, y + 3).GetComponent<Chessman>().player != player &&
                    sc.GetPosition(x + 1, y + 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x + 4, y + 4, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        return noOfMoves;
    }

    //mutarile pieselor simple negre
    public int BackwardsMovePlate(int x, int y, bool check)
    {
        int noOfMoves = 0;
        Game sc = controller.GetComponent<Game>();

        //mutare in fata cu o pozitie -> daca poz e libera si e pe tabla
        if(sc.PositionOnBoard(x + 1, y - 1))
        {
            if(sc.GetPosition(x + 1, y - 1) == null)
            {
                if(!check)
                {
                    MovePlateSpawn(x + 1, y - 1, false, false);
                }
                noOfMoves++;
            }
        }

        if(sc.PositionOnBoard(x - 1, y - 1))
        {
            if(sc.GetPosition(x - 1, y - 1) == null)
            {
                if(!check)
                {
                    MovePlateSpawn(x - 1, y - 1, false, false);
                }
                noOfMoves++;
            }
        }

        //mutare de atac -> daca pe diagonala, la o poz e piesa adversa, iar la 2 poz nu e nimic
        if(sc.PositionOnBoard(x + 1, y - 1) && sc.PositionOnBoard(x + 2, y - 2))
        {
            if(sc.GetPosition(x + 2, y - 2) == null && sc.GetPosition(x + 1, y - 1) != null)
            {
                if(sc.GetPosition(x + 1, y - 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x + 2, y - 2, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        if(sc.PositionOnBoard(x - 1, y - 1) && sc.PositionOnBoard(x - 2, y - 2))
        {
            if(sc.GetPosition(x - 2, y - 2) == null && sc.GetPosition(x - 1, y - 1) != null)
            {
                if(sc.GetPosition(x - 1, y - 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x - 2, y - 2, true, false);
                    }
                    noOfMoves++;
                }
            }
        }

        //mutare dubla de atac -> daca pe diagonala, la o poz si la 3 poz e o piesa adversa si la 2 poz si la 4 poz e gol
        if(sc.PositionOnBoard(x + 1, y - 1) && sc.PositionOnBoard(x + 2, y - 2) && sc.PositionOnBoard(x + 3, y - 3) && sc.PositionOnBoard(x + 4, y - 4))
        {
            if(sc.GetPosition(x + 1, y - 1) != null && sc.GetPosition(x + 2, y - 2) == null && 
                sc.GetPosition(x + 3, y - 3) != null && sc.GetPosition(x + 4, y - 4) == null)
            {
                if(sc.GetPosition(x + 3, y - 3).GetComponent<Chessman>().player != player &&
                    sc.GetPosition(x + 1, y - 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x + 4, y - 4, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        if(sc.PositionOnBoard(x - 1, y - 1) && sc.PositionOnBoard(x - 2, y - 2) && sc.PositionOnBoard(x - 3, y - 3) && sc.PositionOnBoard(x - 4, y - 4))
        {
            if(sc.GetPosition(x - 1, y - 1) != null && sc.GetPosition(x - 2, y - 2) == null && 
                sc.GetPosition(x - 3, y - 3) != null && sc.GetPosition(x - 4, y - 4) == null)
            {
                if(sc.GetPosition(x - 3, y - 3).GetComponent<Chessman>().player != player &&
                    sc.GetPosition(x - 1, y - 1).GetComponent<Chessman>().player != player)
                {
                    if(!check)
                    {
                        MovePlateSpawn(x - 4, y - 4, true, false);
                    }
                    noOfMoves++;
                }   
            }
        }

        return noOfMoves;
    }

    //mutarile pieselor incoronate
    public int KingMovePlate(int x, int y, bool check)
    {
        int forwardMoves = ForwardMovePlate(x, y, check);
        int backwardsMoves = BackwardsMovePlate(x, y, check);
        int kingMoves = forwardMoves + backwardsMoves;
        return kingMoves;
    }

    //pt translatarea pe tabla Unity a move plate normala
    public void MovePlateSpawn(int matrixX, int matrixY, bool attack, bool crtPiece)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        //creaza un move plate si il plaseaza pe tabla -> vizibila pe ecran
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = attack;
        mpScript.crtPiece = crtPiece;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
