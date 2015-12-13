using UnityEngine;
using System.Collections;

public class BodyPartController : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		GameController.instance.OnPlayerDie();
	}
}
