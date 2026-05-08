using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "QuestaoSO", menuName = "Scriptable Objects/QuestaoSO")]
public class QuestaoSO : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] public string txtQuestao;

    [SerializeField] public List<string> opcao = new();

    [SerializeField] public NivelQuestao Nivel;

    [FormerlySerializedAs("ImgQuestao")]
    [SerializeField] public Sprite[] imgQuestao = new Sprite[2];

    [SerializeField] public List<bool> correto = new();

    [TextArea(2, 8)]
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

    public int IndiceCorreto
    {
        get
        {
            if (correto == null)
            {
                return -1;
            }

            for (int i = 0; i < correto.Count; i++)
            {
                if (correto[i])
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public bool TemImagem(int indice)
    {
        return imgQuestao != null && indice >= 0 && indice < imgQuestao.Length && imgQuestao[indice] != null;
    }

    public bool EhOpcaoCorreta(int indice)
    {
        return correto != null && indice >= 0 && indice < correto.Count && correto[indice];
    }

    public string OpcaoCorreta()
    {
        int indice = IndiceCorreto;

        if (opcao == null || indice < 0 || indice >= opcao.Count)
        {
            return string.Empty;
        }

        return opcao[indice];
    }

    public bool EstaValida()
    {
        int indiceCorreto = IndiceCorreto;

        return !string.IsNullOrWhiteSpace(txtQuestao)
            && opcao != null
            && opcao.Count > 0
            && indiceCorreto >= 0
            && indiceCorreto < opcao.Count;
    }

    void OnValidate()
    {
        opcao ??= new List<string>();
        correto ??= new List<bool>();

        while (correto.Count < opcao.Count)
        {
            correto.Add(false);
        }

        while (correto.Count > opcao.Count)
        {
            correto.RemoveAt(correto.Count - 1);
        }

        imgQuestao ??= new Sprite[2];
    }

    public enum NivelQuestao
    {
        Facil,
        Medio,
        Dificil,
        ValeTudo
    }
}
