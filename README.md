# OpenTK

Using the provided example, this repository expands the singular triangle to be a rectangle. Furthermore, adding more vertex attributes enables colour interpolation.

After having used OpenGL a few times before, it's interesting to see what decisions were made by the OpenTK team for abstractions. For instance, GL.BufferData, like the C API counterpart, specifies the buffer size in bytes, despite the fact that the function takes a generic parameter that should permit doing the byte count arithmetic as part of the implementation. This does make the library easy to use for those who already have experience with the OpenGL C API, but when writing an abstraction for myself, I would consider leveraging the power of compile time generics to ensure the correct number of bytes is set. It is far too easy for forget to multiple by the size of a float.

## Running

The project currently is only compatible with Visual Studio on Windows. Only tested on X86_64. I intend to add support for Mac in the future, as despite the fact that OpenGL is deprecated on Mac, it would be convenient to use my MacBook to experiment further with OpenTK.

## Vector & Matrix Operations in C#

I am continuing to use OpenTK, building off of my the previous work, and using vector math operations to change the colour of the square.

For Vectors, I used 3 dimensional float vectors. I implemented the following operations:

- Addition
- Subtraction
- Dot Product
- Cross Product
- Random Generation

For Matrices, I used 3 dimensional float matrices. I implemented the following operations:

- Using the identity matrix
- Scaling a vector
- Rotating a vector along the x axis
- Multiplying two matrices together

### Sample Output

```text
=== VECTOR OPERATIONS ===
BEFORE
Bottom Left: (0, 0, 0)     
Bottom Right: (0.5, 0, 0.5)
Top Right: (0, 0.5, 0)     
Top Left: (1, 1, 1)        

random vector -> (0.7088302, 0.9412896, 0.0708315)

AFTER
Bottom Left: (0.7088302, 0.9412896, 0.0708315)
Bottom Right: (0.7911698, 0.058710396, 0.4291685)
Top Right: (0.4706448, 0.804252, 0.09064126)
Top Left: (0.12954187, 0.6379987, 0.23245943)

=== MATRIX OPERATIONS ===
Identity matrix:
(1, 0, 0)
(0, 1, 0)
(0, 0, 1)
multiplied by Vector(0.7088302, 0.9412896, 0.0708315) = (0.7088302, 0.9412896, 0.0708315)
Same value
Scaling matrix:
(2, 0, 0)
(0, 0.5, 0)
(0, 0, 4)
multiplied by Vector(0.7088302, 0.9412896, 0.0708315) = (1.4176604, 0.4706448, 0.283326)
Different values
Rotation matrix:
(1, 0, 0)
(0, 0.70710677, -0.70710677)
(0, 0.70710677, 0.70710677)
multiplied by Vector(0.7088302, 0.9412896, 0.0708315) = (0.7088302, 0.7156777, -0.6155068)
Rotated on X axis, which component stayed the same, while the others changed
Multiplying matricies:
(2, 0, 0)
(0, 0.5, 0)
(0, 0, 4)
Multiplied
(1, 0, 0)
(0, 0.70710677, -0.70710677)
(0, 0.70710677, 0.70710677)
Equals
(2, 0, 0)
(0, 0.35355338, -0.35355338)
(0, 2.828427, 2.828427)                                                                                                              
```

## Rendering a 3D Cube

I am continuing to use OpenTK, building off of my the previous work.

I generated a cube structure based on a base position and equivalent side lengths. From there I populated the buffers. I also used a Model View Projection matrix (just as the view matrix for now) to rotate the camera around the cube. The cube is stationary in world-space. This MVP was passed to the vertex shader as a uniform.

I also added one control. Through pressing the `E` key, you can toggle wireframe mode.

I also enabled front-face culling. While backfacing culling was desired, the winding order I currently have set results in the interior faces being considered the front faces. That's something I should fix.
