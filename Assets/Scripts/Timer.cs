using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool runTimer;
    public float remainingTime;
    public string timerText;
    public float minutes;
    public float seconds;
    public float milliseconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerIsRunning()) {
            if(remainingTime > 0) {
                remainingTime -= Time.deltaTime;
                if(remainingTime < 0) {
                    remainingTime = 0;
                }
            }
            else {
                StopGameTimer();
            }
        }
    }

    public void StartGameTimer(float timerTime) {
        remainingTime = timerTime;
        runTimer = true;
    }

    public void StopGameTimer() {
        remainingTime = 0;
        runTimer = false;
    }

    public bool TimerIsRunning() {
        return runTimer;
    }

    public string GetTimeForDisplaying() {
        minutes = Mathf.FloorToInt(remainingTime / 60);
        seconds = Mathf.FloorToInt(remainingTime % 60);
        milliseconds = (remainingTime % 1) * 1000;
        timerText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        return timerText;
    }

    public float GetTimeForChangingDisplayColor() {
        return remainingTime;
    }
}
