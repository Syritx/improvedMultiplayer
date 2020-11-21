using System.Collections.Generic;
using System;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK;

namespace _2DMultiplayer.ClientModules {
    class Game : GameWindow {

        static Player player;
        static List<NPC> NPCs = new List<NPC>();

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) :
        base(gameWindowSettings, nativeWindowSettings) {
            player = new Player(this);

            for (int i = 0; i < 10; i++) {
                NPCs.Add(null);
                Console.WriteLine("added NPC");
            }
            Run();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            player.Render();

            try {
                foreach(NPC npc in NPCs) {
                    if (!npc.isCreated) npc.Create();
                    npc.Render();
                }
            }
            catch(Exception e) {}
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            Matrix4 perspectiveMatrix =
                Matrix4.CreatePerspectiveFieldOfView(1, 1000 / 720, 1.0f, 2000.0f);

            GL.LoadMatrix(ref perspectiveMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.End();
        }

        protected override void OnLoad() {
            Client.SendMessage("loaded in game");

            base.OnLoad();
            GL.ClearColor(0,0,0,0);
        }

        public static Vector2 GetPlayerPosition() {
            return player.GetPlayerPosition();
        }

        public static void LoadNPC(int index, Vector2 position) {
            NPCs[index] = new NPC();
            NPCs[index].position = position;
            NPCs[index].isActive = true;
        }
    }
}