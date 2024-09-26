public class TestCase
{
    public TestCase()
    {
        name = "";
        type = "";
        mode = "";
        key = new List<byte>();
        ini_vector = new List<byte>();
        data = new List<byte>();
    }

    public string name { get; set; }
    public string type { get; set; }
    public string mode { get; set; }
    public List<byte> key { get; set; }
    public List<byte> ini_vector { get; set; }
    public List<byte> data { get; set; }
}
