/* 
 * 
 * 
 * Universal Pop Ups (new)
 * 
 * Make Sure Canvas is existing in your scene then thats it. you can call the functions here
 * 
 * */

using System;
using UnityEngine;

public static class UniversalPopUps
{
    #region dialogue input 
    public static void ShowOkCancelPopUp(string p_message, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkCancelPopUp(p_message, p_title, p_messageType, p_yesAction, p_cancelAction);
    }

    public static void ShowOkCancelPopUp(string[] p_messages, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_yesAction, Action p_cancelAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkCancelPopUp(p_messages, p_title, p_messageType, p_yesAction, p_cancelAction);
    }

    public static void ShowOkMessage(string p_message, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkMessage(p_message, p_title, p_messageType, p_okAction);
    }

    public static void ShowOkMessage(string[] p_messages, string p_title, UniversalDialogueInputPopUp.MESSAGE_TYPE p_messageType, Action p_okAction = null)
    {
        CheckDialogueInputInstance();
        UniversalDialogueInputPopUp.Instance.ShowOkMessage(p_messages, p_title, p_messageType, p_okAction);
    }

    private static void CheckDialogueInputInstance()
    {
        if (UniversalDialogueInputPopUp.Instance == null)
        {
            GameObject.Instantiate(Resources.Load("UniversolPopUps/UniversalDialogueInputPopUp"));
        }
    }
    #endregion
}
