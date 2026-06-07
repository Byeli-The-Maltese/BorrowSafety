// Borrow.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BorrowSafety;

/// <summary>
/// A borrow instance that wraps the specified strong reference and cannot escape
/// to the heap because it is a ref struct. The source generator included with this
/// project will forward public instance properties and public instance methods from
/// the wrapped strong reference onto this borrow.
/// </summary>
/// <param name="secret">
/// The strong reference that will be stored in this stack-bound object.
/// </param>
public readonly ref struct Borrow<T>(T secret)
where T : class
{
    // // // fields
    private readonly T secret = secret;
    private readonly bool authentic = true;

    // // // methods

    /// <summary>
    /// Allows the automatically forwarded bindings to access the wrapped strong reference.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public T Unlock_Unsafe([CallerFilePath] string path = "")
        => authentic
        ? path.EndsWith(".g.cs")
            ? secret
            : throw new Exception("This should not be called outside of a generated .cs file")
        : throw new Exception("The Borrow was created with its default constructor and should not be used");

    // // // operators

    /// <summary>
    /// Implicitly wraps a strong reference into a borrow.
    /// </summary>
    public static implicit operator Borrow<T>(T t) => new(t);
}
