using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LBFunc : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField PIDs;
    [SerializeField]
    public TMP_InputField Ptimes;
    [SerializeField]
    public TMP_InputField Atimes;
    public int indexs;


    //PIDs.text = "P";
    //Ptimes.text = "1";
    //Atimes.text = "-1";

    public void SetData(ProcessData pd, int index)
    {
        PIDs.text = pd.PID;
        Ptimes.text = pd.processTime.ToString();
        Atimes.text = pd.arrivalTime.ToString();

        indexs = index;
    }

    public ProcessData RetData()//�ٲ� �ؽ�Ʈ���� �����Ѵ�.
    {
        Ptimes.text ??= "-1";
            ProcessData ret = new ProcessData(PIDs.text, int.Parse(Ptimes.text), int.Parse(Atimes.text));
        //���� ���� �������� ���� ���� ��� �⺻������ �����Ѵ�.
        if (ret.processTime<1) { ret.processTime = 1; }
        if(PIDs.text ==null|| PIDs.text == "") { ret.PID = "P"; }
        
        return ret;

    }

    public void MinusBTN()
    {
        Destroy(gameObject);//���� ��ư�� ������ �ڱ� �ڽ��� �ı��Ѵ�.
    }
}
