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

    public bool Playing = false;//현재 코루틴 진행 중인지 확인

    private void Awake()
    {
        tls = new List<GameObject>();
    }

    public void DisplayProcessList()//프로세스 리스트 보여줌
    {
        ProcList.SetActive(true);
    }

    public void RunBtn()//재생 버튼 함수
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

    IEnumerator CRT()//1초마다 변경(재생)
    {
        Playing = true;
        for(int i = 0; i < RD.time.Count-1; i++)
        {
            intm = i;
            SetTime(intm);
            //TinF.text = intm.ToString();//입력창과 시간 설정을 동기화하지 않는다. 이는 유저의 값 변경을 용이하게 한다.
            yield return new WaitForSeconds(1);
        }
        Playing = false;
    }
    public void ScreenUpdate(RuntimeData rd)
    {
        RD = rd;//데이터 설정 후
        intm = 0;
        SetTime(intm);//시간을 0으로 초기화
        TinF.text = intm.ToString();
        int befproc = -1;
        Quantium.text = "Quantium: " + RD.quantium;
        AT.text = "Response Time: " + (RD.평균응답시간 == -1 ? "---" : RD.평균응답시간);//평균응답시간 계산에 오류가 있는 경우 --- 출력
        for (int i = 0; i < RD.time.Count-1; i++)
        {
            bool chg = befproc != RD.time[i].RunningProcess;
            befproc = RD.time[i].RunningProcess;
            tls.Add(Instantiate(ProcessLine));
            tls[i].transform.localPosition = new Vector3(tls[i].transform.localPosition.x, tls[i].transform.localPosition.y, 1);
            tls[i].GetComponent<TLFunc>().inits(chg, RD.time[i].inProcess, RD.time[i].outProcess, RD.time[i].RunningProcess, i, RD.processData);
            tls[i].transform.SetParent(ProcessView.transform);
        }
        //AT 설정, ProcList 설정
    }

    public void SetTime(int time)//레디큐, 남은 시간, 프로세스 이름 변경(시간마다 변하는 값)
    {
        TimeStamp tmp = RD.time[time];

        Processor.text = "Processor: " + (tmp.RunningProcess != -1 ? RD.processData[tmp.RunningProcess].PID :"None");
        QinF.text = RD.quantium.ToString();
        Remaining_Time.text = "Remain Time: " + (tmp.RunningProcess!=-1?(tmp.RemainintTime[tmp.RunningProcess]+1).ToString():"---");
        string RQT = "Ready Queue:\n";//레디큐 변경
        if (tmp.ProcessQueue.Count == 0) {
            RQT += "None";
        }
        else
        {
            for (int i = 0; i < tmp.ProcessQueue.Count - 1; i++)
            {
                RQT += RD.processData[tmp.ProcessQueue[i]].PID + ", ";//프로세스 이름을 불러온다.
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
        if (newQ <= 0)//Q값은 무조건 1 이상이어야 함.
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
