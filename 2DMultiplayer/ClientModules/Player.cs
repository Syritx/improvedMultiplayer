using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;

namespace _2DMultiplayer.ClientModules {
    class Player {

        Game game;

        float size = .1f;
        public Vector2 position = new Vector2(0,0);

        float[] vertices = {
             0,    0.05f, 0,
            -0.05f,-0.03f, 0,
             0.05f,-0.03f, 0
        };
        Shader shader;
        CustomKeyboard keyboard;

        int vertexArrayObject,
            vertexBufferObject;

        public Player(Game game) {
            this.game = game;
            shader = new Shader("ClientModules/Shaders/Player/pVertexShader.glsl", 
                                "ClientModules/Shaders/Player/pFragmentShader.glsl");

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i]*=size;
            }

            keyboard = new CustomKeyboard(game, this);
            vertexBufferObject = GL.GenBuffer();
        }

        public void Render() {
            vertexArrayObject = GL.GenVertexArray();

            vertices = new float[] {
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

        public void SetPosition(float xOff, float yOff) {
            position = new Vector2(position.X + xOff, position.Y + yOff);
        }

        public Vector2 GetPlayerPosition() {
            return position;
        }
    }


    class CustomKeyboard {

        Game game;
        Player player;
        Vector2 last;

        public CustomKeyboard(Game game, Player player) {
            this.game = game;
            this.player = player;
            game.UpdateFrame += Update;

            last = player.position;
        }

        void Update(FrameEventArgs e) {
            if (game.IsKeyDown(Keys.W))      player.SetPosition(0, .005f);
            else if (game.IsKeyDown(Keys.S)) player.SetPosition(0,-.005f);

            if (game.IsKeyDown(Keys.A))      player.SetPosition(-.005f, 0);
            else if (game.IsKeyDown(Keys.D)) player.SetPosition( .005f, 0);

            if (last != player.position) {
                Client.GetCommand("[POSITION]: " + player.position.X + ", " + player.position.Y);
                last = player.position;
            }
        }
    }
}