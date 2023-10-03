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

    // 난이도 설정 및 유저 스코어 세팅
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

    // 보드 및 활성 테트리스 조각, 난이도, 유저 스코어 등 초기화
    // 활성 테트리스 조각의 경우 첫 시작 시엔 없으므로 재시작인 경우에만 초기화
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

    // 게임 오버시 보드 및 활성 테트리스 조각 비활성화 및 게임 오버 UI 띄우는 함수
    public void GameOver()
    {
        board.ActivePiece.enabled = false;
        board.enabled = false;
        gameOver.interactable = true;
        this.enabled = false;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        endScoreText.text = $"Time: {timeScore:N1}\nLine Clear: {lineScore}";
    }

    // 라인 클리어 점수 올리고 베스트 라인 클리어 점수 세팅 함수
    public void addLineClearCount()
    {
        lineScore += 1;
        SetBestLineScore();
    }

    // 게임 오버 UI 페이드로 띄울 수 있도록 하는 함수
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

    // 인자로 들어온 시간 점수가 베스트 시간 점수보다 높으면 교체
    private void SetBestTimeScore(float timeScore)
    {
        float bestTimeScore = LoadBestTimeScore();
        if (timeScore > bestTimeScore)
        {
            PlayerPrefs.SetFloat("bestTimeScore", timeScore);
        }
    }

    // 베스트 시간 점수 가져오는 함수
    private float LoadBestTimeScore()
    {
        return PlayerPrefs.GetFloat("bestTimeScore", 0);
    }

    // 인자로 들어온 라인 클리어 점수가 베스트 라인 클리어 점수보다 높으면 교체
    private void SetBestLineScore()
    {
        float bestLineScore = LoadBestLineScore();
        if (lineScore > bestLineScore)
        {
            PlayerPrefs.SetInt("bestLineScore", lineScore);
        }
    }

    // 베스트 라인 클리어 점수 가져오는 함수
    private int LoadBestLineScore()
    {
        return PlayerPrefs.GetInt("bestLineScore", 0);
    }

    // 테트리스 조각 내려오는 속도로 난이도 조절하는 함수
    // 내려오는 속도가 너무 빠르지 않도록 임계치 넘으면 고정
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
