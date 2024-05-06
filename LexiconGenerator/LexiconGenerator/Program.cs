using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

// Fill the trie with words
/*try
{

    Dawg dawg = new Dawg();
    StreamReader sr = new StreamReader("SampleWords.txt");
    string line = sr.ReadLine();
    while (line != null)
    {
      //  Console.WriteLine("Adding" + line);
        dawg.AddWord(line);
        line = sr.ReadLine();
    }
    sr.Close();
    dawg.CleanUp();

    SerializeObj(dawg);

    //Test if tree correctly identifies the words
    StreamReader sr2 = new StreamReader("SampleWords.txt");
    line = sr2.ReadLine();
    while (line != null)
    {
        Console.WriteLine("Trie recognizes word: " + line + "? " + dawg.Search(line));
        line = sr2.ReadLine();
    }

    sr2.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
} */
// Deserialize the Trie from a file 
try
{
    JsonSerializerOptions opts = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };
    StreamReader sr = new StreamReader("SampleWords.txt");

    StreamReader sr2 = new StreamReader("DawgSer.json");
    string line = sr2.ReadLine();
    Dawg? dawg = JsonSerializer.Deserialize<Dawg>(line, opts);
    sr2.Close();
    line = sr.ReadLine();
    while (line != null)
    {
        Console.WriteLine("Trie recognizes word: " + line + "? " + dawg.Search(line));
        line = sr.ReadLine();
    }

    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

static void SerializeObj(Dawg d)
{
    JsonSerializerOptions opts = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };
    string jsonString = JsonSerializer.Serialize(d, opts);
    //Console.WriteLine(jsonString);
    File.WriteAllText("DawgSer.json", jsonString);
}