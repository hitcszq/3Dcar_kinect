using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
namespace WindowsGame1
{
    class Hazard
    {
        public Model model = null;
        public Matrix projection;//投影变换矩阵；
        public Matrix view;//观察变换矩阵
        public float haloc;//障碍物的X坐标位置
        public float hadiameter;//障碍物包围球的直径
        public BoundingSphere hazardsphere;//障碍物包围球
        
      public  float right; 
        public  bool ri;
       
        public void zuoyou(){//调用随机数判断新生成的障碍物是在左车道还是在右车道
            Random mRandom = new Random();
            right =(float) mRandom.NextDouble();
            if (right < 0.5)
                ri = true;
            else
                ri = false;
            if (ri)
                haloc = -2.5f;
            else
                haloc = 2.5f;
    }
        public Model HazardModel
        {
          
            set
            {
                zuoyou();
                model = value;
                hazardsphere = model.Meshes[0].BoundingSphere; // 得到包含第一个部件的球体
                for (int i = 1; i < model.Meshes.Count; i++)
                {
                    //循环后，CarBoundingSphere是包含hazard所有部件的球体
                    hazardsphere = BoundingSphere.CreateMerged(hazardsphere, model.Meshes[i].BoundingSphere); //得到一个球包含参数指定的两个球
                }

                hazardsphere.Radius *= 0.09f;					// 包围球的尺寸缩小，调试所得最佳结果
                hazardsphere.Center.Z =60f;				// 世界变换中车位置改变
                hadiameter = hazardsphere.Radius * 2;			// 直径=(2*半径)
                hazardsphere.Center.X = haloc;
                hazardsphere.Center.Y = 0f;
            }
        }
       
       
        public void HazardDraw(GameTime gametime,float xpositon)//xposition表示新生成障碍物在Z方向的位置参数，障碍物移动即不断更新该参数
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();//默认灯光和材质
                    effect.World = Matrix.CreateScale(0.08f, 0.08f, 0.08f) * Matrix.CreateTranslation(haloc, 3.5f, xpositon);//汽车位置
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
        public void addHazard(GameTime gametime)//添加一个障碍物，在函数内部调用HazardDraw（）函数，Z轴位置参数传入初始值60f
        {
            /*float right; 
            bool ri;
            Random mRandom = new Random();
            right =(float) mRandom.NextDouble();
            if (right < 0.5)
                ri = true;
            else
                ri = false;
            if (ri)
                haloc = -2.5f;
            else
                haloc = 2.5f;*/
            this.HazardDraw(gametime,60f);
        }
        public void hazardmove(GameTime gametime,double xpositionc)//障碍物移动函数，不断改变传递的Z轴的位置参数来重绘三维模型，从而实现障碍物的移动
        {
            if (xpositionc > -20)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();//默认灯光和材质
                        effect.World = Matrix.CreateScale(0.08f, 0.08f, 0.08f) * Matrix.CreateTranslation(haloc, 1.5f, 60f - (float)xpositionc);//汽车位置
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
        }

    }
}
