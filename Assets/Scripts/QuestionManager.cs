using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Range(0, 5.0f)]
    [SerializeField] float tempoParaAvancar;
    [SerializeField] bool avancarAutomaticamente = true;

    NivelQuestao nivelAtual = NivelQuestao.Facil;

    int pontuacao;
    int indiceQuestao;
    bool respostaConfirmada;
    readonly List<int> questoesRespondidas = new();
    Coroutine rotinaAvancar;

    public static QuestionManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        IniciarJogo();
    }

    public void IniciarJogo()
    {
        pontuacao = 0;
        nivelAtual = NivelQuestao.Facil;
        questoesRespondidas.Clear();
        GeradorDeQuestao();
    }

    void GeradorDeQuestao()
    {
        ConstroiQuestao();
    }

    void ConstroiQuestao()
    {
        if (!TentaPegarQuestao(out indiceQuestao))
        {
            //FinalizaJogo("Voce respondeu todas as perguntas!");
            //return;
        }

        questaoAtual = questao[indiceQuestao];
        respostaConfirmada = false;

        AtualizaTextoQuestao();
        //AtualizaImagens();
        //AtualizaAjuda();
        ConfiguraBotoes();
    }

    bool TentaPegarQuestao(out int indice)
    {
        indice = -1;

        if (questao == null || questao.Count == 0)
        {
            return false;
        }

        while (true)
        {
            List<int> indicesDisponiveis = new();

            for (int i = 0; i < questao.Count; i++)
            {
                QuestaoSO pergunta = questao[i];

                if (pergunta == null || !pergunta.EstaValida())
                {
                    continue;
                }

                if (pergunta.Nivel == nivelAtual && !questoesRespondidas.Contains(i))
                {
                    indicesDisponiveis.Add(i);
                }
            }

            if (indicesDisponiveis.Count > 0)
            {
                indice = indicesDisponiveis[Random.Range(0, indicesDisponiveis.Count)];
                questoesRespondidas.Add(indice);
                return true;
            }


            if (!AumentarDificuldade())
            {
                return false;
            }
        }
    }

    bool AumentarDificuldade()
    {
        int proximoNivel = (int)nivelAtual + 1;

        if (proximoNivel > (int)NivelQuestao.ValeTudo)
        {
            return false;
        }

        nivelAtual = (NivelQuestao)proximoNivel;
        return true;
    }
    void AtualizaTextoQuestao()
    {
        if (txtQuestao != null)
        {
            txtQuestao.text = questaoAtual.txtQuestao;
        }
    }

    //void AtualizaImagens()
    //{
    //    if (imgQuestao == null)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < imgQuestao.Length; i++)
    //    {
    //        Image imagem = imgQuestao[i];

    //        if (imagem == null)
    //        {
    //            continue;
    //        }

    //        bool temImagem = questaoAtual.TemImagem(i);
    //        imagem.gameObject.SetActive(temImagem);
    //        imagem.sprite = temImagem ? questaoAtual.imgQuestao[i] : null;
    //    }
    //}

    //void AtualizaAjuda()
    //{
    //    if (txtAjuda != null)
    //    {
    //        txtAjuda.text = questaoAtual.TemAjuda ? questaoAtual.Ajuda : string.Empty;
    //    }
    //}

    void ConfiguraBotoes()
    {
        for (int i = 0; i < btnOpcoes.Length; i++)
        {
            Button botao = btnOpcoes[i];

            if (botao == null)
            {
                continue;
            }

            bool temOpcao = i < questaoAtual.QuantidadeOpcoes;
            botao.gameObject.SetActive(temOpcao);
            botao.interactable = temOpcao;

            if (!temOpcao)
            {
                continue;
            }

            BotaoResposta botaoResposta = botao.GetComponent<BotaoResposta>();

            if (botaoResposta != null)
            {
                botaoResposta.RegistraResposta(questaoAtual.opcao[i], i);
                continue;
            }

            TMP_Text textoBotao = botao.GetComponentInChildren<TMP_Text>();

            if (textoBotao != null)
            {
                textoBotao.text = questaoAtual.opcao[i];
            }
        }
    }

    public void ConfirmaResposta(string resposta)
    {
        if (questaoAtual == null || questaoAtual.opcao == null)
        {
            return;
        }

        ConfirmaResposta(questaoAtual.opcao.IndexOf(resposta));
    }

    public void ConfirmaResposta(int indiceOpcao)
    {
        SelecionarOpcao(indiceOpcao);
    }

    public void SelecionarOpcao(int indiceOpcao)
    {
        if (questaoAtual == null || respostaConfirmada)
        {
            return;
        }

        respostaConfirmada = true;
        bool acertou = questaoAtual.EhOpcaoCorreta(indiceOpcao);

        DefineBotoesInterativos(false);

        if (acertou)
        {
            //AtualizaPontuacao();
            //AtualizaFeedback("Resposta correta!");

            //if (nivelAtual == NivelQuestao.ValeTudo)
            //{
            //    FinalizaJogo("Parabens, voce chegou ao fim!");
            //    return;
            //}

            AumentarDificuldade();
            AgendaProximaQuestao();
            return;
        }

        //string respostaCorreta = questaoAtual.OpcaoCorreta();
        //AtualizaFeedback(string.IsNullOrEmpty(respostaCorreta)
        //    ? "Resposta errada."
        //    : $"Resposta errada. Correta: {respostaCorreta}");
    }

    void AgendaProximaQuestao()
        {
            if (!avancarAutomaticamente)
            {
                return;
            }

            if (rotinaAvancar != null)
            {
                StopCoroutine(rotinaAvancar);
            }

            rotinaAvancar = StartCoroutine(AvancaDepoisDoTempo());
        }

        IEnumerator AvancaDepoisDoTempo()
        {
            yield return new WaitForSeconds(Mathf.Max(0.1f, tempoParaAvancar));
            rotinaAvancar = null;
            ConstroiQuestao();
        }

    void DefineBotoesInterativos(bool ativo)
    {
        if (btnOpcoes == null)
        {
            return;
        }

        foreach (Button botao in btnOpcoes)
        {
            if (botao != null && botao.gameObject.activeSelf)
            {
                botao.interactable = ativo;
            }
        }
    }

    //void AtualizaPontuacao()
    //{
    //    if (txtPontuacao != null)
    //    {
    //        txtPontuacao.text = $"Pontuacao: {pontuacao}";
    //    }
    //}

    //void AtualizaFeedback(string mensagem)
    //{
    //    if (txtFeedback != null)
    //    {
    //        txtFeedback.text = mensagem;
    //    }
    //}



    //void FinalizaJogo(string mensagem)
    //{
    //    AtualizaFeedback(mensagem);
    //    DefineBotoesInterativos(false);
    //}
}



