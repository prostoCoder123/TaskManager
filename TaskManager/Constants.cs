namespace TaskManager;

public static class Constants
{
    public const string DbSchema = "tasks";

    public const int MaxElementsOnPage = 100;

    public const int DefaultElementsOnPage = 3;

    public const int MaxLength100 = 100;

    public const int MaxLength1000 = 1000;

    public const int MinLength3 = 3;

    public const string AllowedCharsRegexTemplate = "^[a-zA-Zа-яА-Я0-9.?!, -]+$";
}
