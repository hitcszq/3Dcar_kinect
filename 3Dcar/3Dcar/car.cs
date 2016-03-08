using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class car
    {
        Model model = null;//读入car.x文件中的汽车保存到carmodel中
        public Matrix projection;//投影变换矩阵；
        public Matrix view;//观察变换矩阵
        public float carlocation = 2.5f;//汽车的位置
        public float carspeed = 10.0f; //汽车左右移动的速度
        public bool movingleft=false; //左移
        public bool movingright = false;//右移；
        public float cardiameter;
        public BoundingSphere carboundingsphere;//汽车包围球
        public Model CarModel
        {
            set { model = value;
                carboundingsphere= model.Meshes[0].BoundingSphere; // 得到包含第一个部件的球体
                for (int i = 1; i < model.Meshes.Count; i++)
                {
                    //循环后，CarBoundingSphere是包含汽车所有部件的球体
                    carboundingsphere = BoundingSphere.CreateMerged(carboundingsphere, model.Meshes[i].BoundingSphere); //得到一个球包含参数指定的两个球
                }

                carboundingsphere.Radius *= 0.1f;					// CarDraw方法的世界变换中车尺寸放大0.85倍
                carboundingsphere.Center.Z = -3.0f;				// CarDraw方法的世界变换中车位置改变
                cardiameter = carboundingsphere.Radius * 2;			// 车直径=(2*半径)
                carboundingsphere.Center.Y = 0f;
                carboundingsphere.Center.X = carlocation;
            }
        }
        public void CarDraw(GameTime gametime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();//默认灯光和材质
                    effect.World = Matrix.CreateScale(0.85f, 0.85f, 0.85f) *Matrix.CreateTranslation(carlocation, 1.5f, -3.0f);//汽车位置
                    effect.View = view;//观察变换，摄像机位置
                    effect.Projection = projection;//投影变换
                }
                mesh.Draw();
            }
        }
        public void update(GameTime gameTime)//事实更新汽车的位置，在Game1类的update()函数中调用
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            if(movingleft)
            {
                carlocation += (float)(carspeed * elapsed);
                if(carlocation>=2.5f)
                {
                    movingleft = false;
                    carlocation = 2.5f;
                   carboundingsphere.Center.X = carlocation;
                }
            }
            if (movingright)
            {
                carlocation -= (float)(carspeed * elapsed);
                if (carlocation <=- 2.5f)
                {
                    movingright = false;
                    carlocation = -2.5f;
                    carboundingsphere.Center.X = carlocation;
                }
            }
        }
    }
}
