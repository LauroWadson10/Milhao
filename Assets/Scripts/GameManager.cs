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

    int idPremio;

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
        idPremio = MasterManager.instance.idPremio;
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

    public bool AtualizaPremiacao()
    {
        // Atualiza o texto de Acertar
        txtAcertar.text = $"R$ {premiacao[idPremio]:N0}";

        if(idPremio == premiacao.Count - 1)
        {
            if (idPremio > 0)
                txtParar.text = $"R${premiacao[idPremio - 1]:N0}";
            else
                txtParar.text = "";

            // Atualiza o texto de Errar
            if (idPremio > 0)
                txtErrar.text = "0";
        }
        else
        {
            // Atualiza o texto de Parar
            if (idPremio > 0)
                txtParar.text = $"R${premiacao[idPremio - 1]:N0}";
            else
                txtParar.text = "";

            // Atualiza o texto de Errar
            if (idPremio > 0)
                txtErrar.text = $"R$ {premiacao[idPremio - 1] / 2:N0}";
            else
                txtErrar.text = "";
        }

        return true;
    }

    public void AcertoContador()
    {
        contadorAcertos++;
    }
}