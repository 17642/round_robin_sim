using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UUIIUUI : MonoBehaviour
{
    [SerializeField]
    public TMP_Text Quantium;
    [SerializeField]
    public TMP_Text ReadyQ;
    [SerializeField]
    public TMP_Text Processor;
    [SerializeField]
    public TMP_Text AT;
    [SerializeField]
    public GameObject ProcessView;
    [SerializeField]
    public GameObject ProcList;
    [SerializeField]
    public GameObject ProcessLine;
    [SerializeField]
    public TMP_Text inttime;
    [SerializeField]
    public TMP_Text Remaining_Time;
    [SerializeField]
    public TMP_InputField TinF;
    [SerializeField]
    public TMP_InputField QinF;
    [SerializeField]
    public RoundRobin RR;
    [SerializeField]
    public TMP_Text BTNTEXT;

    List<GameObject> tls;

    public RuntimeData RD;

    public static int intm;

    public bool Playing = false;//���� �ڷ�ƾ ���� ������ Ȯ��

    private void Awake()
    {
        tls = new List<GameObject>();
    }

    public void DisplayProcessList()//���μ��� ����Ʈ ������
    {
        ProcList.SetActive(true);
    }

    public void RunBtn()//��� ��ư �Լ�
    {
        if (!Playing)
        {
            RSTREADY();
            StartCoroutine(CRT());
            BTNTEXT.text = "Pause";
        }
        else
        {
            RSTREADY();
        }
    }

    IEnumerator CRT()//1�ʸ��� ����(���)
    {
        Playing = true;
        for(int i = 0; i < RD.time.Count-1; i++)
        {
            intm = i;
            SetTime(intm);
            //TinF.text = intm.ToString();//�Է�â�� �ð� ������ ����ȭ���� �ʴ´�. �̴� ������ �� ������ �����ϰ� �Ѵ�.
            yield return new WaitForSeconds(1);
        }
        Playing = false;
    }
    public void ScreenUpdate(RuntimeData rd)
    {
        RD = rd;//������ ���� ��
        intm = 0;
        SetTime(intm);//�ð��� 0���� �ʱ�ȭ
        TinF.text = intm.ToString();
        int befproc = -1;
        Quantium.text = "Quantium: " + RD.quantium;
        AT.text = "Response Time: " + (RD.�������ð� == -1 ? "---" : RD.�������ð�);//�������ð� ��꿡 ������ �ִ� ��� --- ���
        for (int i = 0; i < RD.time.Count-1; i++)
        {
            bool chg = befproc != RD.time[i].RunningProcess;
            befproc = RD.time[i].RunningProcess;
            tls.Add(Instantiate(ProcessLine));
            tls[i].transform.localPosition = new Vector3(tls[i].transform.localPosition.x, tls[i].transform.localPosition.y, 1);
            tls[i].GetComponent<TLFunc>().inits(chg, RD.time[i].inProcess, RD.time[i].outProcess, RD.time[i].RunningProcess, i, RD.processData);
            tls[i].transform.SetParent(ProcessView.transform);
        }
        //AT ����, ProcList ����
    }

    public void SetTime(int time)//����ť, ���� �ð�, ���μ��� �̸� ����(�ð����� ���ϴ� ��)
    {
        TimeStamp tmp = RD.time[time];

        Processor.text = "Processor: " + (tmp.RunningProcess != -1 ? RD.processData[tmp.RunningProcess].PID :"None");
        QinF.text = RD.quantium.ToString();
        Remaining_Time.text = "Remain Time: " + (tmp.RunningProcess!=-1?(tmp.RemainintTime[tmp.RunningProcess]+1).ToString():"---");
        string RQT = "Ready Queue:\n";//����ť ����
        if (tmp.ProcessQueue.Count == 0) {
            RQT += "None";
        }
        else
        {
            for (int i = 0; i < tmp.ProcessQueue.Count - 1; i++)
            {
                RQT += RD.processData[tmp.ProcessQueue[i]].PID + ", ";//���μ��� �̸��� �ҷ��´�.
            }
            RQT += RD.processData[tmp.ProcessQueue[^1]].PID;
        }

        inttime.text = "Time: " + time;

        ReadyQ.text = RQT;

    }

    public void RSTREADY()
    {
        StopAllCoroutines();
        Playing = false;
        BTNTEXT.text = "Play";
    }

    public void chgTime()
    {
        RSTREADY();
        intm = int.Parse(TinF.text);
        if (intm < 0)
        {
            intm = 0;
            TinF.text = "0";
        }
        if (intm > RD.time.Count - 2)
        {
            intm =  RD.time.Count - 2;
            TinF.text = (RD.time.Count-2).ToString();
        }
        SetTime(intm);
    }

    public void chgQt()
    {
        RSTREADY();
        int newQ = int.Parse(QinF.text);
        if (newQ <= 0)//Q���� ������ 1 �̻��̾�� ��.
        {
            newQ = 1;
            QinF.text = "1";
        }
        for(int i = 0; i < RD.time.Count-1; i++)
        {
            Destroy(tls[i]);        
        }
        tls.Clear();
        RR.quantium = newQ;
        RR.RunOnce(RR.quantium);
    }

    public void RemoveALL()
    {
        int k = tls.Count;
        for(int i = 0; i < k; i++)
        {
            Destroy(tls[i]);
        }
        tls.Clear();
    }
}
