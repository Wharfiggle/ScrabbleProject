using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;

// Fill the trie with words
try
{

    Trie tree = new Trie();
    StreamReader sr = new StreamReader("sampleWords.txt");

    string line = sr.ReadLine();
    while (line != null)
    {
        tree.AddWord(line);
        line = sr.ReadLine();
    }
    sr.Close();
    SerializeObj(tree);
    //Test if tree correctly identifies the words
    
    StreamReader sr2 = new StreamReader("sampleWords.txt");
    line = sr2.ReadLine();
    while (line != null) {
        Console.WriteLine("Trie recognizes word: " + line + "? " + tree.Search(line));
        line = sr2.ReadLine(); 
    }
    Dawg newDawg = new(tree);
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
// Deserialize the Trie from a file 
/*
try
{

    StreamReader sr = new StreamReader("sampleWords.txt");

    StreamReader sr2 = new StreamReader("TrieSer.json");
    string line = sr2.ReadLine();
    Trie? tree = JsonSerializer.Deserialize<Trie>(line);
    line = sr.ReadLine();
    while (line != null)
    {
        Console.WriteLine("Trie recognizes word: " + line + "? " + tree.Search(line));
        line = sr.ReadLine();
    }

    Dawg newDawg = new Dawg(tree);
    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}
*/
//SerializeObj(tree);

static void SerializeObj(Trie t)
{
    string jsonString = JsonSerializer.Serialize(t);
    //Console.WriteLine(jsonString);
    File.WriteAllText("TrieSer.json", jsonString);
}