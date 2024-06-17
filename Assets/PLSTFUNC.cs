using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLSTFUNC : MonoBehaviour
{
    [SerializeField]
    public GameObject SCRL;
    [SerializeField]
    public GameObject PlBtn;
    [SerializeField]
    public GameObject DatLine;
    [SerializeField]
    public RoundRobin RR;
    [SerializeField]
    public Button CloseBtn;
    [SerializeField]
    UUIIUUI ui;

    bool isChg;

    public List<GameObject> datLines;

    private void Awake()
    {

        
    }

    private void OnEnable()//���� ProcessList�� ����ȭ�ϰ�
    {
        //isChg = false;
        //datLines.Clear();
        datLines = new List<GameObject>();

        for (int i = 0; i < RR.processList.Count; i++)
        {
            Debug.Log("DAT ADDED " + RR.processList[i].PID);   
            datLines.Add(Instantiate(DatLine));//�߰��ϰ�
            datLines[i].transform.SetParent(PlBtn.transform.parent);
            datLines[i].GetComponent<LBFunc>().SetData(RR.processList[i],i);//�����ϰ�(PlBtn ��ġ�� �������� �ʾ����Ƿ� datLines�� ��ĭ �ڿ� �ִ�.
        }
        Debug.Log("DATCNT " + datLines.Count);
        PlBtn.transform.SetSiblingIndex(PlBtn.transform.parent.childCount - 1);
    }
    public void PlusButton()
    {
        datLines.Add(Instantiate(DatLine));
        datLines[^1].transform.SetParent(PlBtn.transform.parent);
        PlBtn.transform.SetSiblingIndex(PlBtn.transform.parent.childCount - 1);
    }

    bool AllInputtFieldfilled()
    {
        foreach(GameObject bo in datLines)
        {
            LBFunc lf = bo.GetComponent<LBFunc>();
            if (string.IsNullOrEmpty(lf.PIDs.text + lf.Ptimes.text + lf.Atimes.text))
            {
                return false;
            }
        }

        return true;
    }

    public void Update()
    {
        if (!AllInputtFieldfilled())
        {
            CloseBtn.enabled = false;
        }
        else
        {
            CloseBtn.enabled = true;
        }
    }

    public void CloseButton()//���� �� ProcessList�� ����ȭ�Ѵ�.
    {
        ui.RSTREADY();
        ui.RemoveALL();
        List<ProcessData> ns = new List<ProcessData>();
        Debug.Log("NSLIST " + datLines.Count);
        for (int i = 0; i < datLines.Count; i++)
        {
            if (datLines[i] != null)
            {
                Debug.Log("ADDED " + datLines[i].GetComponent<LBFunc>().PIDs.text);
                ns.Add(datLines[i].GetComponent<LBFunc>().RetData());
                Destroy(datLines[i]);
            }

        }

        //int s = datLines.Count;
        datLines.Clear();

        RR.processList = ns;
        //if (isChg)
        {
            RR.RunOnce(RR.quantium);//���� �߻� ����
        }
        gameObject.SetActive(false);
    }
}
