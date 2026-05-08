using TMPro;
using UnityEngine;

public class BotaoResposta : MonoBehaviour
{
    string resposta;
    int indiceResposta = -1;

    public void RegistraResposta(string resp)
    {
        RegistraResposta(resp, -1);
    }

    public void RegistraResposta(string resp, int indice)
    {
        resposta = resp;
        indiceResposta = indice;

        TMP_Text texto = GetComponentInChildren<TMP_Text>();

        if (texto != null)
        {
            texto.text = resp;
        }
    }

    public void Responde()
    {
        if (QuestionManager.instance == null)
        {
            return;
        }

        if (indiceResposta >= 0)
        {
            QuestionManager.instance.ConfirmaResposta(indiceResposta);
            return;
        }

        QuestionManager.instance.ConfirmaResposta(resposta);
    }
}
