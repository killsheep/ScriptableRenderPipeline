using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.Graphing;

public class ShaderGraphParser
{
    private const string _NodeNamePrefix = "\"fullName\": \"UnityEditor.ShaderGraph.";

    [MenuItem("Tools/Count Nodes")]
    private static void CountNodes()
    {
        string[] filePaths = GetShaderGraphFilePaths();
        Dictionary<string, int> nodeDict = CreateNodeDictionary();

        Debug.Log("# of ShaderGraph files: " + filePaths.Length);

        foreach (string s in filePaths)
        {
            ReadShaderGraphFile(s, nodeDict);
        }

        // Sort based on # of times node is declared, then alphabetically
        List<KeyValuePair<string, int>> sortedDictList = nodeDict.ToList();
        sortedDictList.Sort(
            delegate (
                KeyValuePair<string, int> pair1,
                KeyValuePair<string, int> pair2)
            {
                if (pair1.Value == pair2.Value) return String.Compare(pair1.Key, pair2.Key,
                                                                  StringComparison.Ordinal);
                return pair2.Value.CompareTo(pair1.Value);
            }
        );

        List<string> containNodes = new List<string>();
        List<string> missingNodes = new List<string>();

        foreach (KeyValuePair<string, int> item in sortedDictList)
        {
            if (item.Value == 0) missingNodes.Add(item.Key);
            else containNodes.Add(item.Key);
        }

        if (containNodes.Count > 0)
        {
            string containString = "Contaned Nodes ("+containNodes.Count+"):\n";
            foreach (string s in containNodes)
            {
                containString += (s + ": " + nodeDict[s] +  "\n");
            }
            Debug.Log(containString);
        }

        if (missingNodes.Count > 0)
        {
            string missingString = "Missing Nodes ("+missingNodes.Count+"):\n";
            missingNodes.Sort();
            foreach (string s in missingNodes)
            {
                missingString += (s + ",\n");
            }
            Debug.LogError(missingString);
        }
    }

    private static string[] GetShaderGraphFilePaths()
    {
        return Directory.GetFiles(Application.dataPath + "/",
                                  "*ShaderGraph",
                                  SearchOption.AllDirectories);
    }

    private static Dictionary<string, int> CreateNodeDictionary()
    {
        Type abstractMatNode = typeof(AbstractMaterialNode);

        List<Type> types = Assembly.GetAssembly(abstractMatNode).GetTypes()
                               .Where(myType => myType.IsClass &&
                               !myType.IsAbstract
                               && myType.IsSubclassOf(abstractMatNode)).ToList();

        Dictionary<string, int> dict = new Dictionary<string, int>();
        foreach (Type t in types)
        {
            // Exclude master nodes.
            bool masternode = t.GetInterfaces().Contains(typeof(IMasterNode));
            if (!masternode)
            {
                dict.Add(t.Name, 0);
            }
        }
        return dict;
    }

    private static void ReadShaderGraphFile(string path, Dictionary<string, int> dict)
    {
        StreamReader reader = File.OpenText(path);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Contains(_NodeNamePrefix))
            {
                line = line.Substring(line.IndexOf(_NodeNamePrefix,
                                                   StringComparison.CurrentCulture) + _NodeNamePrefix.Length);
                line = line.Substring(0, line.IndexOf('"'));

                if (dict.ContainsKey(line))
                {
                    dict[line]++;
                }
            }
        }
    }

    // For testing if nodes are connected to the master node but currenlty I'm unsure how to convert
    // the the string path into the shader graph master node that DepthFirstCollectNodesFromNode needs.
    //private static void ReadConnectedNodesFromSGFile(Dictionary<string, int> nodeDict)
    //{
    //    // GetNodes<INode>().OfType<IMasterNode>().FirstOrDefault();
    //    IShaderGraph isg;
    //
    //    IMasterNode imn = null;
    //    var theNodes = ListPool<INode>.Get();
    //    NodeUtils.DepthFirstCollectNodesFromNode(theNodes, imn);
    //}

}