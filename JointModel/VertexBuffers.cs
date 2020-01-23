using System;
using OpenTK.Graphics.OpenGL;

namespace JointModel
{
    class VertexBuffers
    {
        public static int Init(int program)
        {
            float[] vertices = new float[]
            {
                1.5f, 10f, 1.5f, -1.5f, 10f, 1.5f, -1.5f, 0f, 1.5f, 1.5f, 0f, 1.5f,     // v0-v1-v2-v3 front
                1.5f, 10f, 1.5f, 1.5f, 0f, 1.5f, 1.5f, 0f, -1.5f, 1.5f, 10f, -1.5f,     // v0-v3-v4-v5 right
                1.5f, 10f, 1.5f, 1.5f, 10f, -1.5f, -1.5f, 10f, -1.5f, -1.5f, 10f, 1.5f, // v0-v5-v6-v1 up
                -1.5f, 10f, 1.5f, -1.5f, 10f, -1.5f, -1.5f, 0f, -1.5f, -1.5f, 0f, 1.5f, // v1-v6-v7-v2 left
                -1.5f, 0f, -1f, 1.5f, 0f, -1.5f, 1.5f, 0f, 1.5f, -1.5f, 0f, 1.5f,       // v7-v4-v3-v2 down
                1.5f, 0f, -1.5f, -1.5f, 0f, -1.5f, -1.5f, 10f, -1.5f, 1.5f, 10f, -1.5f  // v4-v7-v6-v5 back
            };

            float[] normals = new float[]
            {
                0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f,     // v0-v1-v2-v3 front
                1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f,     // v0-v3-v4-v5 right
                0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f,     // v0-v5-v6-v1 up
                -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, // v1-v6-v7-v2 left
                0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, // v7-v4-v3-v2 down
                0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f  // v4-v7-v6-v5 back
            };

            int[] indices = new int[]
            {
                0, 1, 2, 0, 2, 3,           // front
                4, 5, 6, 4, 6, 7,           // right
                8, 9, 10, 8, 10, 11,        // up
                12, 13, 14, 12, 14, 15,     // left
                16, 17, 18, 16, 18, 19,     // down
                20, 21, 22, 20, 22, 23      // back
            };

            if (!InitArrayBuffer(program, "aPosition", vertices, 3)) return -1;
            if (!InitArrayBuffer(program, "aNormal", normals, 3)) return -1;
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            int indexBuffer;
            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);

            return indices.Length;
        }

        private static bool InitArrayBuffer(int program, string attributeName, float[] data, int num)
        {
            int buffer;
            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

            int attributeLocation = GL.GetAttribLocation(program, attributeName);
            if (attributeLocation == -1)
            {
                Console.WriteLine("Failed to get the storage location of " + attributeName);
                return false;
            }
            GL.VertexAttribPointer(attributeLocation, num, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(attributeLocation);

            return true;
        }
    }
}
