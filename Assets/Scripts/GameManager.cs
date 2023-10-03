using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI timeScoreText;
    public TextMeshProUGUI timeBestText;
    public TextMeshProUGUI lineScoreText;
    public TextMeshProUGUI lineBestText;
    public TextMeshProUGUI endScoreText;
    private float timeScore;
    private int lineScore;
    private float difficultySetTime;

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        timeScore += Time.deltaTime;
        SetBestTimeScore(timeScore);
        SetDifficulty();
        timeScoreText.text = $"{timeScore:N1}";
        timeBestText.text = $"{LoadBestTimeScore():N1}";
        lineScoreText.text = $"{lineScore}";
        lineBestText.text = $"{LoadBestLineScore()}";
    }

    public void NewGame()
    {
        this.enabled = true;
        board.enabled = true;
        board.ActivePiece.enabled = true;
        board.ActivePiece.stepDelay = 1f;

        difficultySetTime = 0f;
        timeScore = 0f;
        lineScore = 0;
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        board.Tilemap.ClearAllTiles();
        board.ActivePiece.stepDelay = 1f;

        if (board.ActivePiece.Cells != null)
        {
            board.Clear(board.ActivePiece);
            board.ActivePiece.HardDrop();
            board.Tilemap.ClearAllTiles();
        }
    }

    public void GameOver()
    {
        board.ActivePiece.enabled = false;
        board.enabled = false;
        gameOver.interactable = true;
        this.enabled = false;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        endScoreText.text = $"Time: {timeScore:N1}\nLine Clear: {lineScore}";
    }

    public void addLineClearCount()
    {
        lineScore += 1;
        SetBestLineScore();
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private void SetBestTimeScore(float timeScore)
    {
        float bestTimeScore = LoadBestTimeScore();
        if (timeScore > bestTimeScore)
        {
            PlayerPrefs.SetFloat("bestTimeScore", timeScore);
        }
    }

    private float LoadBestTimeScore()
    {
        return PlayerPrefs.GetFloat("bestTimeScore", 0);
    }

    private void SetBestLineScore()
    {
        float bestLineScore = LoadBestLineScore();
        if (lineScore > bestLineScore)
        {
            PlayerPrefs.SetInt("bestLineScore", lineScore);
        }
    }

    private int LoadBestLineScore()
    {
        return PlayerPrefs.GetInt("bestLineScore", 0);
    }

    private void SetDifficulty()
    {
        if (timeScore - difficultySetTime > 7 && board.ActivePiece.stepDelay > 0.01f)
        {
            float newStepDelay = board.ActivePiece.stepDelay - 0.1f;
            if (newStepDelay >= 0.01f)
            {
                board.ActivePiece.stepDelay = newStepDelay;
            }
            difficultySetTime = timeScore;
        }
    }
}
