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

    public ProcessData RetData()//바뀐 텍스트값을 리턴한다.
    {
        Ptimes.text ??= "-1";
            ProcessData ret = new ProcessData(PIDs.text, int.Parse(Ptimes.text), int.Parse(Atimes.text));
        //만약 값이 적절하지 못한 값일 경우 기본값으로 수정한다.
        if (ret.processTime<1) { ret.processTime = 1; }
        if(PIDs.text ==null|| PIDs.text == "") { ret.PID = "P"; }
        
        return ret;

    }

    public void MinusBTN()
    {
        Destroy(gameObject);//빼기 버튼이 눌리면 자기 자신을 파괴한다.
    }
}
