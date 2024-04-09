using System;
using System.Collections.Generic;

public class TrieNode {
    private bool isSuccessState {get; set;}
    private char letter;
    private List<TrieNode> children {get;} 

    public TrieNode(char letter) {
        this.letter = letter;
        isSuccessState = false;
        children = new List<TrieNode>();
    }

    public TrieNode(char letter, bool successState) {
        this.letter = letter;
        isSuccessState = successState;
        children = new List<TrieNode>();
    }

    public void addChild(TrieNode newChild) {
        children.Add(newChild);
    }

}