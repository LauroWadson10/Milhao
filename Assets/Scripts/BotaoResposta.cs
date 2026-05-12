using TMPro;
using UnityEngine;

public class BotaoResposta : MonoBehaviour
{
    string resposta;
    public int indiceResposta = -1;

    public void RegistraResposta(string resp) // Sobrecarga para resposta sem índice
    {
        RegistraResposta(resp, -1);
    }

    public void RegistraResposta(string resp, int indice) // Sobrecarga para resposta com índice
    {
        resposta = resp;
        indiceResposta = indice;

        TMP_Text texto = GetComponentInChildren<TMP_Text>();

        if (texto != null)
        {
            texto.text = resp;
        }
    }

    public void Responde() // Método chamado ao clicar no botão
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

        QuestionManager.instance.ConfirmaResposta(int.Parse(resposta));
    }
}
