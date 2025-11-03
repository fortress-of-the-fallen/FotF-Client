using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {
    [SerializeField] TMP_InputField inputUser;
    [SerializeField] TMP_InputField inputPass;
    [SerializeField] Button btnLogin;
    [SerializeField] TMP_Text errorText;

    public void OnClickLogin() {
        if (string.IsNullOrEmpty(inputUser.text) || string.IsNullOrEmpty(inputPass.text)) {
            errorText.text = "Missing username or password";
            errorText.enabled = true; return;
        }
        // call auth here
    }
}
