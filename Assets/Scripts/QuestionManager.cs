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

    [Header("Painel de confirmação")]
    [SerializeField] private GameObject painelConfirmacao;
    [SerializeField] private Button btnSim;
    [SerializeField] private Button btnNao;

    [Header("Painel de ajuda")]
    [SerializeField] private Button FecharAjuda;
    [SerializeField] private GameObject painelAjuda;
    [SerializeField] private Button btnAjuda;

    [Header("Painel de pular questão")]
    [SerializeField] private Button btnPularQuestao;

    [Space]
    [Header("Textos")]
    [SerializeField] TMP_Text txtAjuda;
    [SerializeField] TMP_Text txtFeedback;
    [SerializeField] TMP_Text txtPontuacao;
    [SerializeField] TMP_Text QntAjuda;
    [SerializeField] TMP_Text QntPularQuestao;


    [Space]
    [SerializeField] List<QuestaoSO> questao = new();

    QuestaoSO questaoAtual;

    [Space]
    [Range(0, 5.0f)]
    [SerializeField] float tempoParaAvancar;
    [SerializeField] bool avancarAutomaticamente = true; //temporariamente
    [SerializeField] int perguntasPorNivel = 5;
    [SerializeField] int AjudaPorNivel = 3;
    [SerializeField] int PularQuestaoPorNivel = 3;

    NivelQuestao nivelAtual = NivelQuestao.Facil;

    int pontosPorAcerto = 0; //temporariamente
    int pontuacao;
    int indiceQuestao; //índice da questão atual dentro da lista de questões
    bool respostaConfirmada; //para que ele não permita clicar em mais de uma resposta
    int contadorPerguntasNivel = 0;
    int contadorAjuda; //contador para limitar o uso da ajuda, se necessário
    readonly List<int> questoesRespondidas = new(); //para evitar repetir questões já respondidas
    Coroutine rotinaAvancar; //para controlar a rotina de avançar automaticamente para a próxima questão 
    int indiceOpcaoSelecionada = -1; //índice da opção selecionada pelo jogador, para confirmar a resposta correta ou errada
    int contadorPularQuestao = 3; //contador para limitar o uso do pular questão, se necessário

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
        pontuacao = 0; //temporariamente
        nivelAtual = NivelQuestao.Facil;
        questoesRespondidas.Clear(); //limpa a lista de questões respondidas para começar do zero
        ConstroiQuestao();
        contadorPerguntasNivel = 0;
        contadorPularQuestao = PularQuestaoPorNivel;
        contadorAjuda = AjudaPorNivel;
    }
    void ConstroiQuestao()
    {
        if (!TentaPegarQuestao(out indiceQuestao))
        {
            FinalizaJogo("Voce respondeu todas as perguntas!");
            return;
        }

        
        questaoAtual = questao[indiceQuestao];
        respostaConfirmada = false;
        indiceOpcaoSelecionada = -1;
        painelConfirmacao.SetActive(false);
        DefineBotoesInterativos(true);
        painelConfirmacao.SetActive(false);
        btnSim.onClick.RemoveAllListeners();
        btnNao.onClick.RemoveAllListeners();
        btnSim.onClick.AddListener(ConfirmaResposta);
        btnNao.onClick.AddListener(CancelaResposta);
        btnAjuda.onClick.RemoveAllListeners();
        btnAjuda.onClick.AddListener(MostraAjuda);
        FecharAjuda.onClick.RemoveAllListeners();
        FecharAjuda.onClick.AddListener(EscondeAjuda);
        btnPularQuestao.interactable = true;
        btnPularQuestao.onClick.RemoveAllListeners();
        btnPularQuestao.onClick.AddListener(PularQuestao);
        btnAjuda.interactable = true;


        ControlaQntPularQuestao();
        AtualizaTextoQuestao();
        //AtualizaImagens();
        AtualizaAjuda();
        ConfiguraBotoes();

    }

    bool TentaPegarQuestao(out int indice) //tenta pegar uma questão válida e disponível, se conseguir retorna true e o índice da questão, caso contrário retorna false
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

                if (pergunta == null || !pergunta.EstaValida()) //verifica se a questão é nula ou inválida, e se for, ignora ela
                {
                    continue;
                }

                if (pergunta.Nivel == nivelAtual && !questoesRespondidas.Contains(i)) //verifica se a questão é do nível atual e se ela ainda não foi respondida, e se for, adiciona ela à lista de disponíveis
                {
                    indicesDisponiveis.Add(i);
                }
            }

            if (indicesDisponiveis.Count > 0) //se tiver questões disponíveis para o nível atual, escolhe uma aleatoriamente
            {
                indice = indicesDisponiveis[Random.Range(0, indicesDisponiveis.Count)];
                questoesRespondidas.Add(indice);
                return true;
            }

            //se não tiver questões disponíveis para o nível atual, tenta aumentar a dificuldade para ver se tem questões disponíveis para o próximo nível, e se não tiver mais níveis para aumentar, retorna false
            if (!AumentarDificuldade())
            {
                return false;
            }
        }
    }
    void ControlaAvancoNivel() //controla o avanço de nível, aumentando o contador de perguntas respondidas no nível atual, e se atingir o limite de perguntas por nível, reseta o contador e aumenta a dificuldade
    {
        contadorPerguntasNivel++;
        if (contadorPerguntasNivel >= perguntasPorNivel)
        {
            contadorPerguntasNivel = 0;
            AumentarDificuldade();
        }
    }
    bool AumentarDificuldade() //tenta aumentar a dificuldade para o próximo nível, se tiver um próximo nível disponível, retorna true, caso contrário retorna false
    {
        int proximoNivel = (int)nivelAtual + 1;

        if (proximoNivel > (int)NivelQuestao.ValeTudo)
        {
            return false;
        }

        nivelAtual = (NivelQuestao)proximoNivel;
        return true;
    }
    void AtualizaTextoQuestao() //atualiza o texto da questão na interface, verificando se o componente de texto existe antes de tentar atualizar, para evitar erros caso ele não esteja configurado
    {
        if (txtQuestao != null)
        {
            txtQuestao.text = questaoAtual.txtQuestao;
        }
    }
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
    //sobrecarga do método para confirmar a resposta, permitindo passar a resposta selecionada como string, e encontrando o índice correspondente na lista de opções da questão atual antes de chamar a função para processar a resposta selecionada
    public void ConfirmaResposta()
    {
        if (questaoAtual == null || respostaConfirmada || indiceOpcaoSelecionada < 0)
        {
            return;
        }

        painelConfirmacao.SetActive(false);

        respostaConfirmada = true;

        bool acertou = questaoAtual.EhOpcaoCorreta(indiceOpcaoSelecionada);

        DefineBotoesInterativos(false);

        if (acertou)
        {
            pontuacao += pontosPorAcerto;
            AtualizaPontuacao();
            AtualizaFeedback("Resposta correta!");

            ControlaAvancoNivel();

            AgendaProximaQuestao();
            return;
        }

        AtualizaFeedback("Resposta errada!");
    }

    public void CancelaResposta()
    {
        indiceOpcaoSelecionada = -1;
        painelConfirmacao.SetActive(false);
    }

    public void ConfirmaResposta(int indiceOpcao) //sobrecarga do método para confirmar a resposta, permitindo passar o índice da opção selecionada diretamente, e chamando a função para processar a resposta selecionada
    {
        SelecionarOpcao(indiceOpcao);
    }

    public void SelecionarOpcao(int indiceOpcao)
    {
        if (questaoAtual == null || respostaConfirmada)
        {
            return;
        }

        indiceOpcaoSelecionada = indiceOpcao;
        painelConfirmacao.SetActive(true);
    }

    public void AgendaProximaQuestao() //agenda a próxima questão para ser construída depois de um tempo, verificando se o avanço automático está habilitado antes de iniciar a rotina, para evitar que o jogo avance automaticamente se essa opção estiver desativada
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

    IEnumerator AvancaDepoisDoTempo() //rotina para avançar automaticamente para a próxima questão depois de um tempo, aguardando o tempo configurado antes de chamar a função para construir a próxima questão
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, tempoParaAvancar));
        rotinaAvancar = null;
        ConstroiQuestao();
    }

    void DefineBotoesInterativos(bool ativo) //ativa ou desativa a interatividade dos botões de resposta, verificando se o array de botões existe antes de tentar acessar, para evitar erros caso ele não esteja configurado
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

    void AtualizaPontuacao() //atualiza o texto de pontuação na interface, verificando se o componente de texto existe antes de tentar atualizar, para evitar erros caso ele não esteja configurado
    {
        if (txtPontuacao != null)
        {
            txtPontuacao.text = $"Pontuacao: {pontuacao}";
        }
    }

    void AtualizaFeedback(string mensagem)//atualiza o texto de feedback na interface, verificando se o componente de texto existe antes de tentar atualizar, para evitar erros caso ele não esteja configurado
    {
        if (txtFeedback != null)
        {
            txtFeedback.text = mensagem;
        }
    }

    void FinalizaJogo(string mensagem)//finaliza o jogo, mostrando uma mensagem de feedback e desativando os botões para evitar que o jogador continue interagindo
    {
        AtualizaFeedback(mensagem);
        DefineBotoesInterativos(false);
    }

    //atualiza o texto de ajuda na interface, verificando se o componente de texto existe antes de tentar atualizar, para evitar erros caso ele não esteja configurado, e mostrando a ajuda apenas se a questão atual tiver uma ajuda disponível
    void AtualizaAjuda()
    {
        if (txtAjuda != null)
        {
            txtAjuda.text = questaoAtual.TemAjuda ? questaoAtual.Ajuda : string.Empty;
        }
    }

    void MostraAjuda() //mostra o painel de ajuda, verificando se a questão atual tem uma ajuda disponível antes de mostrar o painel, para evitar mostrar um painel vazio ou irrelevante
    {
        if (questaoAtual == null || !questaoAtual.TemAjuda)
        {
            return;
        }

        contadorAjuda--;
        ControlaQntAjuda();
        painelAjuda.SetActive(true);
        FecharAjuda.onClick.RemoveAllListeners();
        FecharAjuda.onClick.AddListener(EscondeAjuda);

    }

    void EscondeAjuda() //esconde o painel de ajuda, verificando se o painel está ativo antes de tentar esconder, para evitar erros caso ele já esteja escondido
    {
        if (painelAjuda != null && painelAjuda.activeSelf)
        {
            painelAjuda.SetActive(false);
        }
    }
    void ControlaQntAjuda()//controla a quantidade de ajudas disponíveis, atualizando o texto correspondente e desativando o botão de ajuda se o contador chegar a zero, para evitar que o jogador tente usar mais ajudas do que o permitido
    {
        if (QntAjuda != null)
        {
            QntAjuda.text = $"{contadorAjuda}";
        }

        if (contadorAjuda < 0)
        {
            contadorAjuda = 0;
        }

        if (contadorAjuda <= 0)
        {
            btnAjuda.interactable = false;
        }
    }
    void ControlaQntPularQuestao()
    {
        if (QntPularQuestao != null)
        {
            QntPularQuestao.text = $"{contadorPularQuestao}";
        }

        if (contadorPularQuestao <= 0)
        {
            btnPularQuestao.interactable = false;
        }
    }
    public void PularQuestao() //permite ao jogador pular a questão atual, verificando se o contador de pulos disponíveis é maior que zero antes de permitir o pulo, para evitar que o jogador tente pular mais vezes do que o permitido
    {
        if (contadorPularQuestao <= 0)
        {
            return;
        }
        contadorPularQuestao--;
        ControlaQntPularQuestao();
        AgendaProximaQuestao();
    }
}
