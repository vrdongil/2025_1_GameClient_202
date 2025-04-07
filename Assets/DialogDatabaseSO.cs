using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogDatabase", menuName = "Dialog System/Database")]

public class DialogDatabaseSO : ScriptableObject
{
    // Start is called before the first frame update

    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<int, DialogSO> dialogsByld;

    public void Initialize()
    {
        dialogsByld = new Dictionary<int, DialogSO>();

        foreach(var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogsByld[dialog.id] = dialog;
            }
        }
    }

    public DialogSO GetDialogByld(int id)
    {
        if (dialogsByld == null)
            Initialize();
        if (dialogsByld.TryGetValue(id, out DialogSO dialog))
            return dialog;

        return null;
    }
}
