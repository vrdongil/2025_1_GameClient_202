using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { get; private set; }

    [Header("Dialog References")]
    [SerializeField] private DialogDatabaseSO dialogDatabase;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image portraitImage;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTypewriterEffect = true;

    [Header("Dialog Settings")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject choicesButtonPrefeb;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private DialogSO currentDialog;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (dialogDatabase != null)
        {
            dialogDatabase.Initialize();
        }
        else
        {
            Debug.LogError("Dialog Database is not assinged to Dialog Manager");
        }
        if(NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);
        }
        else
        {
            Debug.LogError("Next Button is not assigned");
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        CloseDialog();
        StartDialog(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialog(int dialogid)
    {
        DialogSO dialog = dialogDatabase.GetDialogByld(dialogid);
        if(dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialog with ID {dialogid} not found!");
        }
    }

    public void StartDialog(DialogSO dialog)
    {
        if (dialog == null) return;

        currentDialog = dialog;
        ShowDialog();
        dialogPanel.SetActive(true);

    }
    public void ShowDialog()
    {
        if (currentDialog == null) return;
        characterNameText.text = currentDialog.characterName;

        if (useTypewriterEffect)
            {
            StartTypingEffect(currentDialog.text);
            }
        else
        {
            dialogText.text = currentDialog.text;
        }
            

        if(currentDialog.portrait != null)
        {
            portraitImage.sprite = currentDialog.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if(!string.IsNullOrEmpty(currentDialog.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentDialog.portraitPath);
            if(portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"Portrait not found at path : {currentDialog.portraitPath}");
                portraitImage.gameObject.SetActive(false);
            }
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        ClearChoices();
        if (currentDialog.choices != null && currentDialog.choices.Count > 0)
        {
            ShowChoices();
            NextButton.gameObject.SetActive(false);
        }
        else
        {
            NextButton.gameObject.SetActive(true);
        }
    }

    public void NextDialog()
    {
        
        if(isTyping)
        {
            StopTypingEffect();
            dialogText.text = currentDialog.text;
            isTyping = false;
            return;
        }
        
        if(currentDialog != null && currentDialog.nextId > 0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogByld(currentDialog.nextId);
            if (nextDialog != null)
            {
                currentDialog = nextDialog;
                ShowDialog();

            }
            else
            {
                CloseDialog();
            }
        }
        else 
        {
            CloseDialog();
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach(char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void StopTypingEffect()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private void StartTypingEffect(string text)
    {
        isTyping = true;
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }
    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        currentDialog = null;
        StopTypingEffect();
    }

    private void ClearChoices()
    {
        foreach(Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(false);
    }

    public void SelectChoice(DialogChoiceSO choice)
    {
        if(choice != null && choice.nextId >0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogByld(choice.nextId);
            if(nextDialog != null)
            {
                currentDialog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    private void ShowChoices()
    {
        choicesPanel.SetActive(true);
        
        foreach(var choice in currentDialog.choices)
        {
            GameObject choiceGO = Instantiate(choicesButtonPrefeb, choicesPanel.transform);
            TextMeshProUGUI buttonText = choiceGO.GetComponentInChildren<TextMeshProUGUI>();
            Button button = choiceGO.GetComponent<Button>();

            if(buttonText != null)
            {
                buttonText.text = choice.text;
            }
            if(button != null)
            {
                DialogChoiceSO choiceSO = choice;
                button.onClick.AddListener(() => SelectChoice(choiceSO));
            }
        }
    }
}
