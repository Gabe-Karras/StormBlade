using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

// This class represents a component of a boss
// Active components have their own health or can be a direct pipe to the boss' health
// If a component is not active, the health is not touched
public class BossComponent : MonoBehaviour
{
    // The boss this component is attached to
    [SerializeField]
    private Boss boss;

    // Health of this component (disregard if component will not be active)
    [SerializeField]
    private int hp = 1;

    // Whether the health is shared by the whole boss or not
    [SerializeField]
    private bool hasTotalBossHealth;

    // Level of defense this boss has (What to divide incoming attacks by)
    [SerializeField]
    private float defense = 1;

    // The text that will pop up when this component is selected
    [SerializeField]
    private string componentName;

    // The type of explosion this component will have when it is destroyed
    [SerializeField]
    private GameObject explosion;

    // How long it takes the standard component to destroy
    private float deathTime = 1.5f;

    private bool iframes = false;
    private float iframeSeconds = 0.06f;

    // Game manager
    private GameManager gameManager;

    // Reference to ui manager for text animation
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get managers
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiManager = gameManager.GetUIManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update hp up or down
    // Plays text animation showing the value
    public void UpdateHp(int change) {
        // Text color in animation
        Color textColor = new Color(1, 1, 1);

        // If change is negative, apply defense
        if (change < 0) {
            change = (int) (change / defense);
        }
        // If change is positive, make it a health color
        else {
            // Same color as player healthbar and particle animation
            textColor = new Color(0.93f, 0.38f, 0.36f);
        }

        // Play animation
        float textX = transform.position.x * GameSystem.CANVAS_RATIO * GameSystem.PIXELS_PER_UNIT;
        float textY = transform.position.y * GameSystem.CANVAS_RATIO * GameSystem.PIXELS_PER_UNIT;

        GameObject damageText = Instantiate(uiManager.GetDamageText());
        damageText.GetComponent<TextMeshProUGUI>().text = Math.Abs(change) + "";
        damageText.GetComponent<TextMeshProUGUI>().color = textColor;

        damageText.transform.SetParent(uiManager.GetTurnCanvas().transform);
        damageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(textX, textY);

        if (hasTotalBossHealth)
            boss.UpdateHp(change);
        else
            hp += change;
    }

    // Sets sprite to be white briefly and gives iframes
    public IEnumerator FlashWhite(float time=0) {
        if (iframes) {
            yield break;
        }

        iframes = true;

        // Flash white material
        StartCoroutine(GameSystem.FlashSprite(GetComponent<SpriteRenderer>(), (Material) Resources.Load("Materials/SolidWhite"), time: time));

        // Play hit sound
        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Damage/DamageEnemy"), boss.GetComponent<AudioSource>(), 0.3f);

        if (time == 0)
            time = iframeSeconds;

        // Wait out iframes
        yield return new WaitForSeconds(time);
        iframes = false;
    }

    // Flash and explode!
    public void Death() {
        StartCoroutine(GameSystem.DelayedDestroy(gameObject, deathTime));
        StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), 0));
        StartCoroutine(GameSystem.StartExploding(GetComponent<SpriteRenderer>(), explosion));
    }

    // Getters and setters yooooo
    public int GetHp() {
        return hp;
    }

    public void SetDeathTime(float seconds) {
        deathTime = seconds;
    }

    public string GetName() {
        return componentName;
    }
}
