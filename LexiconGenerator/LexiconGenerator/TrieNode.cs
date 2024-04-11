using System;
using System.Collections.Generic;

public class TrieNode {
    public bool isSuccessState {get; set;} = false;
    
    public Dictionary<char, TrieNode> children {get;} = new Dictionary<char, TrieNode>();


    public void AddChild(TrieNode newChild, char letter) {
        try
        {
        children.Add(letter, newChild);
        } catch (ArgumentException) {
            Console.WriteLine("An element is alerady mapped to letter: " + letter);
        }
    }

}