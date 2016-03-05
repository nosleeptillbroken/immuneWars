using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveCountdown : MonoBehaviour {

    private Slider slider;

    private bool countdown = false;

    private float _countdownTime;
    private float _remainingTime;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(countdown)
        {
            _remainingTime -= Time.deltaTime;
            slider.value = _remainingTime;
        }
	}

    public void BeginCountdown(float time)
    {
        countdown = true;
        slider.maxValue = _countdownTime = time;
        slider.value = _remainingTime = time;
    }
}
