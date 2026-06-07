using BorrowSafety;

namespace BorrowSafety.Test;

public static class Program
{
    public static void Main()
    {
        Borrow<FileInfo> fs = new FileInfo("Program.cs");
        string name = fs.Name;
        Console.WriteLine(name);
    }
}
