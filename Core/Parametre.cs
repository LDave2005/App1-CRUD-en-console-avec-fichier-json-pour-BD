namespace App1.Core
{
    public class Parametre
    {
        public string Key { get; }
        public string Value { get; set; }

        public Parametre(string key, string value = null!)
        {
            Key = key;
            Value = value;
        }
    }
}
