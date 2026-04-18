using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    //pt a putea accesa orice piesa dorim
    //e de 8x8 pt ca tabla are 8 linii si 8 coloane
    private GameObject[,] positions = new GameObject[8, 8];

    //vect cu piese pt jucatori, cate 12 piese pt fiecare
    private GameObject[] playerBlack = new GameObject[12];
    private GameObject[] playerWhite = new GameObject[12];

    //pt a vedea randul carui jucator este 
    //inti e negru, pt ca negrul incepe
    private string currentPlayer = "white";

    private bool gameOver = false;

    void Start()
    {
        //cream o noua instanta de piesa si o asezam pe tabla
        //quatrernion e pt rotatie, iar vector3 pt pozitie
        //Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);

        //umplem vectorii de piese
        playerWhite = new GameObject[]
        {
            Create("white_pawn",0,0), Create("white_pawn",2,0),
            Create("white_pawn",4,0), Create("white_pawn",6,0),
            Create("white_pawn",1,1), Create("white_pawn",3,1),
            Create("white_pawn",5,1), Create("white_pawn",7,1),
            Create("white_pawn",0,2), Create("white_pawn",2,2),
            Create("white_pawn",4,2), Create("white_pawn",6,2)
        };

        playerBlack = new GameObject[]
        {
            Create("black_pawn",1,7), Create("black_pawn",3,7),
            Create("black_pawn",5,7), Create("black_pawn",7,7),
            Create("black_pawn",0,6), Create("black_pawn",2,6),
            Create("black_pawn",4,6), Create("black_pawn",6,6),
            Create("black_pawn",1,5), Create("black_pawn",3,5),
            Create("black_pawn",5,5), Create("black_pawn",7,5)
        };

        //punem toate piesele pe tabla
        for(int i = 0; i < playerWhite.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        //cream o piesa generala si ii setam pozitia
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    //pt a putea muta o piesa individuala
    //trebuie ca pozitia pe care a fost sa devina goala
    public void SetPositionEmpty(int x, int y)
    {
        //adica setam pozitia unde a fost ca fiind null
        positions[x, y] = null;
    }

    //pt a returna piesa de la o anumita pozitie
    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    //pt a vedea daca o anumita poz (x,y) este sau nu pe tabla de joc
    public bool PositionOnBoard(int x, int y)
    {
        if(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1))
        {
            return false;
        }
        return true;
    }

    //pt a vedea care e jucatorul crt (alb sau negru)
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    //pt a vedea daca jocul s-a terminat sau nu
    public bool IsGameOver()
    {
        return gameOver;
    }

    //pt a seta randul urmatorului jucator
    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        //daca jocul s-a terminat, iar jucatorul da click
        if(gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //resetam jocul
            SceneManager.LoadScene("Game");
        }
    }

    public void Winner(string playerWinner)
    {
        string message;
        gameOver = true;
        if(playerWinner == "tie")
        {
            message = "Tie";
        }
        else
        {
            message = playerWinner + " is the winner";
        }
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = message;

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    //pt a vedea daca jucatorul player nu mai are piese pe tabla
    public string GetWinner()
    {
        int i, j;
        Game sc = this.GetComponent<Game>();
        int noOfWhitePiece = 0;
        int noOfBlackPieces = 0;
        string message = "no winner";
        int totalWhiteMoves = 0;
        int totalBlackMoves = 0;
        int totalMoves = 0;
        for(i = 0; i < 8; i++)
        {
            for(j = 0; j < 8; j++)
            {
                GameObject piece = sc.GetPosition(i, j);
                if(piece != null)
                {
                    Chessman cm = positions[i, j].GetComponent<Chessman>();
                    switch(cm.name)
                    {
                        case "black_king":
                            totalBlackMoves += cm.KingMovePlate(i, j, true);
                            noOfBlackPieces++;
                            break;                            
                        case "white_king":
                            totalWhiteMoves += cm.KingMovePlate(i, j, true);
                            noOfWhitePiece++;
                            break;
                        case "black_pawn":
                            totalBlackMoves += cm.BackwardsMovePlate(i, j, true);
                            noOfBlackPieces++;
                            break;
                        case "white_pawn":
                            totalWhiteMoves += cm.ForwardMovePlate(i, j, true);
                            noOfWhitePiece++;
                            break;
                        default:   
                            break;
                    }
                }
            }
        }

        //negrul castiga daca nu mai e nicio piesa alba pe tabla
        //sau daca albul nu se mai poate muta
        if(noOfWhitePiece == 0 || totalWhiteMoves == 0)
        {
            message = "black";
            gameOver = true;
        }

        //albul castiga daca nu mai e nicio piesa neagra pe tabla
        //sau daca negrul nu se mai poate muta
        if(noOfBlackPieces == 0 || totalBlackMoves == 0)
        {
            message = "white";
            gameOver = true;
        }

        //este remiza daca nicio piesa nu se mai poate muta
        totalMoves = totalBlackMoves + totalWhiteMoves;
        if(totalMoves == 0)
        {
            message = "tie";
            gameOver = true;
        }
        return message;
    }
}
