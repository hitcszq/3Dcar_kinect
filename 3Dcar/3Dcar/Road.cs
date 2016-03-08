using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class road
    {
        public Model RoadModel = null;
        public Matrix projection;
        public Matrix view;

        public void RoadDraw(GameTime gameTime,float x,float y,float z)//根据传入的位置来绘制道路，在GAME1种的update()函数d调用
        {
            foreach (ModelMesh mesh in RoadModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateTranslation(x, y, z);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
