using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotaoResposta : MonoBehaviour
{
    [SerializeField] Image imgCorrecao;
    string resposta;

    public void Responde() // Método chamado ao clicar no botão
    {
        resposta = GetComponentInChildren<TMP_Text>().text;
        QuestionManager.instance.QuestionaResposta(resposta, imgCorrecao);
    }
}
