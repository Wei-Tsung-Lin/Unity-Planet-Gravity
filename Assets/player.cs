using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
	public main Main;

	public bool isGround;

	public float speed = 0.05f;

	public Vector2 groundNormal;

	private Vector3 newRight;

	private Rigidbody2D rb;

	private SpriteRenderer spriteRenderer;

	public Vector3 GravityPoint;

	private List<PolygonCollider2D> Lands = new List<PolygonCollider2D>();

	// Use this for initialization

	void Start()
	{
		Lands = Main.Lands;

		isGround = false;

		rb = GetComponent<Rigidbody2D>();

		spriteRenderer = GetComponent<SpriteRenderer>();

	}

	// Update is called once per frame
	void Update()
	{
		groundTest();   // 檢測角色是否正在地面？

		alignSurface(); // 讓角色對齊碰觸的地面

		gravityBias();  // 修正重力方向
	}

	private void gravityBias()
	{
		if (Lands.Count == 0) return;
		
		GravityPoint = getClosetPointonEdge(Lands[0]);

		float dist = Vector3.Distance(transform.position, GravityPoint);
        
		foreach (var l in Lands)
		{
			var p = getClosetPointonEdge(l);

			var d = Vector3.Distance(transform.position, p);

			if (d >= dist) continue;

			dist = d;

			GravityPoint = p;
		}

		Vector2 one = Vector3.Normalize(transform.position - GravityPoint) * (-9.8f) * Time.deltaTime;

		rb.velocity += one;

	}

	int GetClosestPointIndex(PolygonCollider2D polygonCollider2D)
	{
		Vector2[] points = polygonCollider2D.points;

		int ClosestPointIndex = 0;

		float closestDist = Mathf.Infinity;

		for (var i = 0; i < points.Length; i++)
		{
			var worldSpaceVertex = polygonCollider2D.transform.TransformPoint(points[i]);

			float currentDist = Vector3.Distance(worldSpaceVertex, transform.position);

			if (currentDist < closestDist)
			{
				closestDist = currentDist;

				ClosestPointIndex = i;
			}
		}

		return ClosestPointIndex;
	}

	Vector3 getClosetPointonEdge(PolygonCollider2D PolygonCollider2D)
	{
		Vector3 result = Vector3.zero;

		Vector2[] points = PolygonCollider2D.points;

		var index = GetClosestPointIndex(PolygonCollider2D);

		int index_next = index == points.Length - 1 ? 0 : index 
        + 1;
		int index_last = index == 0 ? points.Length - 1 : index - 1;

		Vector3 point = PolygonCollider2D.transform.TransformPoint(points[index]);

		Vector3 point_next = PolygonCollider2D.transform.TransformPoint(points[index_next]);

		Vector3 point_last = PolygonCollider2D.transform.TransformPoint(points[index_last]);

		var a = ClosestPointToLine(point, point_next, transform.position);

		var b = ClosestPointToLine(point, point_last, transform.position);

		return result = Vector3.Distance(transform.position, a) > Vector3.Distance(transform.position, b) ? b : a;
	}

	Vector3 ClosestPointToLine(Vector3 _lineStartPoint, Vector3 _lineEndPoint, Vector3 _testPoint)
	{
		Vector3 pointTowardStart = _testPoint - _lineStartPoint;

		Vector3 startTowardEnd = (_lineEndPoint - _lineStartPoint).normalized;

		float lengthOfLine = Vector3.Distance(_lineStartPoint, _lineEndPoint);

		float dotProduct = Vector3.Dot(startTowardEnd, pointTowardStart);

		if (dotProduct <= 0)
		{
			return _lineStartPoint;
		}

		if (dotProduct >= lengthOfLine)
		{
			return _lineEndPoint;
		}

		Vector3 thirdVector = startTowardEnd * dotProduct;

		Vector3 closestPointOnLine = _lineStartPoint + thirdVector;

		return closestPointOnLine;
	}
	void alignSurface()
	{
		Vector3 normal = getGroundSurface().normal;

		Vector3 newDir = Vector3.RotateTowards(transform.up, normal, 0.05f, 0.0F);

		Debug.DrawRay(transform.position, newDir, Color.blue);

		transform.rotation = Quaternion.FromToRotation(Vector3.up, newDir);
	}


	void groundTest()
	{
		isGround = Vector3.Distance(transform.position, GravityPoint) < 1f;
	}

	RaycastHit2D getGroundSurface()
	{

		var raycasthit2d = Physics2D.Raycast(transform.position + Vector3.down * 0.01f, GravityPoint - transform.position, 5f);

		return raycasthit2d;
	}

	public void goLeft()
	{
		transform.position -= transform.right * speed;

		spriteRenderer.flipX = true;

	}
	public void goRight()
	{
		transform.position += transform.right * speed;

		spriteRenderer.flipX = false;

	}
	public void goJump()
	{
		if (isGround) rb.AddForce(transform.up * 0.05f);
	}
	public void release()
	{
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); 
	}
}