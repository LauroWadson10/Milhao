using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static QuestaoSO;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] TMP_Text txtQuestao;
    [SerializeField] Image[] imgQuestao = new Image[2];
    [Space]
    [Header("UI")]
    [SerializeField] Button[] btnOpcoes = new Button[4];
    [Space]
    [Header("Textos")]
    [SerializeField] TMP_Text txtAjuda;
    [SerializeField] TMP_Text txtFeedback;
    [SerializeField] TMP_Text txtPontuacao;
    [Space]
    [SerializeField] List<QuestaoSO> questao = new();

    QuestaoSO questaoAtual;

    [Space]
    [Range(0,5.0f)]
    [SerializeField] float tempoParaAvancar;

    NivelQuestao nivelAtual = NivelQuestao.Facil;

    int pontuacao;

    int indiceQuestao;

    public static QuestionManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GeradorDeQuestao();
    }

    void GeradorDeQuestao()
    {
        do
        {
            indiceQuestao = Random.Range(0,questao.Count);
        } while (questao[indiceQuestao].Nivel != nivelAtual);

        txtQuestao.text = questao[indiceQuestao].txtQuestao;

        for(int i = 0; i < btnOpcoes.Length; i++)
        {
            btnOpcoes[i].GetComponent<BotaoResposta>().RegistraResposta(questao[indiceQuestao].opcao[i]);
        }
    }

    public void ConfirmaResposta(string resposta)
    {
        if(resposta == questao[indiceQuestao].respostaCorreta)
        {
            print("Resposta correta");
        }
        else
        {
            print("Resposta errada");
        }
    }

    //int PegaQuestaoPorNivel()
    //{
    //    for (int i = 0; i < questoesDisponiveis.Count; ++i)
    //    {
    //        int indiceQuestao = questoesDisponiveis[i];

    //        if (questao[indiceQuestao].Nivel == nivelAtual)
    //        {
    //            questoesDisponiveis.RemoveAt(i);
    //            return indiceQuestao;
    //        }
    //    }

    //    return 0;
    //}

    public void SelecionarOpcao(int indiceOpcao)
    {
        if (questaoAtual == null)
        {
            return;
        }

        bool acertou = questaoAtual.EhOpcaoCorreta(indiceOpcao);

        if (acertou)
        {
            pontuacao++;
            AumentarDificuldade();
        }

        //AtualizaPontuacao();
        //AtualizaFeedback(acertou, questaoAtual.IndiceCorreto);
        //DefineBotoesInterativos(false);

        //if (avancarAutomaticamente)
        //{
        //    Invoke(nameof(ConstroiQuestao), Mathf.Max(0.1f, tempoParaAvancar));
        //}
    }

    void ConstroiQuestao()
    {

    }

    void AumentarDificuldade()
    {
        nivelAtual++;
    }
} 