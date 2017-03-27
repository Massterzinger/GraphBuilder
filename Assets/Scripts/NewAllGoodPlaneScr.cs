using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Tup
{
	public int A,B;
	public Tup(int a, int b)
	{
		A = a;
		B = b;
	}
	public bool Equals(Tup obj)
	{
		return (obj.A == this.A && obj.B == this.B);
	}
}
public class NewAllGoodPlaneScr : MonoBehaviour
{
    public Material StartMaterial, EndMaterial, DefaultMaterial;
    public Text ModeText;
    public GameObject WayObj1;
    public GameObject WayObj2;
    public int[,] List_V_E;
    public List<GameObject> Targets;
    public List<GameObject> Line;
    public List<Tup> LineNames;
    public GameObject LineVert;
    public GameObject Canvass;
    public GameObject PlaneIncid;
    public GameObject InputFieldTextObject;
    private Text Tex, InputFieldText;
    public bool IsDragCl;
    public bool DelClick;
    public bool Way1Click;
    public bool Way2Click;
    public bool IsFirstClick;
    public bool SumOrIncid;
    public bool Oriented;
    public bool TabCl;
    public int MaxWeight;
    public bool Withloops, CanCreate;
    public GameObject FirstTargetIter;
    public GameObject SecondTargetIter;
    int LineCount;
    public GameObject pref;
    int ICount, JCount;
    int[,] List_History;

    int TotalLoops;
    int TarCount;
    string path;
    System.Random r;
    string DragMode = "Drag Mode";
    string CreateMode = "Creating Mode";
    string DeleteMode = "Deleting Mode";
    string PathViewMode = "Choosing path mode";
    // Use this for initialization
    void Start()
    {
        ModeText.text = CreateMode;
        IsFirstClick = false;
        TabCl = false;
        Withloops = false;
        SumOrIncid = false;
        Oriented = false;
        IsDragCl = false;
        CanCreate = true;
        LineCount = 0;
        TotalLoops = 0;
        MaxWeight = 9;
        r = new System.Random();
        Targets = new List<GameObject>();
        Line = new List<GameObject>();
        LineNames = new List<Tup>();
        Tex = PlaneIncid.GetComponent<Text>();
        InputFieldText = InputFieldTextObject.GetComponent<Text>();
    }
    public void OrientedOrNotOriented()
    {
        Oriented = !Oriented;
    }
    public void SpaceClick()
    {
        IsDragCl = true;
        DelClick = false;
        Way1Click = false;
        Way2Click = false;
        ModeText.text = DragMode;
    }
    public void CreateModeClick()
    {
        SetDefaultColor();
        CanCreate = true;
        ModeText.text = CreateMode;
    }
    public void DeleteModeClick()
    {
        SetDefaultColor();
        ModeText.text = DeleteMode;
        DelClick = true;
    }
    public void GetCheckingPathClick()
    {
        SetDefaultColor();
        Way1Click = true;
        ModeText.text = PathViewMode;
    }
    public void SumOrIncident()
    {
        SumOrIncid = !SumOrIncid;
    }
    public void WithLoopsChange()
    {
        Withloops = !Withloops;
    }
    //******************************************************************************
    void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && CanCreate && !IsDragCl)
        {
            MakeAVerticle(hit.point, "");
        }
    }
    //******************************************************************************
    void MakeAVerticle(Vector3 Point, string name)
    {
        GameObject TempVerticle = (GameObject)Instantiate(pref, Point, Quaternion.identity);

        TarCount++;
        if (name.Length < 1)
        {
            name = TarCount.ToString();
        }
        TempVerticle.name = TarCount.ToString();
        TempVerticle.GetComponent<NewLineDrawer>().SetNameNText(name);
        TempVerticle.GetComponent<NewLineDrawer>().i = TarCount;
        TempVerticle.transform.SetParent(gameObject.transform);
        Targets.Add(TempVerticle);
    }
    //******************************************************************************
    void RePaintWay()
    {

    }
    //*****************************************************************************************************
    public void MakeRandom()
    {
        int Verts = 10; int Edgs = 17;
        if (InputFieldText.text != "")
        {

            string[] a = InputFieldText.text.Split(' ');
            Verts = int.Parse(a[0]);
            Edgs = int.Parse(a[1]);
        }
        int Lind = 0, Rind = 0;
        CreateVerticles(Verts, 5);
        for (int i = 0; i < Edgs; i++)
        {
            if (!Withloops)
            {
                while (true)
                {
                    Lind = r.Next(Verts);
                    Rind = r.Next(Verts);

                    if (Lind != Rind && LineNames.Contains(new Tup(Lind, Rind)) == false && LineNames.Contains(new Tup(Rind, Lind)) == false)
                    {
                        LineNames.Add(new Tup(Lind, Rind));
                        FirstTargetIter = Targets[Lind];
                        SecondTargetIter = Targets[Rind];
                        PaintNewLine();
                        break;

                    }
                }
            }
        }

    }

    //******************************************************************************
    public void ClearAll()
    {
        SetDefaultColor();
        foreach (GameObject A in Line)
        {
            Destroy(A);
        }
        Line.Clear();
        foreach (GameObject B in Targets)
        {
            Destroy(B);
        }
        Targets.Clear();
        LineNames.Clear();
        FirstTargetIter = null;
        SecondTargetIter = null;
        IsFirstClick = false;
        CanCreate = true;
        TarCount = 0;
        LineCount = 0;
        TotalLoops = 0;
    }
    //******************************************************************************
    void FormMatrixInc()
    {
        if (List_V_E != null)
        {
            Array.Clear(List_V_E, 0, List_V_E.Length);
        }
        ICount = Targets.Count;
        JCount = Line.Count;//-TotalLoops;
        List_V_E = new int[ICount, JCount];
        path = "testMatrixIncidence.txt";

        List_V_E.Initialize();
        for (int j = 0; j < JCount; j++)
        {
            short te = 0;
            if (Oriented)
            {
                te = -1;
            }
            else
            {
                te = 1;
            }
            List_V_E[Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target1), j] = te;
            List_V_E[Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target2), j] = 1;
        }
        DrawMatrix();
    }
    //************************************************************************************************************
    void WriteMatrix(int[,] Prim, int adder)
    {
        Tex.text += "\n";
        Prim.GetLength(0);
        for (int i = 0; i < Prim.GetLength(0); i++)
        {
            for (int j = 0; j < Prim.GetLength(1); j++)
            {
                Tex.text += Prim[i, j] >= 0 ? (Prim[i, j] + adder).ToString() + '\t' : "-\t";
            }
            Tex.text += '\n';
        }
    }
    //PPPPPPPPPPPPPPPpPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP
    public void FormMatrixWeight()
    {
        path = "Floyd-Warshall.txt";
        if (List_V_E != null)
        {
            Array.Clear(List_V_E, 0, List_V_E.Length);
        }
        List_V_E = new int[Targets.Count, Targets.Count];
        List_History = new int[Targets.Count, Targets.Count];

        for (int i = 0; i < Targets.Count; i++)
        {
            for (int j = 0; j < Targets.Count; j++)
            {
                List_V_E[i, j] = 1000000;
                List_History[i, j] = -10;
            }
            List_V_E[i, i] = 0;
            List_History[i, i] = i;
        }

        for (int j = 0; j < Line.Count; j++)
        {
            int temp1 = Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target1);
            int temp2 = Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target2);
            List_V_E[temp1, temp2] = Line[j].GetComponent<NewVarUpdate>().Weight;
            List_History[temp1, temp2] = temp2;
        }

        for (int k = 0; k < Targets.Count; k++)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                for (int j = 0; j < Targets.Count; j++)
                {
                    if (List_V_E[i, k] + List_V_E[k, j] < List_V_E[i, j])
                    {
                        List_V_E[i, j] = List_V_E[i, k] + List_V_E[k, j];
                        List_History[i, j] = List_History[i, k];
                    }
                }
            }

        }
        DrawMatrix();
        WriteMatrix(List_History, 1);
    }

    List<int> GetPath(int From, int To)
    {

        if (List_History[From, To] < 0)
            return new List<int>();
        List<int> Path = new List<int>();
        Path.Add(From);
        while (From != To)
        {
            From = List_History[From, To];
            Path.Add(From);
        }
        return Path;
    }
    //ppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp
    public void SetColPath()
    {
        FormMatrixWeight();
        //Debug.Log(WayObj1 + " =F, T= " + WayObj2);
        List<int> Temp = GetPath(Targets.IndexOf(WayObj1), Targets.IndexOf(WayObj2));
        foreach (GameObject a in Line)
        {
            a.GetComponent<LineRenderer>().startColor = Color.red;
            a.GetComponent<LineRenderer>().endColor = Color.red;

        }
        for (int C = 0; C < Temp.Count - 1; C++)
        {
            SetColorLine(Temp[C], Temp[C + 1], Color.blue);
        }
    }

    void SetColorLine(int FR, int To, Color Col)
    {
        for (int Z = 0; Z < Line.Count; Z++)
        {
            if (Targets.IndexOf(Line[Z].GetComponent<NewVarUpdate>().Target1) == FR && Targets.IndexOf(Line[Z].GetComponent<NewVarUpdate>().Target2) == To)
            {
                Line[Z].GetComponent<LineRenderer>().startColor = Col;
                Line[Z].GetComponent<LineRenderer>().endColor = Col;
                break;
            }

        }
    }
    //***********************************************************************************************************************
    void FormMatrixSum()
    {
        if (List_V_E != null)
        {
            Array.Clear(List_V_E, 0, List_V_E.Length);
        }
        List_V_E = new int[Targets.Count, Targets.Count];
        ICount = Targets.Count;
        JCount = Targets.Count;
        List_V_E.Initialize();
        path = "testMatrixSumizhn.txt";
        for (int j = 0; j < Line.Count; j++)
        {
            int temp1 = Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target1);
            int temp2 = Targets.IndexOf(Line[j].GetComponent<NewVarUpdate>().Target2);
            List_V_E[temp1, temp2] += 1;
            if (!Oriented)
            {
                if (temp1 != temp2)
                {
                    List_V_E[temp2, temp1] += 1;
                }
            }

        }
        DrawMatrix();
    }
    void DrawMatrix()
    {
        Tex.text = "";

        for (int i = 0; i < List_V_E.GetLength(0); i++)
        {
            for (int j = 0; j < List_V_E.GetLength(1); j++)
            {
                if (List_V_E[i, j] > 100000)
                {
                    Tex.text += "M\t";
                }
                else
                {
                    Tex.text += List_V_E[i, j].ToString() + '\t';
                }
            }
            Tex.text += '\n';
        }
    }
    //---------------------------------------------------------------------------------------DFS
    public void DeepFirstSearch()
    {
        //Line[2].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
        Tex.text = "";
        if (WayObj1 != null) DFS(WayObj1); else DFS(Targets[0]);
    }
    void DFS(GameObject Vert)
    {
        NewLineDrawer NLD = Vert.GetComponent<NewLineDrawer>();
        if (NLD.Visited)
        {
            return;
        }
        Tex.text += Vert.name + '\t';
        NLD.Visited = true;
        for (int i = 0; i < NLD.LineCountersArray.Count; ++i)
        {
            GameObject g = NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target1 != Vert ? NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target1 : NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target2;
            if (!g.GetComponent<NewLineDrawer>().Visited)
            {
                //NLD.LineCountersArray[i].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
                NLD.LineCountersArray[i].GetComponent<LineRenderer>().startColor = Color.green;
                NLD.LineCountersArray[i].GetComponent<LineRenderer>().endColor = Color.green;
                DFS(g);
            }
        }
    }
    //---------------------------------------------------------------------------------------BFS

    public void BreadthFirstSearch()
    {
        Tex.text = "";
        if (WayObj1 != null) BFS(WayObj1); else BFS(Targets[0]);
    }

    void BFS(GameObject Vert)
    {
        GameObject Verticle = Vert;
        Queue<GameObject> Q = new Queue<GameObject>();
        Q.Enqueue(Verticle);
        Tex.text += Verticle.name + '\t';

        while (Q.Count != 0)
        {
            Verticle = Q.Dequeue();
            NewLineDrawer NLD = Verticle.GetComponent<NewLineDrawer>();

            NLD.Visited = true;
            //Tex.text += Verticle.name + '\t';

            for (int i = 0; i < NLD.LineCountersArray.Count; ++i)
            {
                GameObject g = NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target1 != Verticle ? NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target1 : NLD.LineCountersArray[i].GetComponent<NewVarUpdate>().Target2;
                if (!g.GetComponent<NewLineDrawer>().Visited)
                {
                    NLD.LineCountersArray[i].GetComponent<LineRenderer>().startColor = Color.green;
                    NLD.LineCountersArray[i].GetComponent<LineRenderer>().endColor = Color.green;
                    //NLD.LineCountersArray[i].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
                    g.GetComponent<NewLineDrawer>().Visited = true;
                    Tex.text += g.name + '\t';
                    Q.Enqueue(g);

                }
            }
        }
    }
    public void CanvPaint()
    {

        if (TabCl)
        {
            if (SumOrIncid)
            {
                FormMatrixSum();
            }
            else
            {
                FormMatrixInc();
            }
        }
    }
    public void PaintNewLine()
    {
        GameObject TempLine = (GameObject)Instantiate(LineVert, transform.position, Quaternion.identity);

        if (FirstTargetIter != SecondTargetIter)
        {
            LineCount++;
        }
        else
        {
            TotalLoops++;
        }
        int L = FirstTargetIter.GetComponent<NewLineDrawer>().i;
        int R = SecondTargetIter.GetComponent<NewLineDrawer>().i;
        string Name = (Convert.ToString(L) + "-" + Convert.ToString(R));
        int TempWei = (InputFieldText.text.IndexOf(' ') == -1) && InputFieldText.text != "" ? int.Parse(InputFieldText.text) : r.Next(1, MaxWeight);
        TempLine.GetComponent<NewVarUpdate>().Weight = TempWei;
        TempLine.name = Name + 'W' + TempWei.ToString();
        TempLine.GetComponent<NewVarUpdate>().Target1 = FirstTargetIter;
        TempLine.GetComponent<NewVarUpdate>().Target2 = SecondTargetIter;
        TempLine.GetComponent<NewVarUpdate>().CountOfLine = LineCount;
        TempLine.transform.SetParent(gameObject.transform);
        Line.Add(TempLine);


    }
    public void TabClicked()
    {
        TabCl = !TabCl;
        gameObject.GetComponent<MeshCollider>().enabled = !TabCl;
        Canvass.SetActive(TabCl);
        //CanvPaint();
    }
    void CreateVerticles(int VerticlesCount, int Radius)
    {
        // сделать расщет радиуса в зависимости от количества вершин
        //угол всегда тот-же например
        float RotationAngle = 360 / VerticlesCount;
        Vector3 rotpo = new Vector3(0, Radius, 0);
        GameObject Po = new GameObject();
        Po.transform.position = rotpo;
        for (int i = 0; i < VerticlesCount; i++)
        {
            MakeAVerticle(Po.transform.position, "");
            Po.transform.RotateAround(this.gameObject.transform.position, Vector3.forward, RotationAngle);
        }
        Destroy(Po);
    }
    public void LoadFriendsGraph()
    {
        ClearAll();

        CanCreate = false;
    }

    public void SetDefaultColor()
    {
        DelClick = false;
        CanCreate = false;
        IsDragCl = false;
        Way1Click = false;
        Way2Click = false;
        foreach (GameObject Tar in Targets)
        {
            Tar.GetComponent<NewLineDrawer>().Visited = false;
            Tar.GetComponent<MeshRenderer>().material = DefaultMaterial;
        }
        foreach (GameObject ln in Line)
        {
            ln.GetComponent<LineRenderer>().startColor = Color.red;
            ln.GetComponent<LineRenderer>().endColor = Color.red;
            //ln.GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
        }
    }
    public void LoadGraph()
    {
        ClearAll();
        if (InputFieldText.text != "") path = InputFieldText.text;
        string[] textOfPlane = File.ReadAllLines(path);
        short[][] Arr = new short[textOfPlane.Length][];
        CreateVerticles(textOfPlane.Length, 3);
        Tex.text = "";
        for (int i = 0; i < textOfPlane.Length; i++)
        {
            textOfPlane[i] = textOfPlane[i].Remove(textOfPlane[i].Length - 1);
            Arr[i] = Array.ConvertAll<string, short>(textOfPlane[i].Split('\t'), Convert.ToInt16);
            Tex.text += textOfPlane[i] + '\n';
        }
        // target1 = (find (indexof -1 ))
    }
    public void SaveMatrix()
    {
        File.WriteAllText(path, Tex.text);
    }
}