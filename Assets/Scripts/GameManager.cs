using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button PararJogo;
    [SerializeField] private Button btnSimEncerar;
    [SerializeField] private Button btnNaoEncerrar;
    [SerializeField] private GameObject PainelConfirmar;
    [SerializeField] private GameObject painelEncerrar;
    [SerializeField] private List<int> premiacao = new();
    [SerializeField] private TMP_Text txtAcertar;
    [SerializeField] private TMP_Text txtParar;
    [SerializeField] private TMP_Text txtErrar;

    public static GameManager instance;

    //private int nivelAtual = 1; // Começa na pergunta 1
    private int contadorAcertos;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        PainelConfirmar.SetActive(false);
        painelEncerrar.SetActive(false);

        controladorBotoes();
        contadorAcertos = 1;
        AtualizaPremiacao(out _);
    }

    void controladorBotoes()
    {
        btnNaoEncerrar.onClick.RemoveAllListeners();
        btnNaoEncerrar.onClick.AddListener(EsconderConfirmacao);
        btnSimEncerar.onClick.RemoveAllListeners();
        btnSimEncerar.onClick.AddListener(EncerrarJogo);
        PararJogo.interactable = true;
        PararJogo.onClick.RemoveAllListeners();
        PararJogo.onClick.AddListener(MostrarConfirmacao);
    }

    void MostrarConfirmacao()
    {
        PainelConfirmar.SetActive(true);
        controladorBotoes();
    }

    void EncerrarJogo()
    {
        painelEncerrar.SetActive(true);
    }

    void EsconderConfirmacao()
    {
        PainelConfirmar.SetActive(false);
    }

    public bool AtualizaPremiacao(out int indice)
    {
        // Calcula índice com base nos acertos
        indice = contadorAcertos - 1;

        // Se a lista estiver vazia, retorna falso
        if (premiacao == null || premiacao.Count == 0)
            return false;

        // Garante que o índice esteja dentro do tamanho da lista
        if (indice < 0)
            indice = 0;
        else if (indice >= premiacao.Count)
            indice = premiacao.Count - 1;

        // Atualiza o texto de Acertar
        txtAcertar.text = $"R$ {premiacao[indice + 1] * 2:N0}";

        // Atualiza o texto de Parar
        if (indice > 0)
            txtParar.text = $"R${premiacao[indice * 2] / 1:N0}";
        else
            txtParar.text = "R$ 0";

        // Atualiza o texto de Errar
        if (indice > 0)
            txtErrar.text = $"R$ {premiacao[indice]:N0}";
        else
            txtErrar.text = "R$ 0";

        return true;
    }

    public void AcertoContador()
    {
        contadorAcertos++;
    }
}