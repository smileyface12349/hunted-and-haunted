using System.Collections;
using System.Collections.Generic;
using Effects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public HealthBar healthBar;
    public TextMeshProUGUI topTextTitle;
    public TextMeshProUGUI topTextDescription;
    public TextMeshProUGUI effectsText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverWindow;
    public bool isPaused;
    
    private float topTextExpiry = 0;
    private ActiveEffect[] effects = new ActiveEffect[0];
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        gameOverWindow.SetActive(false);
        UpdateHealth(PlayerController.MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) return;
        
        if (topTextExpiry != 0 && Time.time > topTextExpiry) {
            topTextTitle.text = "";
            topTextDescription.text = "";
            topTextExpiry = 0;
        }
        
        UpdateEffects();
        
        elapsedTime += Time.deltaTime;
        int minutes = elapsedTime < 60 ? 0 : (int) (elapsedTime / 60);
        timerText.text = minutes + ":" + (elapsedTime % 60).ToString("F2");
    }
    
    public void GameOver() {
        isPaused = true;
        gameOverWindow.SetActive(true);
    }
    
    public void Quit() {
        Application.Quit();
    }
    
    public void Restart() {
        SceneManager.LoadScene("SampleScene");
    }
    
    public void UpdateHealth(float health) {
        healthBar.Set(health);
    }
    
    public void SetTopText(string title, string description, float duration) {
        topTextTitle.text = title;
        topTextDescription.text = description;
        topTextExpiry = Time.time + duration;
    }
    
    public void SetEffects(ActiveEffect[] newEffects) {
        effects = newEffects;
    }
    
    private void UpdateEffects() {
        string text = "";
        foreach (ActiveEffect effect in effects) {
            if(effect == null) {
                break;
            }
            text += effect.ToString() + "\n";
        }
        effectsText.text = text;
    }
    

}