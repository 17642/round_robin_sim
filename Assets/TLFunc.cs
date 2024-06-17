using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TLFunc : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    GameObject UpArrow;
    [SerializeField]
    GameObject DownArrow;
    [SerializeField]
    TMP_Text PID;
    [SerializeField]
    GameObject ProcWall;
    [SerializeField]
    TMP_Text UpPnum;
    [SerializeField]
    TMP_Text DownPnum;
    [SerializeField]
    TMP_Text Tnum;
    UUIIUUI ui;

    int it;
    bool nne;
    bool msflag = false;
    private void Start()
    {
        ui = FindFirstObjectByType<UUIIUUI>();
    }
    public void inits(bool chg, int inProc, int outProc, int PID, int time, List<ProcessData> pd)
    {
        if (!chg)
        {
            ProcWall.SetActive(false);
        }
        else
        {
            if (PID != -1)
            {
                this.PID.text = pd[PID].PID;
            }
            else
            {
                ProcWall.SetActive(false );
            }
        }

        if (PID == -1) {
            GetComponent<Image>().color = Color.gray;//프로세스가 없을 경우 색상 어둡게
        }

        if (inProc == -1)
        {
            UpArrow.SetActive(false);
        }
        else
        {
            UpPnum.text = pd[inProc].PID;
        }

        if(outProc == -1)
        {
            DownArrow.SetActive(false);
        }
        else
        {
            DownPnum.text = pd[outProc].PID;
        }

        Tnum.text = time.ToString();
        it = time;
        nne = PID != -1;

    }

    public void Update()
    {
        if (msflag)
        {
            GetComponent<Image>().color = Color.green;
        }
        else
        {
            if (UUIIUUI.intm == it)//색상 업데이트
            {
                GetComponent<Image>().color = nne ? Color.yellow : Color.red;
            }
            else
            {
                GetComponent<Image>().color = nne ? Color.white : Color.grey;

            }
        }
    }

    //마우스 상호작용 함수

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.RSTREADY();
        UUIIUUI.intm = it;
        ui.TinF.text = it.ToString();
        ui.SetTime(UUIIUUI.intm);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        msflag = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        msflag = false;
    }
}
