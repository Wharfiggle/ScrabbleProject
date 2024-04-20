using System;
using System.Collections.Generic;

[Serializable]
public class Node {
    public bool isSuccessState {get; set;} = false;

    public Dictionary<char, Node> children {get; set;} = new Dictionary<char, Node>();


    public void AddChild(Node newChild, char letter) {
        try
        {
        children.Add(letter, newChild);
        } catch (ArgumentException) {
            Console.WriteLine("An element is alerady mapped to letter: " + letter);
        }
    }

}