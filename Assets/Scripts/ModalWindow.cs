using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;
using System;

public class ModalWindow : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI headerText;

    [SerializeField] private GameObject buttonContainer;
    private GameObject[] buttonsObject;

    [SerializeField] private Transform window;
    [SerializeField] private CanvasGroup backgroud;


    public void Initialize(string HeaderText, string[] buttonsText, Action<string>[] buttonAction, string[] message)
    {
        headerText.text = HeaderText;


        int nButtons = buttonsText.Length;
        if (nButtons > 0)
        {
            //load the button prefab
            Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Button.prefab").Completed +=
            (asyncOperationHandle) => {
                if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    buttonsObject = new GameObject[nButtons];
                    for (int i = 0; i < nButtons; i++)
                    {
                        GameObject button = Instantiate(asyncOperationHandle.Result, buttonContainer.transform);
                        button.GetComponentInChildren<TextMeshProUGUI>().text = buttonsText[i];

                        int index = i; //Not sure why but doesn't work without this line
                        button.GetComponent<Button>().onClick.AddListener(() => buttonAction[index](message[index]));


                        buttonsObject[i] = button;
                    }

                    //set circular button navigation
                    for (int i = 0; i < nButtons; i++)
                    {
                        var navigation = buttonsObject[i].GetComponent<Button>().navigation;
                        navigation.mode = Navigation.Mode.Explicit;

                        navigation.selectOnRight = buttonsObject[(i + 1) % nButtons].GetComponent<Button>();
                        navigation.selectOnLeft = buttonsObject[(i + nButtons - 1) % nButtons].GetComponent<Button>();


                        buttonsObject[i].GetComponent<Button>().navigation = navigation;
                    }

                    buttonsObject[0].GetComponent<Button>().Select();

                }
                else
                    Debug.Log("Error loading the button prefab");

            };
        }
        else
        {
            buttonContainer.SetActive(false);
        }

        //change the size and alpha to recover it with the animation
        window.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        backgroud.alpha = 0;
        openAnimation();
    }

    public void closeModal()
    {
        closeAnimation();
        Destroy(gameObject, 0.2f);
    }

    //deselect all modal butons, it's called in the changeColor action
    public void unableButtons()
    {
        foreach (GameObject button in buttonsObject)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void openAnimation()
    {
        window.LeanScale(new Vector3(1f, 1f, 0), 0.2f);
        backgroud.LeanAlpha(1, 0.2f);
    }

    public void closeAnimation(){
        window.LeanScale(new Vector3(1.2f, 1.2f, 0), 0.2f);
        backgroud.LeanAlpha(0, 0.2f);
    }
}