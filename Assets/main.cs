using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
	public GameObject player1;

	private player p1;

	public List <PolygonCollider2D> Lands = new List<PolygonCollider2D> ();

    void Start()
    {
		Lands = GetLandsFormTag ();

		p1 = player1.AddComponent<player> ();

		p1.Main =this;
    }

	private List <PolygonCollider2D>GetLandsFormTag ()
	{
		List <PolygonCollider2D>polygonCollider2Ds=new List<PolygonCollider2D>();

		GameObject[]LandGameObjects=GameObject.FindGameObjectsWithTag("Land");

		foreach (var g in LandGameObjects)polygonCollider2Ds.Add(g.GetComponent<PolygonCollider2D>());

		return polygonCollider2Ds;
	}
		
    // Update is called once per frame
    void Update()
    {
        control();
    }

    void control()
    {
        if (Input.GetKey("a")) p1.goLeft();

        if (Input.GetKey("d")) p1.goRight();

        if (Input.GetKeyDown("space")) p1.goJump();

        if (Input.GetKeyUp("space")) p1.release();
    }
}
