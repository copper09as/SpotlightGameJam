using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WordsManager : SingleCaseMono<WordsManager>
{
    public class Words
    {
        public List<string> words;
    }
    private Words Babblewords = new Words();
    private const string path =  "WordsData.json";
    void Start()
    {
        Babblewords = JsonTool.LoadByJson<Words>(Path.Combine(Application.streamingAssetsPath, path));
    }

   
    public string GetWords()
    {
        return Babblewords.words[Random.Range(0, Babblewords.words.Count)];//返回随机的一句话
    }
}
