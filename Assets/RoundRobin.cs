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
    public float �������ð�;
    public float ��մ��ð�;

    public RuntimeData()
    {
        time = new List<TimeStamp>();
        processData = new List<ProcessData>();
        processTime = new List<int>();
    }
}



public class RoundRobin : MonoBehaviour
{
    public int quantium;//���ڰ�
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
            rd.processData.Add(pd);//���μ��� ������ ä���
        }
        rd.quantium = q;
        return rd;
    }

    public void SetData(RuntimeData rd)//�������ð� ����
    {
        float rrst = 0.0f;
        List<int> processInTime = new();
        List<int> processOutTime = new();//�뷮 ������ ������ ���������� �Ҵ������ �ʴ´�.

        for(int i = 0; i < rd.processData.Count; i++)
        {
            processInTime.Add(-1);//�׷��Ƿ� �ʱⰪ�� �Ҵ�������.
            processOutTime.Add(-1);
        }

        Debug.Log("PITSIZE: " + rd.processData.Count + ", SAMPLE: " + processInTime[0]);
        

        for(int i=0;i<rd.time.Count;i++)
        {
            if (rd.time[i].inProcess != -1)
            {
                Debug.Log("" + rd.time[i].inProcess + " RDt "+ i+ " TIME");
                processInTime[rd.time[i].inProcess] = i;//�ð� ���
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
            rrst -= processInTime[i];//rrst ���
        }

        if(rrst == 0)//rrst���� 0�� ���(����� �� ���� ���)
        {
            rd.�������ð� = -1;
        }
        else
        {
            Debug.Log("RRST: " + rrst);
            rd.�������ð� = rrst / rd.processData.Count;//��� ���� �ð� ���
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
            foreach (ProcessData tt in rd.processData) // ���μ��� �����Ͱ� ���� ���
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
                news.RemainintTime.Add(rt.processTime - 1);//�ð� ���� �� �ʱ�ȭ
            }

            for (int s = 0; s < rd.processData.Count; s++)
            {
                if (rd.processData[s].arrivalTime == 0)
                {
                    if (news.RunningProcess == -1)//�ƹ��͵� �������� ����
                    {
                        news.RunningProcess = s;
                        news.remainQuantium = rd.quantium-1;
                        news.inProcess = s;
                        break;//���ο� Ÿ�ӽ����� ���� ��
                    }
                }
            }


        }
        else
        {
            //news �ʱ�ȭ
            foreach (int ts in before.RemainintTime)
            {
                news.RemainintTime.Add(ts);//�ð� ����
            }

            foreach (int s in before.ProcessQueue)
            {
                news.ProcessQueue.Add(s);//���� Ÿ���� ť ����
            }

            for (int s = 0; s < rd.processData.Count; s++)//���μ��� ť ����
            {
                if (rd.processData[s].arrivalTime == internalTIme && before.RemainintTime[s] != 0)
                {
                    news.ProcessQueue.Add(s);
                    news.inProcess = s;
                    break;//���ο� Ÿ�ӽ����� ���� ��
                }
            }

            if (before.RunningProcess != -1)
            {
                if (before.RemainintTime[before.RunningProcess] == 0)//�ð��� ��� ���Ǹ�
                {
                    news.RunningProcess = -1;
                    news.outProcess = before.RunningProcess;//������Ʈ
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
                if (before.remainQuantium == 0)//������ ��� ���Ǹ�
                {
                    if (news.ProcessQueue.Count == 0)//�׷��� ������ ���μ����� ������
                    {
                        //���� �ð� ���� �� ���μ����� ����
                        news.RemainintTime[news.RunningProcess]--;//���� �ð��� 1 ����
                    }
                    else
                    {
                        news.ProcessQueue.Add(news.RunningProcess);
                        news.RemainintTime[news.RunningProcess]--;
                        news.RunningProcess = -1;//���μ��� ���¸� �����Ѵ�.
                    }
                }
                else//�ƴϸ�
                {//���μ����� �����Ѵ�.
                    //news.RunningProcess = before.RunningProcess;
                    news.remainQuantium = before.remainQuantium - 1;
                    news.RemainintTime[news.RunningProcess]--;//���� �ð��� 1 ����.
                }
            }



            if (news.RunningProcess == -1 && news.ProcessQueue.Count > 0)
            {
                news.RunningProcess = news.ProcessQueue[0];
                news.ProcessQueue.RemoveAt(0);
                news.remainQuantium = rd.quantium - 1;//���� �ʱ�ȭ

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