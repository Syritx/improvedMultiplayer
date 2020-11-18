using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Text;

namespace _2DMultiplayer.ClientModules {
    class Shader {

        int handle;
        int vertexShader, fragmentShader;

        public Shader(string vertexShaderPath, string fragmentShaderPath) {

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderPath));
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderPath));
            GL.CompileShader(fragmentShader);

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use() {
            GL.UseProgram(handle);
        }

        string LoadShaderSource(string path) {
            string code = "";

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8)) {
                code = reader.ReadToEnd();
            }
            return code;
        }
    }
}