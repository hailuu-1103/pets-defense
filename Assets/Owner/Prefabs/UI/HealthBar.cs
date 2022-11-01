using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public GameObject slow;

	public GameObject burning;
    

	public void SetMaxHealth(float health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
	}

    public void SetHealth(float health)
	{
		slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

	public void SetStatus(string status)
    {
        if (status.Equals("slow"))
        {
			slow.SetActive(true);

        }else if (status.Equals("burning"))
        {
			burning.SetActive(true);
        }
    }
    

	public void DisableStatus(string status)
	{
		if (status.Equals("slow"))
		{
			slow.SetActive(false);

		}
		else if (status.Equals("burning"))
		{
			burning.SetActive(false);
		}
	}

}
