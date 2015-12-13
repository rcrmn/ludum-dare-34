using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		GameController.instance.OnPlayerEat();
	}
}
