using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[Serializable]

public class Dawg
{
    public Node Root{get; set;} = new Node();
    private readonly char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
     'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];

    // Stack keeps track of the transition into the unchecked node in the form of Parent, character, Child
    private Stack<Tuple<Node, char, Node>> uncheckedNodes = new();

    // List tracking nodes we have checked
    private List<Node> minimizedNodes = new();
    private string previousWord = "" ;
    public Dawg() {
    
    }
    public void CleanUp(){
        uncheckedNodes.Clear();
        minimizedNodes.Clear();
    }

    public int Search(string word)  {
        Node curNode = Root;
        foreach (char c in word.Trim())
        {
            if (!curNode.Children.TryGetValue(c, out Node? value))
            {
                return -1;
            }
            curNode = value;
        }
        if (curNode.IsSuccessState)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public void AddWord(string word){
        int commonPrefix = FindCommonPrefix(word);
        Minimize(commonPrefix);
        Node curNode = uncheckedNodes.Count() == 0 ? Root : uncheckedNodes.Peek().Item3; 
        string part = word.Substring(commonPrefix) ;
        foreach (char c in word.Substring(commonPrefix)){
                Node newNode = new(){
                    Parent = curNode,
                    ParentChar = c
                };
                curNode.Children.Add(c, newNode);
                uncheckedNodes.Push(new Tuple<Node, char, Node>(curNode, c, newNode));
                curNode = newNode;
        }
        curNode.IsSuccessState = true;
        previousWord = word;
    }
    private int FindCommonPrefix(string word) {
        int searchLength = word.Length < previousWord.Length ? word.Length : previousWord.Length;
        int commonPrefix = 0; 
        for (int i = 0; i < searchLength; i++) {
            if (word[i] != previousWord[i]) {
                break;
            }       
            commonPrefix++; 
        }
        return commonPrefix;
    }

    private void Minimize(int prefix){
        for (int i = uncheckedNodes.Count - 1; i >= prefix; i--) {
            Tuple<Node, char, Node> unNode = uncheckedNodes.Pop(); 
            Node childNode = unNode.Item3;
            Node parentNode = unNode.Item1;
            char character = unNode.Item2;
            
            // If we have a minimized node that is equal to the current one then we will redirect the parent to the other node
            if(minimizedNodes.Contains(childNode)){
                if(parentNode.Children.ContainsKey(character)){
                    parentNode.Children[character] = childNode;
                } else{
                    parentNode.Children.Add(character, childNode);
                }
            } else { // node is alerady minimized so we can continue
                minimizedNodes.Add(childNode);
            }
        }
    }
}