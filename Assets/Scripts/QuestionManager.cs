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

    [SerializeField] Button[] btnOpcoes = new Button[4];
    [SerializeField] TMP_Text[] txtOpcoes = new TMP_Text[4];

    [SerializeField] TMP_Text txtAjuda;
    [SerializeField] TMP_Text txtFeedback;
    [SerializeField] TMP_Text txtPontuacao;

    [SerializeField] List<QuestaoSO> questao = new();

    [SerializeField] bool construirAoIniciar = true;
    [SerializeField] bool embaralharQuestoes = true;
    [SerializeField] bool avancarAutomaticamente = true;

    NivelQuestao nivelAtual = NivelQuestao.Facil;

    readonly List<int> questoesDisponiveis = new();

    //void Start()
    //{
    //    if (construirAoIniciar)
    //    {
    //        ReiniciarJogo();
    //    }
    //}
    
    ////public void ReiniciarJogo()
    //{
    //    pontuacao = 0;
    //    nivelAtual = NivelQuestao.Facil;

    //    PrepararQuestoesDisponives();
    //    AtualizaPontuacao();
    //    ConstruirProximaQuestao();
        
    //}

    //void PrepararQuestoesDisponives()
    //{
    //    questoesDisponiveis.Clear();
    //    if (questao == null)
    //    {
    //        return; 
    //    }

    //    for (int i = 0; i < questao.Count; i++)
    //    {
    //        if (questao[i] != null)
    //        {
    //            questoesDisponiveis.Add(i);
    //        }
    //    }
    //}

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
    //}

    //public void SelecionarOpcao(int indiceOpcao)
    //{
    //    if (questaoAtual == null)
    //    {
    //        return;
    //    }

    //    bool acertou = questaoAtual.EhOpcaoCorreta(indiceOpcao);

    //    if (acertou)
    //    {
    //        pontuacao++;
    //        AumentarDificuldade();
    //    }

    //    AtualizaPontuacao();
    //    AtualizaFeedback(acertou, questaoAtual.IndiceCorreto);
    //    DefineBotoesInterativos(false);

    //    if (avancarAutomaticamente)
    //    {
    //        Invoke(nameof(ConstroiQuestao), Mathf.Max(0.1f, tempoParaAvancar));
    //    }
    //}

    //void AumentarDificuldade()
    //{
    //    if (nivelAtual == NivelQuestao.Facil)
    //    {
    //        nivelAtual = NivelQuestao.Medio;
    //    }
    //    else if (nivelAtual == NivelQuestao.Medio)
    //    {
    //        nivelAtual = NivelQuestao.Dificil;
    //    }
    //}


} 