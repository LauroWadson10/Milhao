using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] TMP_Text txtQuestao;
    [SerializeField] Image[] imgQuestao = new Image[2];
    [SerializeField] List<QuestaoSO> questao = new();


    void ConstroiQuestao()
    {
        int x = Random.Range(0, questao.Count);

        txtQuestao.text = questao[x].txtQuestao;

        if (questao[x].imgQuestao != null)
        {   
            imgQuestao[0].sprite = questao[x].imgQuestao[0];
            imgQuestao[1].sprite = questao[x].imgQuestao[1];
        }

    }


}