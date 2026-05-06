using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestaoSO", menuName = "Scriptable Objects/QuestaoSO")]
public class QuestaoSO : ScriptableObject
{
    [SerializeField] public string txtQuestao;
    [SerializeField] public List<string> opcao = new(); 
    [SerializeField] public NivelQuestao Nivel;
    [SerializeField] public Sprite ImgQuestao;
    [SerializeField] public List<bool> correto = new();
    [SerializeField] public string Ajuda;

    public enum NivelQuestao
    {
        Facil,
        Medio,
        Dificil,
        ValeTudo
    }

}

