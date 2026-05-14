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

    private int nivelAtual = 1; // Começa na pergunta 1
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
        AtualizaPremiacao();
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

    public void AtualizaPremiacao()
    {
        // ACERTAR: valor do nível atual
        int valorAcertar = premiacao[nivelAtual];

        // PARAR: valor do nível anterior
        int valorParar = premiacao[nivelAtual - 1];

        // ERRAR: metade do PARAR, exceto no último nível que é R$ 0
        int valorErrar = (nivelAtual >= premiacao.Count - 1) ? 0 : premiacao[nivelAtual - 1] / 2;

        txtAcertar.text = FormatarDinheiro(valorAcertar);
        txtParar.text = FormatarDinheiro(valorParar);
        txtErrar.text = FormatarDinheiro(valorErrar);
    }

    public void AcertoContador()
    {
        contadorAcertos++;

        if (nivelAtual < premiacao.Count - 1)
        {
            nivelAtual++;
            AtualizaPremiacao();
        }
        else
        {
            Debug.Log("GANHOU O MILHÃO!");
        }
    }

    // Chame quando o jogador errar uma pergunta
    public void Errou()
    {
        int valorFinal = (nivelAtual >= premiacao.Count - 1) ? 0 : premiacao[nivelAtual - 1] / 2;
        Debug.Log("Errou! Levou: " + FormatarDinheiro(valorFinal));
        nivelAtual = 1;
        AtualizaPremiacao();
    }

    private string FormatarDinheiro(int valor)
    {
        return "R$ " + valor.ToString("N0", new CultureInfo("pt-BR"));
    }
}