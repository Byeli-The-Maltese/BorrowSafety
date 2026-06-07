// Lease.cs
namespace BorrowSafety;

/// <summary>
/// Wraps a weak reference and an optional cancellation token. Can be used to
/// temporarily borrow a strong reference that the weak reference targets. Use
/// the TryBorrow method to borrow the strong reference.
/// </summary>
/// 
/// <param name="resource">
/// The weak reference to the resource.
/// </param>
/// 
/// <param name="ct">
/// A token that may be cancelled to prevent further borrows from occurring.
/// </param>
/// <remarks>
/// Can be thought of as a revokable Borrow factory that can be issued by a
/// central authority. The lease permits temporary, stack-bound strong references
/// while only permitting weak references to be stored on the heap.
/// </remarks>
public sealed class Lease<T>(WeakReference<T> resource, CancellationToken ct=default)
where T : class
{
    // // // fields
    private readonly WeakReference<T> resource = resource;

    // // // alternate constructor
    /// <summary>
    /// Creates a new lease by wrapping the given strong greference in a weak
    /// reference and an optional cancellation token.
    /// </summary>
    /// 
    /// <param name="strongReference">
    /// The strong reference to be stored as a weak reference in this lease
    /// </param>
    /// 
    /// <param name="ct">
    /// The cancellation token that, when cancelled, will cause this lease
    /// to disallow further borrows
    /// </param>
    public Lease(T strongReference, CancellationToken ct=default)
        : this(new WeakReference<T>(strongReference), ct) { }

    // // // properties
    /// <summary>
    /// When this token is cancelled, further attempts to
    /// borrow the resource will fail.
    /// </summary>
    public CancellationToken CancellationToken { get; } = ct;

    // // // methods

    /// <summary>
    /// Attempts to borrow the resource in this lease.
    /// </summary>
    /// 
    /// <param name="borrow">
    /// The borrow that was produced. This is only valid if the method returns true.
    /// Attempting to use the borrow if this method returned false will cause
    /// an exception to be thrown.
    /// </param>
    /// 
    /// <returns>
    /// True if the borrow was successful and the output parameter is valid. False otherwise.
    /// </returns>
    /// 
    /// <remarks>
    /// The borrow will succeed if:
    /// <list type="number">
    ///   <item>CancellationToken is has not requested cancellation</item>
    ///   <item>The weakly referenced resource is still alive</item>
    /// </list>
    /// </remarks>
    public bool TryBorrow(out Borrow<T> borrow)
    {
        if (!CancellationToken.IsCancellationRequested 
            && resource.TryGetTarget(out T? strongReference)
            )
        {
            borrow = new(strongReference);
            return true;
        }
        else
        {
            borrow = default;
            return false;
        }
    }
}
