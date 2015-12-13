using UnityEngine;
using System.Collections;

public class BoxController : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		GameController.instance.OnPlayerDie();
	}
}
