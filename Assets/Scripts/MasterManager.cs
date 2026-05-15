using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager instance;

    public int contAcertos;
    public int idPremio;
    public List<QuestaoSO> questoes = new();
    public QuestaoSO.NivelQuestao nivelAtual;
    public int contadorPularQuestao;
    public int contadorAjuda;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AcertaQuestao(int id)
    {
        questoes.RemoveAt(id);
        contAcertos++;
        idPremio++;

        if(contAcertos >= 5)
        {
            nivelAtual++;
            contAcertos = 0;
        }
    }
}
