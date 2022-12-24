using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int score = 0;
    public int wins = 0;
    public int ties = 0;
    public int losses = 0;
    [SerializeField] bool firstTurn = true;
    [SerializeField] bool autoPlayGame = false;
    CanvasManager canvas;
    
    [SerializeField] PlayerTurn playerTurn = PlayerTurn.Null;
    enum PlayerTurn
    {
        Player,
        AI,
        Null,
        AutoPlay
    }

    struct Move
    {
        public Move(int r, int c)
        {
            row = r;
            col = c;
        }
        public int row, col;
    };

    BoardPosition[,] board = new BoardPosition[3, 3];

    bool IsMovesLeft()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j].status == BoardPosition.BoardStatus.Empty)
                    return true;
        return false;
    }

    int Evaluate()
    {
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0].status == board[row, 1].status && board[row, 1].status == board[row, 2].status)
            {
                if (board[row, 0].status == BoardPosition.BoardStatus.X)
                    return +10;
                else if (board[row, 0].status == BoardPosition.BoardStatus.O)
                    return -10;
            }
        }

        for (int col = 0; col < 3; col++)
        {
            if (board[0, col].status == board[1, col].status && board[1, col].status == board[2, col].status)
            {
                if (board[0, col].status == BoardPosition.BoardStatus.X)
                    return +10;

                else if (board[0, col].status == BoardPosition.BoardStatus.O)
                    return -10;
            }
        }

        if (board[0, 0].status == board[1, 1].status && board[1, 1].status == board[2, 2].status)
        {
            if (board[0, 0].status == BoardPosition.BoardStatus.X)
                return +10;
            else if (board[0, 0].status == BoardPosition.BoardStatus.O)
                return -10;
        }

        if (board[0, 2].status == board[1, 1].status && board[1, 1].status == board[2, 0].status)
        {
            if (board[0, 2].status == BoardPosition.BoardStatus.X)
                return +10;
            else if (board[0, 2].status == BoardPosition.BoardStatus.O)
                return -10;
        }

        return 0;
    }

    float Minimax(float depth, bool isMax)
    {
        int score = Evaluate();

        if (score == 10)
            if (playerTurn == PlayerTurn.AI)
                return score;
            else return -score;

        if (score == -10)
            if (playerTurn == PlayerTurn.AI)
                return score;
            else return -score;

        if (IsMovesLeft() == false)
            return 0;

        if (isMax)
        {
            float best = -1000f;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].status == BoardPosition.BoardStatus.Empty)
                    {
                        if (playerTurn == PlayerTurn.AI)
                            board[i, j].status = BoardPosition.BoardStatus.X;
                        else
                            board[i, j].status = BoardPosition.BoardStatus.O;

                        best = Mathf.Max(best, Minimax(depth + 1, !isMax));

                        board[i, j].status = BoardPosition.BoardStatus.Empty;
                    }
                }
            }
            return best / depth;
        }
        else
        {
            float best = 1000f;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].status == BoardPosition.BoardStatus.Empty)
                    {
                        if (playerTurn == PlayerTurn.AI)
                            board[i, j].status = BoardPosition.BoardStatus.O;
                        else
                            board[i, j].status = BoardPosition.BoardStatus.X;

                        best = Mathf.Min(best, Minimax(depth + 1, !isMax));

                        board[i, j].status = BoardPosition.BoardStatus.Empty;
                    }
                }
            }
            return best / depth;
        }
    }

    Move FindBestMove()
    {
        if (firstTurn)
        {
            firstTurn = false;
            return new(Random.Range(0, 3), Random.Range(0, 3));
        }

        float bestVal = -1000;
        Move bestMove = new(-1, -1);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j].status == BoardPosition.BoardStatus.Empty)
                {
                    if (playerTurn == PlayerTurn.AI)
                        board[i, j].status = BoardPosition.BoardStatus.X;
                    else
                        board[i, j].status = BoardPosition.BoardStatus.O;

                    float moveVal = Minimax(1, false);

                    board[i, j].status = BoardPosition.BoardStatus.Empty;

                    if (moveVal > bestVal)
                    {
                        //Debug.Log(moveVal);
                        bestMove = new(i, j);
                        bestVal = moveVal;
                    }
                }
            }
        }
        
        //Debug.Log(bestMove.row + ", " + bestMove.col);
        return bestMove;
    }

    void Start()
    {
        Screen.SetResolution(1080, 1080, false);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = GameObject.Find("Board(" + i.ToString() + "/" + j.ToString() + ")").GetComponent<BoardPosition>();
                //Debug.Log(board[i, j]);
            }
        }

        canvas = GameObject.Find("Canvas").GetComponent<CanvasManager>();
    }

    void Update()
    {
        if (playerTurn == PlayerTurn.AI)
        {
            Move move = FindBestMove();
            board[move.row, move.col].status = BoardPosition.BoardStatus.X;
            board[move.row, move.col].OnStatusChange();

            int e = Evaluate();
            if (e == 0 && IsMovesLeft())
                if (!autoPlayGame)
                    playerTurn = PlayerTurn.Player;
                else
                    StartCoroutine(TurnDelay(PlayerTurn.AutoPlay));
            else
            {
                playerTurn = PlayerTurn.Null;
                WinCon(e);
            }
        }
        else if (playerTurn == PlayerTurn.AutoPlay)
        {
            Move move = FindBestMove();
            board[move.row, move.col].status = BoardPosition.BoardStatus.O;
            board[move.row, move.col].OnStatusChange();

            int e = Evaluate();
            if (e == 0 && IsMovesLeft())
                StartCoroutine(TurnDelay(PlayerTurn.AI));
            else
            {
                playerTurn = PlayerTurn.Null;
                WinCon(e);
            }
        }
    }

    void WinCon(int eval)
    {
        score += eval;
        canvas.menu.SetActive(true);
        if (!canvas.tieLossText.gameObject.activeSelf)
            canvas.tieLossText.gameObject.SetActive(true);
        autoPlayGame = false;
        switch (eval)
        {
            case -10:
                wins++;
                canvas.tieLossText.text = "You Won! How!?";
                canvas.winsCount.text = "Wins: " + wins;
                break;
            case 10:
                losses++;
                canvas.tieLossText.text = "You Lost! :(";
                canvas.lossCount.text = "Losses: " + losses;
                break;
            case 0:
                ties++;
                canvas.tieLossText.text = "You Tied";
                canvas.tiesCount.text = "Ties: " + ties;
                break;

        }
    }

    public void PlayTurn(int i, int j)
    {
        if (board[i, j].status == BoardPosition.BoardStatus.Empty && playerTurn == PlayerTurn.Player)
        {
            board[i, j].status = BoardPosition.BoardStatus.O;
            board[i, j].OnStatusChange();

            int e = Evaluate();
            if (e == 0 && IsMovesLeft())
                StartCoroutine(TurnDelay(PlayerTurn.AI));
            else
            {
                playerTurn = PlayerTurn.Null;
                WinCon(e);
            }
        }
    }

    public void RandomStart(bool autoPlay)
    {
        ResetBoard();
        int p = Random.Range(0, 2);
        if (p == 0)
        {
            if (autoPlay)
            {
                autoPlayGame = true;
                StartCoroutine(TurnDelay(PlayerTurn.AutoPlay));
            }
            else
            {
                playerTurn = PlayerTurn.Player;
                firstTurn = false;
            }
        }
        else if (p == 1)
        {
            if (autoPlay)
                autoPlayGame = true;
        }

        StartCoroutine(ShowStartText(p));
    }

    void ResetBoard()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j].status = BoardPosition.BoardStatus.Empty;
                board[i, j].OnStatusChange();
            }
        }
        firstTurn = true;
    }

    IEnumerator ShowStartText(int p)
    {
        canvas.startText.gameObject.SetActive(true);
        if (p == 0)
            canvas.startText.text = "You Start!";
        else if (p == 1)
            canvas.startText.text = "Computer Starts";

        yield return new WaitForSeconds(1.5f);

        if (p == 1)
            playerTurn = PlayerTurn.AI;

        canvas.startText.gameObject.SetActive(false);
    }

    IEnumerator TurnDelay(PlayerTurn play)
    {
        playerTurn = PlayerTurn.Null;

        yield return new WaitForSeconds(1.5f);

        playerTurn = play;
    }
}
