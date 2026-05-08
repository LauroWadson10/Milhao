using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestaoSO", menuName = "Scriptable Objects/QuestaoSO")]
public class QuestaoSO : ScriptableObject
{
    [TextArea(3,10)]
    [SerializeField] public string txtQuestao;

    [SerializeField] public List<string> opcao = new();

    [SerializeField] public NivelQuestao Nivel;

    [SerializeField] public Sprite[] imgQuestao = new Sprite[2];

    [SerializeField] public List<bool> correto = new();

    [TextArea(2,8)]
    [SerializeField] public string Ajuda;

    public int QuantidadeOpcoes
    {
        get
        {
            if (opcao != null)
            {
                return opcao.Count;
            }

            return 0;
        }
    }
    public bool TemAjuda => !string.IsNullOrWhiteSpace(Ajuda);

    public bool TemImagem(int indice)
    {
        return imgQuestao != null && indice >= 0 && indice < imgQuestao.Length && imgQuestao[indice] != null;
    }

    public string OpcaoCorreta()
    {
        for(int i = 0; i < correto.Count; i++)
        {
            if (correto[i])
                return opcao[i];
        }

        return null;
    }

    //public bool EhOpcaoCorreta(int x)
    //{
    //    if(x == IndiceCorreto)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}


    public enum NivelQuestao
    {
        Facil,
        Medio,
        Dificil,
        ValeTudo
    }

}

