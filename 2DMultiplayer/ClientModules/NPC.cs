using System;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _2DMultiplayer.ClientModules {
    class NPC {

        public bool isActive = false, isCreated = false;
        public Vector2 position = new Vector2(0,0);
        Shader shader;
        int vertexArrayObject,
            vertexBufferObject;

        public void Create() {
            isCreated = true;
            shader = new Shader("ClientModules/Shaders/NPC/npcVertexShader.glsl", 
                                "ClientModules/Shaders/NPC/npcFragmentShader.glsl");
            vertexBufferObject = GL.GenBuffer();
            Render();
        }

        public void Render() {
            if (isActive) {
                vertexArrayObject = GL.GenVertexArray();

                float[] vertices = new float[] {
                    0+position.X,      0.05f+position.Y, 0,
                    -0.05f+position.X ,-0.05f+position.Y, 0,
                    0.05f+position.X ,-0.05f+position.Y, 0
                };

                GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,3 * sizeof(float),0);
                GL.EnableVertexAttribArray(0);

                GL.BindVertexArray(vertexArrayObject);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                try { 
                    shader.Use();
                } catch(Exception e1) {}
                GL.BindVertexArray(vertexArrayObject);
                GL.DrawArrays(PrimitiveType.TriangleFan, 0, vertices.Length/3);
            }
        }
    }
}