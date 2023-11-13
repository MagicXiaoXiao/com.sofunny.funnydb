using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoFunny.FunnyDB;
using SoFunny.FunnySDK.UIModule;

public class SetUserIDCell : MonoBehaviour
{
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUserID() {
        FunnyDBSDK.SetUserId(inputField.text);
        Toast.Show("用户 ID 设置完毕");
    }

    public void RandomValue() {
        inputField.text = Random.Range(10000000, 99999999).ToString();
    }

}
