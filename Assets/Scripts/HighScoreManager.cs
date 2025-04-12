using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    public InputField initialsInput;
    public Text top10Text;
    public GameObject panel;
    public Image inputBorder; // Agora é Image, mais fácil de referenciar no Unity

    private const int MAX_ENTRIES = 10;
    private List<HighScoreEntry> highscores = new List<HighScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScores();
        UpdateDisplay();

    }

    public void TryAddNewScore(int score)
    {
        panel.SetActive(true);

        initialsInput.gameObject.SetActive(true);
        if (inputBorder != null)
            inputBorder.enabled = true;

        initialsInput.text = "";
        initialsInput.characterLimit = 3;
        initialsInput.ActivateInputField();

        initialsInput.onEndEdit.RemoveAllListeners();
        initialsInput.onEndEdit.AddListener((input) =>
        {
            string initials = input.ToUpper().Substring(0, Mathf.Min(3, input.Length));
            AddScore(initials, score);

            initialsInput.gameObject.SetActive(false);
            if (inputBorder != null)
                inputBorder.enabled = false;
        });
    }

    private void AddScore(string initials, int score)
    {
        highscores.Add(new HighScoreEntry(initials, score));
        highscores = highscores.OrderByDescending(x => x.score).Take(MAX_ENTRIES).ToList();
        SaveScores();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        top10Text.text = "<b>🏆 TOP 10</b>\n";
        for (int i = 0; i < highscores.Count; i++)
        {
            top10Text.text += $"{i + 1}. {highscores[i].initials} - {highscores[i].score}\n";
        }
    }

    private void SaveScores()
    {
        for (int i = 0; i < highscores.Count; i++)
        {
            PlayerPrefs.SetString($"HS_Name{i}", highscores[i].initials);
            PlayerPrefs.SetInt($"HS_Score{i}", highscores[i].score);
        }
    }

    private void LoadScores()
    {
        highscores.Clear();
        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            string name = PlayerPrefs.GetString($"HS_Name{i}", "---");
            int score = PlayerPrefs.GetInt($"HS_Score{i}", 0);
            highscores.Add(new HighScoreEntry(name, score));
        }
    }
}

[System.Serializable]
public class HighScoreEntry
{
    public string initials;
    public int score;

    public HighScoreEntry(string initials, int score)
    {
        this.initials = initials;
        this.score = score;
    }
}
