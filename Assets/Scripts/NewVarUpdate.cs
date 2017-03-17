using UnityEngine;
using System.Collections;

public class NewVarUpdate : MonoBehaviour
{
    public GameObject Target1;
    public GameObject Target2;
    public GameObject TextO;
    public int CountOfLine;
    public int Weight;
    GameObject Ma;
    // Use this for initialization
    void Start()
    {
        gameObject.GetComponentInChildren<TextMesh>().text = CountOfLine.ToString() + 'W' + Weight;
        Ma = GameObject.Find("PlaneMainAdder");
        if (Target1 != null || Target2 != null)
        {
            //gameObject.GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
            gameObject.GetComponent<LineRenderer>().startColor = Color.red;
            gameObject.GetComponent<LineRenderer>().endColor = Color.red;
            Target1.GetComponent<NewLineDrawer>().ArrayOfLineCounters(gameObject);
            Target2.GetComponent<NewLineDrawer>().ArrayOfLineCounters(gameObject);
            CleverUpdate();
        }
    }

    void OnMouseDown()
    {
        if (Ma.GetComponent<NewAllGoodPlaneScr>().DelClick)
        {
            Ma.GetComponent<NewAllGoodPlaneScr>().IsFirstClick = false;
            Deleting();
        }

    }
    void Deleting()
    {
        Target1.GetComponent<NewLineDrawer>().LineCountersArray.Remove(this.gameObject);
        Target2.GetComponent<NewLineDrawer>().LineCountersArray.Remove(this.gameObject);
        Ma.GetComponent<NewAllGoodPlaneScr>().Line.Remove(this.gameObject);
        Destroy(gameObject);
    }
    // Update is called once per frame
    public void CleverUpdate()
    {

        if (Target1 == null || Target2 == null)
        {
            Deleting();
        }
        else
        {
            Ma.GetComponent<NewAllGoodPlaneScr>().IsFirstClick = false;
            GetComponent<BoxCollider>().transform.position = (Target1.transform.position + Target2.transform.position) / 2;
            GetComponent<LineRenderer>().SetPosition(0, Target1.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, Target2.transform.position);

        }
    }
}