namespace PLCTester.Prompter;

public class PromptContext
{
    private readonly IPromptStrategy _strategy;

    public PromptContext(IPromptStrategy strategy)
    {
        _strategy = strategy;
    }

    public void Display(string text)
    {
        _strategy.SetColor();
        Console.WriteLine(text);
        Console.ResetColor();
    }
}