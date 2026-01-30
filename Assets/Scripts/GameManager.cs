using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private int count;
    private int totalPickups;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optional: keep across scene reloads
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (countText == null)
            countText = GameObject.Find("CountText")?.GetComponent<TextMeshProUGUI>();

        if (winTextObject == null)
            winTextObject = GameObject.Find("WinText");

        if (countText == null || winTextObject == null)
        {
            Debug.LogError("UI not found. Need CountText and WinText in the scene.");
            enabled = false;
            return;
        }

        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;

        count = 0;
        winTextObject.SetActive(false);
        UpdateUI();
    }

    public void AddPickup()
    {
        count++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        countText.text = $"Count: {count}/{totalPickups}";

        if (totalPickups > 0 && count >= totalPickups)
        {
            winTextObject.SetActive(true);

            var tmp = winTextObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "You Win!";

            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null) Destroy(enemy);
        }
    }
}
