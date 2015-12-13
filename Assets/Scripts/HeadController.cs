using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeadController : MonoBehaviour
{
	public float startSpeed = 1.7f;

	public int startTailLength = 1;

	public float speedIncrement = 0.1f;

	public float sizeMultiplier = 1.5f;

	public float transitionTime = 0.1f;

	public GameObject bodyBlock;

	[SerializeField]
	private float speed = 1.0f;

	[SerializeField]
	private int tailLength = 1;

	private float distanceRemaining = 0;

	private Vector2 nextDirectionChange = new Vector2(0, 0);

	private Coroutine moveForwardCo = null;

	private LinkedList<GameObject> tail = new LinkedList<GameObject>();

	private Queue<Vector3> nextTailPositions = new Queue<Vector3>();
	private Queue<Quaternion> nextTailOrientations = new Queue<Quaternion>();


	public int GetTailLength()
	{
		return tailLength;
	}

	public void AddSnakeSegment()
	{
		//tailLength += 1;
		tailLength = (int)(tailLength * sizeMultiplier)+1;
		speed += speedIncrement;
	}

	public void Stop()
	{
		if (moveForwardCo != null)
		{
			distanceRemaining = 0;
			StopCoroutine(moveForwardCo);
			moveForwardCo = null;
		}
	}

	public void Release()
	{
		foreach (var t in tail)
		{
			Destroy(t);
		}
		tail.Clear();
		nextTailPositions.Clear();
		nextTailOrientations.Clear();
		Stop();
	}

	public void Reset()
	{
		Release();
		Init();
	}

	public void Play()
	{
		moveForwardCo = StartCoroutine(MoveForward());
	}

	void Start()
	{
		GameController.instance.SetSnakeHeadController(this);

		Init();
	}


	void Init()
	{
		transform.position = new Vector3(0, 0, 0);
		transform.rotation = Quaternion.identity;

		speed = startSpeed;
		tailLength = startTailLength;

		for (int i = 0; i < tailLength; ++i)
		{
			tail.AddFirst(Instantiate(bodyBlock,
				transform.position - transform.forward.normalized * (i + 1),
				transform.rotation) as GameObject);
			if(i == 0)
			{
				tail.First.Value.GetComponent<Collider>().enabled = false;
			}
			nextTailPositions.Enqueue(transform.position - transform.forward.normalized * (tailLength - i));
			nextTailOrientations.Enqueue(transform.rotation);
		}
		nextTailPositions.Enqueue(transform.position);
		nextTailOrientations.Enqueue(transform.rotation);
	}

	void Update()
	{

		if (distanceRemaining > 0)
		{
			float d = Time.deltaTime / (transitionTime/speed);

			d = Math.Min(d, distanceRemaining);

			transform.position = transform.position + transform.forward.normalized * d;

			// Update the tail parts position and orientations
			var tailIt = tail.GetEnumerator();
			var tailNPIt = nextTailPositions.GetEnumerator();
			var tailNRIt = nextTailOrientations.GetEnumerator();

			tailNPIt.MoveNext();
			var currentPos = tailNPIt.Current;

			tailNRIt.MoveNext();
			var currentRot = tailNRIt.Current;

			while (tailIt.MoveNext() && tailNPIt.MoveNext() && tailNRIt.MoveNext())
			{
				tailIt.Current.transform.position += (tailNPIt.Current - currentPos) * d;
				tailIt.Current.transform.rotation = Quaternion.Lerp(currentRot, tailNRIt.Current, 1 - distanceRemaining);
				currentPos = tailNPIt.Current;
				currentRot = tailNRIt.Current;
			}

			distanceRemaining -= d;

			if (distanceRemaining <= 0)
			{
				nextTailPositions.Enqueue(transform.position);
				nextTailOrientations.Enqueue(transform.rotation);

				// Update the lists
				if (tail.Count == tailLength)
				{
					nextTailPositions.Dequeue();
					nextTailOrientations.Dequeue();
				}
				else
				{
					// Add a new tail part
					tail.AddFirst(Instantiate(bodyBlock,
						nextTailPositions.Peek(),
						nextTailOrientations.Peek()) as GameObject);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			nextDirectionChange = new Vector2(0, -1);
		}
		else if (Input.GetKeyDown(KeyCode.A))
		{
			nextDirectionChange = new Vector2(-1, 0);
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			nextDirectionChange = new Vector2(0, 1);
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			nextDirectionChange = new Vector2(1, 0);
		}

	}

	IEnumerator MoveForward()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / speed);

			distanceRemaining = 1.0f;

			var rotation = transform.rotation;
			rotation *= Quaternion.Euler(90 * nextDirectionChange.y, 90 * nextDirectionChange.x, 0);
			transform.rotation = rotation;

			nextDirectionChange = new Vector2(0, 0);
		}
	}
}
