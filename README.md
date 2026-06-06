# BorrowSafety
Enables access to the Borrow&lt;T> ref struct in C#, which can prevent references and other data from escaping the stack. public methods and properties from the wrapped class T are forwarded to the closed Borrow&lt;T> via a source-generated extension block.
