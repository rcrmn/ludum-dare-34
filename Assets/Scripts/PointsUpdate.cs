using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointsUpdate : MonoBehaviour
{
	Text text;

	void Start()
	{
		text = GetComponent<Text>();
	}
	void Update ()
	{
		text.text = GameController.instance.getPoints().ToString();
	}
}
