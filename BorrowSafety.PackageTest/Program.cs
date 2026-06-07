

namespace BorrowSafety.PackageTest;

public static class Program
{
    public static void Main()
    {
        Borrow<FileInfo> borrow = new FileInfo("Program.cs");
        string name = borrow.Name;
        Console.WriteLine(name);
    }
}
