using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static QuestaoSO;
using DG.Tweening;

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
    [SerializeField] TMP_Text QntAjuda;
    [SerializeField] TMP_Text QntPularQuestao;
    [Space]
    [SerializeField] List<QuestaoSO> questao = new();
    [Space]
    [Range(0, 5.0f)]
    [SerializeField] float tempoParaAvancar;
    [SerializeField] int PularQuestaoPorNivel;

    QuestaoSO questaoAtual;
    NivelQuestao nivelAtual;
    public GameManager GameManager;

    int indiceQuestao;
    int contadorAjuda; //contador para limitar o uso da ajuda, se necessário
    int contadorPularQuestao = 3; //contador para limitar o uso do pular questão, se necessário
    string respostaSelecionada; //string para armazenar a resposta selecionada pelo jogador, para confirmar a resposta correta ou errada

    public static QuestionManager instance;

    Image botaoRespostaDada;

    [Header("Cor dos botoes")]
    [SerializeField] Color corAcerto;
    [SerializeField] Color corErro;
    [SerializeField] Color corNeutra;

    private void Awake()
    {
        nivelAtual = MasterManager.instance.nivelAtual;

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
        ConstroiQuestao();
        contadorPularQuestao = MasterManager.instance.contadorPularQuestao;
        contadorAjuda = MasterManager.instance.contadorAjuda;

        //Controla a disponiblidade dos botões
        btnAjuda.interactable = contadorAjuda >= 1;
        btnPularQuestao.interactable = contadorPularQuestao >= 1;
    }
    void ConstroiQuestao()
    {
        do
        {
            indiceQuestao = Random.Range(0, MasterManager.instance.questoes.Count);

            txtQuestao.text = MasterManager.instance.questoes[indiceQuestao].txtQuestao;

            for(int i = 0; i < MasterManager.instance.questoes[indiceQuestao].QuantidadeOpcoes; i++)
            {
                if (i < btnOpcoes.Length)
                {
                    btnOpcoes[i].GetComponentInChildren<TMP_Text>().text = MasterManager.instance.questoes[indiceQuestao].opcao[i];
                }
            }

        } while (nivelAtual != MasterManager.instance.questoes[indiceQuestao].Nivel);

        print(MasterManager.instance.questoes[indiceQuestao].OpcaoCorreta());

        questaoAtual = MasterManager.instance.questoes[indiceQuestao];

        GameManager.AtualizaPremiacao();
        //AtualizaImagens();
        AtualizaAjuda();
    }

    //sobrecarga do método para confirmar a resposta, permitindo passar a resposta selecionada como string, e encontrando o índice correspondente na lista de opções da questão atual antes de chamar a função para processar a resposta selecionada
    public void ConfirmaResposta()
    {
        if (questaoAtual.EhOpcaoCorreta(respostaSelecionada))
        {
            if(MasterManager.instance.idPremio == 15)
            {
                print("Fim de jogo");
            }
            else
            {
                MasterManager.instance.AcertaQuestao(indiceQuestao);

                StartCoroutine(FinalizaQuestao());
                print("Acertou");
            }

            botaoRespostaDada.DOColor(corAcerto, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            botaoRespostaDada.DOColor(corErro, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void QuestionaResposta(string resp, Image img)
    {
        painelConfirmacao.SetActive(true);
        respostaSelecionada = resp;
        botaoRespostaDada = img;
    }

    public void CancelaResposta()
    {
        painelConfirmacao.SetActive(false);
    }

    IEnumerator FinalizaQuestao()
    {
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        btnAjuda.interactable = false; //desativa o botão de ajuda para evitar que o jogador tente usar a ajuda novamente enquanto o painel de ajuda está aberto

    }
    void EscondeAjuda() //esconde o painel de ajuda, verificando se o painel está ativo antes de tentar esconder, para evitar erros caso ele já esteja escondido
    {
        if (painelAjuda != null && painelAjuda.activeSelf)
        {
            painelAjuda.SetActive(false);
        }
    }
    void ControlaQntAjuda()//c ontrola a quantidade de ajudas disponíveis, atualizando o texto correspondente e desativando o botão de ajuda se o contador chegar a zero, para evitar que o jogador tente usar mais ajudas do que o permitido
    {
        if (QntAjuda != null)
        {
            QntAjuda.text = $"{contadorAjuda}";
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
            return;

        contadorPularQuestao--;
        ControlaQntPularQuestao();
        btnPularQuestao.interactable = false; //desativa o botão de pular questão para evitar que o jogador tente pular novamente enquanto a questão atual ainda está sendo processada
        //AgendaProximaQuestao();
    }
    void EscondeConfirmacao()
    {
        if (painelConfirmacao != null && painelConfirmacao.activeSelf)
        {
            painelConfirmacao.SetActive(false);
        }
    }

    private void OnDisable()
    {
        DOTween.KillAll();
    }
}

