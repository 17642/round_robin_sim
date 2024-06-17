using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProcessData
{
    public string PID;
    public int processTime;
    public int arrivalTime;
   // public Color color;

    public ProcessData(string name, int pt, int at)
    {
        PID = name;
        processTime = pt;
        arrivalTime = at;
        //this.color = color;
    }
}

public class TimeStamp
{
    public int RunningProcess;
    public int remainQuantium;
    public List<int> ProcessQueue;
    public List<int> RemainintTime;
    public int inProcess;
    public int outProcess;

    public TimeStamp()
    {
        ProcessQueue = new List<int>();
        RemainintTime = new List<int>();
        RunningProcess = -1;
        inProcess = -1;
        outProcess = -1;
    }
}

public class RuntimeData
{
    public string RuntimeName;
    public List<TimeStamp> time;
    public List<ProcessData> processData;
    public List<int> processTime;
    public int runTime;
    public int quantium;
    public float 평균응답시간;
    public float 평균대기시간;

    public RuntimeData()
    {
        time = new List<TimeStamp>();
        processData = new List<ProcessData>();
        processTime = new List<int>();
    }
}



public class RoundRobin : MonoBehaviour
{
    public int quantium;//양자값
    public List<ProcessData> processList;
    //public List<int> temporalTime;
    //public List<RuntimeData> runtimeDatas;
    [SerializeField]
    public UUIIUUI cav;
    public void Awake()
    {
        //temporalTime = new List<int>();
        //processList = new List<ProcessData>();
        //runtimeDatas = new List<RuntimeData>();
    }

    //public void Tinit()
    //{
    //    temporalTime.Clear();
    //    for (int i = 0; i < processList.Count; i++)
    //    {
    //        temporalTime.Add(processList[i].processTime);
    //    }
    //}
    public RuntimeData StartRun(int q)
    {
        RuntimeData rd = new RuntimeData();
        foreach (ProcessData pd in processList)
        {
            rd.processData.Add(pd);//프로세스 데이터 채우기
        }
        rd.quantium = q;
        return rd;
    }

    public void SetData(RuntimeData rd)//평균응답시간 결정
    {
        float rrst = 0.0f;
        List<int> processInTime = new();
        List<int> processOutTime = new();//용량 설정은 되지만 실질적으로 할당되지는 않는다.

        for(int i = 0; i < rd.processData.Count; i++)
        {
            processInTime.Add(-1);//그러므로 초기값을 할당해주자.
            processOutTime.Add(-1);
        }

        Debug.Log("PITSIZE: " + rd.processData.Count + ", SAMPLE: " + processInTime[0]);
        

        for(int i=0;i<rd.time.Count;i++)
        {
            if (rd.time[i].inProcess != -1)
            {
                Debug.Log("" + rd.time[i].inProcess + " RDt "+ i+ " TIME");
                processInTime[rd.time[i].inProcess] = i;//시간 기록
            }
            if (rd.time[i].outProcess != -1)
            {
                Debug.Log("" + rd.time[i].outProcess + " RDO " + i + " TIME");
                processOutTime[rd.time[i].outProcess] = i;
            }

        }
        for(int i = 0; i < rd.processData.Count; i++)
        {
            rrst += processOutTime[i];
            rrst -= processInTime[i];//rrst 계산
        }

        if(rrst == 0)//rrst값이 0인 경우(계산할 수 없는 경우)
        {
            rd.평균응답시간 = -1;
        }
        else
        {
            Debug.Log("RRST: " + rrst);
            rd.평균응답시간 = rrst / rd.processData.Count;//평균 응답 시간 계산
        }

        //runtimeDatas.Add(rd);
    }

    public bool Run(RuntimeData rd, TimeStamp before, int internalTIme)
    {
        bool rx = false;

        if (before != null)
        {

            foreach (int tt in before.RemainintTime)
            {
                if (tt != 0)
                {
                    rx = true;
                    break;
                }
            }
        }

        TimeStamp news = new TimeStamp();

        if (before == null)
        {
            foreach (ProcessData tt in rd.processData) // 프로세스 데이터가 없을 경우
            {
                if (tt.processTime != 0)
                {
                    rx = true;
                    break;
                }
            }

            if (rx == false)
            {

                return false;

            }

            foreach (ProcessData rt in rd.processData)
            {
                news.RemainintTime.Add(rt.processTime - 1);//시간 삽입 및 초기화
            }

            for (int s = 0; s < rd.processData.Count; s++)
            {
                if (rd.processData[s].arrivalTime == 0)
                {
                    if (news.RunningProcess == -1)//아무것도 도달하지 않음
                    {
                        news.RunningProcess = s;
                        news.remainQuantium = rd.quantium-1;
                        news.inProcess = s;
                        break;//새로운 타임스탬프 설정 끝
                    }
                }
            }


        }
        else
        {
            //news 초기화
            foreach (int ts in before.RemainintTime)
            {
                news.RemainintTime.Add(ts);//시간 삽입
            }

            foreach (int s in before.ProcessQueue)
            {
                news.ProcessQueue.Add(s);//이전 타임의 큐 삽입
            }

            for (int s = 0; s < rd.processData.Count; s++)//프로세스 큐 관리
            {
                if (rd.processData[s].arrivalTime == internalTIme && before.RemainintTime[s] != 0)
                {
                    news.ProcessQueue.Add(s);
                    news.inProcess = s;
                    break;//새로운 타임스탬프 설정 끝
                }
            }

            if (before.RunningProcess != -1)
            {
                if (before.RemainintTime[before.RunningProcess] == 0)//시간이 모두 고갈되면
                {
                    news.RunningProcess = -1;
                    news.outProcess = before.RunningProcess;//업데이트
                }
                else
                {
                    news.RunningProcess = before.RunningProcess;
                }
            }
            else
            {
                news.RunningProcess = -1;
            }


            if (news.RunningProcess != -1)
            {
                if (before.remainQuantium == 0)//퀀텀이 모두 고갈되면
                {
                    if (news.ProcessQueue.Count == 0)//그런데 진입할 프로세스가 없으면
                    {
                        //진행 시간 유지 후 프로세스를 끈다
                        news.RemainintTime[news.RunningProcess]--;//수행 시간도 1 뺀다
                    }
                    else
                    {
                        news.ProcessQueue.Add(news.RunningProcess);
                        news.RemainintTime[news.RunningProcess]--;
                        news.RunningProcess = -1;//프로세스 상태를 변경한다.
                    }
                }
                else//아니면
                {//프로세스를 수행한다.
                    //news.RunningProcess = before.RunningProcess;
                    news.remainQuantium = before.remainQuantium - 1;
                    news.RemainintTime[news.RunningProcess]--;//수행 시간도 1 뺀다.
                }
            }



            if (news.RunningProcess == -1 && news.ProcessQueue.Count > 0)
            {
                news.RunningProcess = news.ProcessQueue[0];
                news.ProcessQueue.RemoveAt(0);
                news.remainQuantium = rd.quantium - 1;//퀀텀 초기화

            }
        }




        rd.time.Add(news);

        Debug.Log("PROCESS " + news.RunningProcess + " IN: " + news.inProcess + " OUT: " + news.outProcess+ " RQ: "+news.remainQuantium+ (news.RunningProcess==-1?"":" RT: "+ news.RemainintTime[news.RunningProcess]));

        if (news.outProcess == -1)
        {
            return rx;
        }

        return true;
    }

    public void RunOnce(int q)
    {
        //Tinit();
        RuntimeData tempRd = StartRun(q);
        for (int g = 0; g < tempRd.processData.Count; g++)
        {
            Debug.Log(tempRd.processData[g].PID + " - " + tempRd.processData[g].processTime + " - " + tempRd.processData[g].arrivalTime);
        }
        bool bfd = true;
        TimeStamp bf = null;
        int i = 0;
        //tempRd.time.Add(new TimeStamp());
        //tempRd.time.Add(new TimeStamp());
        while (bfd)
        {

            bfd = Run(tempRd, bf, i);
            if (tempRd.time.Count != 0)
            {
                bf = tempRd.time[^1];
            }
            i++;
        }
        SetData(tempRd);
        cav.ScreenUpdate(tempRd);
        cav.SetTime(0);
    }

    private void Start()
    {
        RunOnce(quantium);
    }
}