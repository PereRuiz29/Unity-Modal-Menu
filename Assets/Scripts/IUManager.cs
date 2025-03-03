using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class IUManager : MonoBehaviour
{

    [SerializeField] private Button[] mainMenuButtons;
    [SerializeField] private AssetReference assetModalWindow;

    //Buttons list, with the button action, the button text, and a message
    //The message is the text that pops up in the debug action, and the color that pops up in the action changes color,
    //with the close action doesn't do  anything.
    public enum customAction { Close, ChangeColor, Debug };

    [System.Serializable]
    public struct myButton
    {
        public customAction buttonAction;
        public string buttonText;
        public string message;
    }


    [Header("---Model 1---")]
    [SerializeField] private string headerText1;
    [SerializeField] private myButton[] buttonsModal1;

    [Space(10)]
    [Header("---Model 2---")]
    [SerializeField] private string headerText2;
    [SerializeField] private myButton[] buttonsModal2;

    [Space(10)]
    [Header("---Model 3---")]
    [SerializeField] private string headerText3;
    [SerializeField] private myButton[] buttonsModal3;


    private GameObject modalWindow;
    private int lastButonSelected;
    bool canCloseWithSubmit = false;


    #region CreateModal
    public void CreateModal1()
    {
        CreateModal(buttonsModal1, headerText1);
        lastButonSelected = 0;
    }

    public void CreateModal2()
    {
        CreateModal(buttonsModal2, headerText2);
        lastButonSelected = 1;
    }

    public void CreateModal3()
    {
        CreateModal(buttonsModal3, headerText3);
        lastButonSelected = 2;
    }

    public void CreateModal(myButton[] buttonsModal, string headerText)
    {
        Action<string>[] buttonsAction = new Action<string>[buttonsModal.Length];
        string[] buttonsText = new string[buttonsModal.Length];
        string[] message = new string[buttonsModal.Length];

        if (buttonsModal.Length == 0) canCloseWithSubmit = true;

        for (int i = 0; i < buttonsModal.Length; i++)
        {
            buttonsText[i] = buttonsModal[i].buttonText;
            message[i] = buttonsModal[i].message;

            switch (buttonsModal[i].buttonAction)
            {
                case customAction.Close:
                    buttonsAction[i] = CloseAction;
                    break;
                case customAction.ChangeColor:
                    buttonsAction[i] = ChangeColorAction;
                    break;
                case customAction.Debug:
                    buttonsAction[i] = DebugAction;
                    break;
            }
        }


        assetModalWindow.LoadAssetAsync<GameObject>().Completed +=
        (asyncOperationHandle) => {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                modalWindow = Instantiate(asyncOperationHandle.Result);
                modalWindow.GetComponent<ModalWindow>().Initialize(headerText, buttonsText, buttonsAction, message);
                unableMainMenuBotons();
            }
            else
                Debug.Log("Error loading the ModalWindow prefab");
        };
    }
    #endregion

    #region Actions
    private void CloseAction(string unuse)
    {
        closeModalWindow();
    }

    private void ChangeColorAction(string color)
    {
        Color colorText;
        if (!ColorUtility.TryParseHtmlString(color, out colorText))
            colorText = UnityEngine.Random.ColorHSV();


        modalWindow.transform.Find("Container/Header/HeaderText").GetComponent<TextMeshProUGUI>().color = colorText;

        Invoke("closeModalWindow", 2f);
        modalWindow.GetComponent<ModalWindow>().unableButtons();
    }

    private void DebugAction(string message)
    {
        Debug.Log(message);
    }
    #endregion


    private void onableMainMenuBotons()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = true;
        }
        mainMenuButtons[lastButonSelected].Select();
    }

    private void unableMainMenuBotons()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = false;
        }
    }
    public void closeModalWindow()
    {
        modalWindow.GetComponent<ModalWindow>().closeModal();
        canCloseWithSubmit = false;

        //release assets
        Addressables.ReleaseInstance(modalWindow);
        assetModalWindow.ReleaseAsset();

        Invoke("onableMainMenuBotons", 0.01f);
    }

    //close window without buttons on submit
    public void onSubmit(InputAction.CallbackContext context)
    {
        if (context.performed && canCloseWithSubmit)
        {
            closeModalWindow();
        }
    }

    public void closeGame()
    {
        Application.Quit();
    }
}
