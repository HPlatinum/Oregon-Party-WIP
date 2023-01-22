using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndSanityTracker : MonoBehaviour{

    public int maxHealth;
    public int maxSanity;
    public int currentHealth;
    public int currentSanity;

    private List<DangerObject> dangerObjects = new List<DangerObject>();
    private bool justDealtDamage = false;
    private float timeBetweenDamage = 1f; //in seconds

    private float healthBarDisplayLengthPerPoint; //how much RectTransform width the current health bar shows per health point the player has
    private float sanityBarDisplayLengthPerPoint; //how much RectTransform width the current sanity bar shows per sanity point the player has

    private RectTransform currentHealthBar;
    private RectTransform currentSanityBar;
    private Text currentHealthText;
    private Text currentSanityText;


    public void SetUpBarSizes() {
        //the health and sanity bar display has 3 components - the background, the max health bar, and the current health bar
        
        //"normal" bar and background length is the length the bars would be if the player had 100 max health and sanity
        Vector2 normalHealthBackgroundSize = transform.Find("Health and Sanity").Find("Health Bar").GetComponent<RectTransform>().sizeDelta;
        Vector2 normalSanityBackgroundSize = transform.Find("Health and Sanity").Find("Health Bar").GetComponent<RectTransform>().sizeDelta;
        float maxHealthBarRightAnchor = Mathf.Abs(transform.Find("Health and Sanity").Find("Health Bar").Find("Max Health").GetComponent<RectTransform>().offsetMax.x);
        float maxSanityBarRightAnchor = Mathf.Abs(transform.Find("Health and Sanity").Find("Sanity Bar").Find("Max Sanity").GetComponent<RectTransform>().offsetMax.x);
        float normalMaxHealthBarLength = normalHealthBackgroundSize.x - maxHealthBarRightAnchor;
        float normalMaxSanityBarLength = normalSanityBackgroundSize.x - maxSanityBarRightAnchor;

        //"new" bar and background length is based on the player's actual health and sanity values
        float healthBarLengthPerPoint = normalMaxHealthBarLength / 100f;
        float sanityBarLengthPerPoint = normalMaxSanityBarLength / 100f;

        float newMaxHealthBarLength = healthBarLengthPerPoint * maxHealth;
        float newMaxSanityBarLength = sanityBarLengthPerPoint * maxSanity;

        float newHealthBackgroundLength = newMaxHealthBarLength + maxHealthBarRightAnchor;
        float newSanityBackgroundLength = newMaxSanityBarLength + maxSanityBarRightAnchor;

        Vector2 newHealthBackgroundSize = new Vector2(newHealthBackgroundLength, normalHealthBackgroundSize.y);
        Vector2 newSanityBackgroundSize = new Vector2(newSanityBackgroundLength, normalSanityBackgroundSize.y);

        //set the new background size, based on the calculated max length of the bar
        transform.Find("Health and Sanity").Find("Health Bar").GetComponent<RectTransform>().sizeDelta = newHealthBackgroundSize;
        transform.Find("Health and Sanity").Find("Sanity Bar").GetComponent<RectTransform>().sizeDelta = newSanityBackgroundSize;

        //set the displayLengthPerPoint values
        healthBarDisplayLengthPerPoint = healthBarLengthPerPoint;
        sanityBarDisplayLengthPerPoint = sanityBarLengthPerPoint;
    }

    public void SetStartingValues(int maxHealth, int maxSanity) {
        //temp();

        this.maxHealth = maxHealth;
        this.maxSanity = maxSanity;

        //start at full health
        currentHealth = this.maxHealth;
        currentSanity = this.maxSanity;

        SetUpBarSizes();

        currentHealthBar = transform.Find("Health and Sanity").Find("Health Bar").Find("Max Health").Find("Remaining Health").GetComponent<RectTransform>();
        currentSanityBar = transform.Find("Health and Sanity").Find("Sanity Bar").Find("Max Sanity").Find("Remaining Sanity").GetComponent<RectTransform>();

        currentHealthText = transform.Find("Health and Sanity").Find("Health Fraction").Find("Text").GetComponent<Text>();
        currentSanityText = transform.Find("Health and Sanity").Find("Sanity Fraction").Find("Text").GetComponent<Text>();

        UpdateVisuals();
    }

    private void UpdateVisuals() {
        int missingHealth = maxHealth - currentHealth;
        int missingSanity = maxSanity - currentSanity;

        float missingHealthLength = missingHealth * healthBarDisplayLengthPerPoint;
        float missingSanityLength = missingSanity * sanityBarDisplayLengthPerPoint;

        currentHealthBar.offsetMax = new Vector2(-missingHealthLength, currentHealthBar.offsetMax.y);
        currentSanityBar.offsetMax = new Vector2(-missingSanityLength, currentSanityBar.offsetMax.y);

        currentHealthText.text = currentHealth + "/" + maxHealth;
        currentSanityText.text = currentSanity + "/" + maxSanity;
    }

    public void GainHealth(int amount) {
        //print("health change: +" + amount);
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateVisuals();
    }

    public void LoseHealth(int amount) {
        //print("health change: -" + amount);
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;
        UpdateVisuals();

        if (currentHealth == 0)
            print("out of health");
    }

    public void GainSanity(int amount) {
        //print("sanity change: +" + amount);
        currentSanity += amount;
        if (currentSanity > maxSanity)
            currentSanity = maxSanity;
        UpdateVisuals();
    }

    public void LoseSanity(int amount) {
        //print("sanity change: -" + amount);
        currentSanity -= amount;
        if (currentSanity < 0)
            currentSanity = 0;
        UpdateVisuals();

        if (currentSanity == 0)
            print("out of sanity");
    }

    public void EnterDangerZone(DangerObject dango) {
        dangerObjects.Add(dango);
        if (!justDealtDamage)
            RepeatedlyDealDamageFromDangers(); //if you have just taken damage, entering a new danger zone won't deal damage again
        //print("enter danger zone - danger obj count: " + dangerObjects.Count);
    }

    public void LeaveDangerZone(DangerObject dango) {
        dangerObjects.Remove(dango);
        //print("exit danger zone - danger obj count: " + dangerObjects.Count);
    }

    private bool DealDamageFromDangers() {
        //only deal damage from 1 danger object of each type, so we can have overlapping damage areas
        //returns true if any damage was dealt
        int highestHealthDamage = 0;
        int highestSanityDamage = 0;
        foreach (DangerObject dango in dangerObjects) {
            if (dango.damageType == DangerObject.DamageType.health && dango.damageAmount > highestHealthDamage)
                highestHealthDamage = dango.damageAmount;
            if (dango.damageType == DangerObject.DamageType.sanity && dango.damageAmount > highestSanityDamage)
                highestSanityDamage = dango.damageAmount;
        }
        if (highestHealthDamage > 0)
            LoseHealth(highestHealthDamage);
        if (highestSanityDamage > 0)
            LoseSanity(highestSanityDamage);

        if ((highestHealthDamage == 0) && (highestSanityDamage == 0)) 
            return false;
        return true;
        
    }

    private void RepeatedlyDealDamageFromDangers() {
        //does damage, and then waits the cooldown time, and does damage again
        //if no damage is dealt, the cycle ends
        justDealtDamage = DealDamageFromDangers();
        if (justDealtDamage)
            StaticVariables.WaitTimeThenCallFunction(timeBetweenDamage, RepeatedlyDealDamageFromDangers);
    }

}
