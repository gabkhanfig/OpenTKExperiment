# OpenTK

Using the provided example, this repository expands the singular triangle to be a rectangle. Furthermore, adding more vertex attributes enables colour interpolation.

After having used OpenGL a few times before, it's interesting to see what decisions were made by the OpenTK team for abstractions. For instance, GL.BufferData, like the C API counterpart, specifies the buffer size in bytes, despite the fact that the function takes a generic parameter that should permit doing the byte count arithmetic as part of the implementation. This does make the library easy to use for those who already have experience with the OpenGL C API, but when writing an abstraction for myself, I would consider leveraging the power of compile time generics to ensure the correct number of bytes is set. It is far too easy for forget to multiple by the size of a float.

## Running

The project currently is only compatible with Visual Studio on Windows. Only tested on X86_64. I intend to add support for Mac in the future, as despite the fact that OpenGL is deprecated on Mac, it would be convenient to use my MacBook to experiment further with OpenTK.
