namespace PLCTester.Prompter;

public class DefaultPromptStrategy : IPromptStrategy
{
    public void SetColor()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
}

public class ValuePromptStrategy : IPromptStrategy
{
    public void SetColor()
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }
}

public class ErrorPromptStrategy : IPromptStrategy
{
    public void SetColor()
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
}
