using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckVersionUI : MonoBehaviour
{
    Image m_pProgressImg;
    Text m_pProgressText;
    GameObject m_pAlertUI;
    Button m_pAlertOKButton;
    Text m_pAlertText;
    Action m_pAlertCallback;

    //多线程赋值,update刷新
    float m_pProgress = -1;
    string m_pTipText = string.Empty;

    private void Awake() {
        m_pProgressImg = transform.Find("ProgressBar/Bar").GetComponent<Image>();
        m_pProgressText = transform.Find("ProgressBar/Text").GetComponent<Text>();
        m_pProgressText.text = string.Empty;

        m_pAlertUI = transform.Find("AlertUI").gameObject;
        m_pAlertUI.gameObject.SetActivityExt(false);

        m_pAlertOKButton = transform.Find("AlertUI/OkButton").GetComponent<Button>();
        m_pAlertText = transform.Find("AlertUI/TipText").GetComponent<Text>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (m_pProgress >= 0) {
            m_pProgressImg.fillAmount = Math.Min(m_pProgress, 1f);
        }
        if(!string.IsNullOrEmpty(m_pTipText))
            m_pProgressText.text = string.Format(m_pTipText, (m_pProgress * 100f).ToString("F2"));
    }

    public void Show() {
        gameObject.SetActivityExt(true);
        m_pProgress = -1;
        m_pTipText = string.Empty;
        GameMain.AddEventListener(CommonEvents.UpdateCheckVersionProgress,OnUpdateCheckVersionProgress);
        m_pAlertOKButton.onClick.AddListener(OnAlertOKClick);
    }

    public void Hide() {
        gameObject.SetActivityExt(false);
        GameMain.RemoveEventListener(CommonEvents.UpdateCheckVersionProgress, OnUpdateCheckVersionProgress);
        m_pAlertOKButton.onClick.AddListener(OnAlertOKClick);
    }

    private void OnAlertOKClick() {
        m_pAlertUI.gameObject.SetActivityExt(false);
        if (m_pAlertCallback != null)
            m_pAlertCallback.Invoke();
        m_pAlertCallback = null;
    }

    public void ShowAlert(string pText, Action pCallback) {
        m_pAlertUI.gameObject.SetActivityExt(true);
        m_pAlertText.text = pText;
        m_pAlertCallback = pCallback;
    }

    private void OnUpdateCheckVersionProgress(BaseEvent evt) {
        if (evt.EventObjects == null || evt.EventObjects.Length < 2)
            return;
        m_pProgress = (float)evt.EventObjects[0];
        m_pTipText = (string)evt.EventObjects[1];

    }
}
