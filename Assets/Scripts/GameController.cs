using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
	public GameObject FoodObject;

	public GameObject[] StartScreen;

	public GameObject[] EndScreen;

	public GameObject[] PlayScreen;

	private HeadController snakeHeadController;

	private GameObject foodInstance = null;

	private System.Random rnd = new System.Random();

	private int points = 0;

	private Coroutine pointsCorot = null;

	public int getPoints()
	{
		return points;
	}

	public static GameController instance
	{
		get; private set;
	}

	public void OnPlayerDie()
	{
		Debug.Log("YOU DED!");
		snakeHeadController.Stop();

		StopCoroutine(pointsCorot);

		foreach(var i in PlayScreen)
		{
			i.SetActive(false);
		}

		foreach(var i in EndScreen)
		{
			i.SetActive(true);
		}
	}

	public void OnPlayerEat()
	{
		points += snakeHeadController.GetTailLength() * 10;

		snakeHeadController.AddSnakeSegment();

		if(foodInstance != null)
		{
			Destroy(foodInstance);
		}

		CreateFood();
	}

	public void ResetGame()
	{
		snakeHeadController.Reset();
		PreStartGame();
		points = 0;

		if(foodInstance != null)
		{
			Destroy(foodInstance);
		}
	}

	public void StartGame()
	{
		snakeHeadController.Play();
		CreateFood();
		pointsCorot = StartCoroutine(PointTimeIncrement());

		foreach(var i in PlayScreen)
		{
			i.SetActive(true);
		}
	}

	public void SetSnakeHeadController(HeadController hc)
	{
		snakeHeadController = hc;
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		PreStartGame();

		foreach(var i in PlayScreen)
		{
			i.SetActive(false);
		}
	}

	void PreStartGame()
	{
		foreach(var i in StartScreen)
		{
			i.SetActive(true);
		}

		foreach(var i in EndScreen)
		{
			i.SetActive(false);
		}
	}


	void CreateFood()
	{
		Vector3 pos = new Vector3(rnd.Next(-9, 9), rnd.Next(-9, 9), rnd.Next(-9, 9));
		while(Physics.CheckSphere(pos, 0.4f))
		{
			pos = NewRandomPos();
		}
		foodInstance = Instantiate(FoodObject, pos, Quaternion.identity) as GameObject;
	}

	Vector3 NewRandomPos()
	{
		return new Vector3(NewRandomScalar(), NewRandomScalar(), NewRandomScalar());
	}

	float NewRandomScalar()
	{
		return (rnd.Next(0, 18) + rnd.Next(0, 19))/2 - 9;
	}

	IEnumerator PointTimeIncrement()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			points += 1;
		}
	}
}
