using System;
using OpenTK.Graphics.OpenGL;

namespace JointModel
{
    class ShaderProgram
    {
        private static string _vShaderSource =
            @"  #version 130

                in vec4 aPosition;
                in vec4 aNormal;

                uniform mat4 uMvpMatrix;
                uniform mat4 uNormalMatrix;

                out vec4 vColor;

                void main()
                {
                    gl_Position = uMvpMatrix * aPosition;

                    vec3 lightDirection = normalize(vec3(0.0, 0.5, 0.7)); // Light direction
                    vec4 color = vec4(1.0, 0.4, 0.0, 1.0);
                    vec3 normal = normalize((uNormalMatrix * aNormal).xyz);
                    float nDotL = max(dot(normal, lightDirection), 0.0);
                    vColor = vec4(color.rgb * nDotL + vec3(0.1), color.a);
                }
            ";

        private static string _fShaderSource =
            @"  #version 130

                precision mediump float;

                in vec4 vColor;
                out vec4 fragColor;

                void main()
                {
                    fragColor = vColor;
                }
            ";

        public static int Create()
        {
            int vShader = CreateShader(_vShaderSource, ShaderType.VertexShader);
            int fShader = CreateShader(_fShaderSource, ShaderType.FragmentShader);
            if (vShader == -1 || fShader == -1)
            {
                return -1;
            }

            int program = GL.CreateProgram();
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);
            GL.LinkProgram(program);
            int ok;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out ok);
            if (ok == 0)
            {
                Console.WriteLine("Failed to link a program. Error: " + GL.GetProgramInfoLog(program));
                return -1;
            }
            GL.UseProgram(program);

            return program;
        }

        private static int CreateShader(string source, ShaderType type)
        {
            int shader = GL.CreateShader(type);

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            int ok;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out ok);
            if (ok == 0)
            {
                Console.WriteLine(type.ToString() + ": " + GL.GetShaderInfoLog(shader));
                return -1;
            }

            return shader;
        }
    }
}
