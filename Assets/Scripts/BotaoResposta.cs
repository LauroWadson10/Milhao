using TMPro;
using UnityEngine;

public class BotaoResposta : MonoBehaviour
{
    string resposta;

    public void RegistraResposta(string resp)
    {
        resposta = resp;
        GetComponentInChildren<TMP_Text>().text = resp;
    }

    public void Responde()
    {
        QuestionManager.instance.ConfirmaResposta(resposta);
    }
}
