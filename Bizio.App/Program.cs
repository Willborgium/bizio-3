namespace Bizio.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (BizioGame game = new())
            {
                game.Run();
            }
        }
    }
}
