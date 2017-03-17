using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NewLineDrawer : MonoBehaviour {
	public List<GameObject> LineCountersArray;
	public int i;
	public string Value = "";
	GameObject Ma ;
    public bool Visited;
    public GameObject Flame;
	NewAllGoodPlaneScr SCR;
	// Use this for initialization
	void Start () {
        Visited = false;
		Ma = GameObject.Find ("PlaneMainAdder");
		SCR = Ma.GetComponent<NewAllGoodPlaneScr> ();
		LineCountersArray = new List<GameObject> ();
	}
	

	void OnMouseDrag()
	{
		if (SCR.IsDragCl) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit)) { 
				SCR.IsFirstClick = false;
				Vector3 Temp = new Vector3(hit.point.x, hit.point.y);
				transform.position = Temp;
				for(int z = 0; z < LineCountersArray.Count; z++)
				{
					if(LineCountersArray[z] != null)
					{
						LineCountersArray[z].GetComponent<NewVarUpdate>().CleverUpdate();//BroadcastMessage("CleverUpdate");
					}
				}
			}
		}
	}
	public void ArrayOfLineCounters(GameObject CounterOfLine)
	{
		LineCountersArray.Add (CounterOfLine);
	}
	public void SetNameNText(string val)
	{
		Value = val;
		gameObject.GetComponentInChildren<TextMesh> ().text = Value;
	}
    void OnMouseDown()
    {
        //Debug.Log("On Mouse Down");
        if (SCR.DelClick)
        {
            SCR.IsFirstClick = false;

            for (int z = 0; z < LineCountersArray.Count; z++)
            {
                if (LineCountersArray[z] != null)
                {
                    Destroy(LineCountersArray[z]);
                    SCR.Line.Remove(LineCountersArray[z]);
                    //.GetComponent<NewVarUpdate>().BroadcastMessage("Deleting");
                }
            }
            SCR.Targets.Remove(this.gameObject);
            Destroy(gameObject);
            return;
        }
        if (SCR.Way1Click)
        {
            //Debug.Log("1Cl" + gameObject.name);
            if (SCR.WayObj1 != null) SCR.WayObj1.GetComponent<NewLineDrawer>().Flame.SetActive(false);
            SCR.WayObj1 = gameObject;
            gameObject.GetComponent<MeshRenderer>().material = SCR.StartMaterial;
            SCR.Way1Click = false;
            SCR.Way2Click = true;
            return;
        }
        if (SCR.Way2Click)
        {
            //Debug.Log("2Cl" + gameObject.name);
            if (SCR.WayObj2 != null) SCR.WayObj2.GetComponent<NewLineDrawer>().Flame.SetActive(false);
            SCR.WayObj2 = gameObject;
            //Flame.SetActive(true);
            gameObject.GetComponent<MeshRenderer>().material = SCR.EndMaterial;
            SCR.Way2Click = false;
            SCR.SetColPath();
            return;
        }
        if (SCR.CanCreate)
        {
            if (SCR.IsFirstClick == false)
            {
                SCR.IsFirstClick = true;
                SCR.FirstTargetIter = this.gameObject;
                //return;
            }
            else
            {
                if (SCR.Withloops)
                {
                    SCR.IsFirstClick = false;
                    SCR.SecondTargetIter = this.gameObject;
                    if (SCR.FirstTargetIter == SCR.SecondTargetIter)
                    {
                        this.gameObject.GetComponentInChildren<TextMesh>().text += "L";
                    }
                    SCR.PaintNewLine();
                }
                else
                {
                    if (SCR.FirstTargetIter != this.gameObject)
                    {
                        SCR.IsFirstClick = false;
                        SCR.SecondTargetIter = this.gameObject;
                        SCR.PaintNewLine();
                    }
                }

            }

        }
    }
}
