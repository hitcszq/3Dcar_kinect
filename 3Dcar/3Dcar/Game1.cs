using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Drawing.Imaging;

using System.IO;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary> 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public struct Pose
        {
            public string Title;//姿势名称
            public PoseAngle[] Angles;//PoseAngle数组
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Matrix projection;
        public Matrix view;
        private SpriteFont mFont;
        car car;
        //Texture2D mSpriteTexture;
        road road;
        Hazard hazard;
         float Hv = 4f;//
        float roaddepth0 = 0.0f;
        float roaddepth1 = 100.0f;
        private float RoadSpeed=20.0f;
        double xpositionc = 0;//障碍物在z方向的移动距离
        public bool hazardvisible = true;
        private Texture2D mBackground;
        public int hazardnumber = 1;//已通过的障碍物
        private KinectSensor kinect = null;//指向Kinect对象  
        private Skeleton[] skeletonData;   //存储从Kinect传感器接收到的数据对象 
        private Pose[] poseLibrary;//自定义手势库对象
    
        private void StartKinectST()
        {
            // Get first Kinect Sensor  
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
            //注意：此判断MS目前无效  

            // 允许骨骼跟踪  
            kinect.SkeletonStream.Enable();
            //关闭颜色和景深数据的接收  
            kinect.ColorStream.Disable();
            kinect.DepthStream.Disable();
            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
            // Get Ready for Skeleton Ready Events  
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            
            // Start Kinect sensor   
            kinect.Start();
        }
        private void ProcessPosePerforming(Skeleton skeleton)
        {
            //bool ret;
            //Graphics grMain = screenPanel.CreateGraphics();
            //int value = 0;
           /* if (IsPose(skeleton, this.poseLibrary[0]))//    up
            {
                Console.WriteLine("双手上举垂直姿势符合条件");
                //value = UP;
            }
            else if (IsPose(skeleton, this.poseLibrary[1]))
            {
                Console.WriteLine("双手交叉姿势符合条件");//fall down
               // value = DOWN;
            }*/
            if (IsPose(skeleton, this.poseLibrary[2]))
            {
                car.movingleft = true;
                car.movingright = false;
            }
            else if (IsPose(skeleton, this.poseLibrary[3]))
            {
                car.movingleft = false;
                car.movingright = true;
            }
           
        }
        private bool IsPose(Skeleton skeleton, Pose pose)
        {
            bool isPose = true;
            double angle;
            double poseAngle;
            double poseThreshold;
            double loAngle;
            double hiAngle;

            //遍历一个姿势中所有poseAngle，判断是否符合相应的条件
            for (int i = 0; i < pose.Angles.Length && isPose; i++)
            {
                poseAngle = pose.Angles[i].Angle;
                poseThreshold = pose.Angles[i].Threshold;
                //调用 GetJointAngle 方法来计算两个关节点之间的角度
                angle = GetJointAngle(skeleton.Joints[pose.Angles[i].CenterJoint], skeleton.Joints[pose.Angles[i].AngleJoint]);

                hiAngle = poseAngle + poseThreshold;
                loAngle = poseAngle - poseThreshold;

                //判断角度是否在360范围内，如果不在，则转换到该范围内
                if (hiAngle >= 360 || loAngle < 0)
                {
                    loAngle = (loAngle < 0) ? 360 + loAngle : loAngle;
                    hiAngle = hiAngle % 360;

                    isPose = !(loAngle > angle && angle > hiAngle);
                }
                else
                {
                    isPose = (loAngle <= angle && hiAngle >= angle);
                }
            }
            //如果判断角度一致，则返回true
            return isPose;
        }
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {

                //Tracked that defines whether a skeleton is 'tracked' or not.   
                //The untracked skeletons only give their position.   
                //if (SkeletonTrackingState.Tracked != data.TrackingState) continue;  
                //this.label1.Text = "Kinect传感器连结成功！";

                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame  
                {
                    // check that a frame is available  
                    if (skeletonFrame != null && this.skeletonData != null)
                    {
                        // get the skeletal information in this frame  
                        skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                      Skeleton  skeleton = GetPrimarySkeleton(this.skeletonData);



                        //调用处理姿势的方法
                        if (skeleton != null)
                        {
                           // Graphics jointg = panel1.CreateGraphics();
                            //jointg.FillRectangle(new SolidBrush(Color.White), 0, 0, panel1.Width, panel1.Height);
                            //DrawBonesAndJoints(skeleton, jointg);
                            this.ProcessPosePerforming(skeleton);

                        }
                    }
                }


                foreach (Skeleton skeleton in this.skeletonData)
                {
                    if (null != skeleton)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            //Console.WriteLine("KinectID: " + skeleton.TrackingId);
                            //只处理跟踪移动的数据   
                            //DrawTrackedSkeletonJoints(skeleton.Joints);
                        }
                        else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                        {

                            //temDrawSkeletonPosition(skeleton.Position);  
                        }
                    }
                    else
                    {
                        Console.WriteLine("null Skeleton");
                    }
                }
            }
            catch (Exception ex)
            {
               // this.label1.Text = "Kinect失去连结，请检查USB或电源";
            }

        }


        /// <summary>
        /// 得到距离最近的骨骼对象
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns></returns>
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton       
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }

            return skeleton;
        }

        /// <summary>
        /// 获取每一个节点在主 UI 布局空间中的坐标的方法
        /// </summary>
        /// <param name="kinectDevice"></param>
        /// <param name="joint"></param>
        /// <param name="containerSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// 

        private static System.Drawing.Point GetJointPoint(KinectSensor kinectDevice, Joint joint, System.Drawing.Point offset)
        {
            //得到节点在主 UI 布局空间中的坐标
            //DepthImagePoint point = kinectDevice.MapSkeletonPointToDepth(joint.Position, kinectDevice.DepthStream.Format);
            DepthImagePoint point = kinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, kinectDevice.DepthStream.Format);
            point.X = (int)(point.X - offset.X);
            point.Y = (int)(point.Y - offset.Y);

            return new System.Drawing.Point(point.X, point.Y);
        }
    
        /// <summary>
        /// 计算2关节点之间的夹角
        /// </summary>
        /// <param name="centerJoint"></param>
        /// <param name="angleJoint"></param>
        /// <returns></returns>
        private double GetJointAngle(Joint centerJoint, Joint angleJoint)
        {

            System.Drawing.Point primaryPoint = GetJointPoint(this.kinect, centerJoint, new System.Drawing.Point());
            System.Drawing.Point anglePoint = GetJointPoint(this.kinect, angleJoint, new System.Drawing.Point());
            System.Drawing.Point x = new System.Drawing.Point(primaryPoint.X + anglePoint.X, primaryPoint.Y);

            double a;
            double b;
            double c;

            a = Math.Sqrt(Math.Pow(primaryPoint.X - anglePoint.X, 2) + Math.Pow(primaryPoint.Y - anglePoint.Y, 2));
            b = anglePoint.X;
            c = Math.Sqrt(Math.Pow(anglePoint.X - x.X, 2) + Math.Pow(anglePoint.Y - x.Y, 2));

            double angleRad = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            double angleDeg = angleRad * 180 / Math.PI;

            //如果计算角度大于180度，将其转换到0-180度
            if (primaryPoint.Y < anglePoint.Y)
            {
                angleDeg = 360 - angleDeg;
            }

            return angleDeg;
        }
        private void PopulatePoseLibrary()
        {
            this.poseLibrary = new Pose[4];



            //Pose 1 -举起手来 Both Hands Up
            this.poseLibrary[0] = new Pose();
            this.poseLibrary[0].Title = "举起手来(Arms Up)";
            this.poseLibrary[0].Angles = new PoseAngle[4];
            this.poseLibrary[0].Angles[0] = new PoseAngle(JointType.ShoulderLeft, JointType.ElbowLeft, 180, 8);
            this.poseLibrary[0].Angles[1] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 180, 8);
            this.poseLibrary[0].Angles[2] = new PoseAngle(JointType.ShoulderRight, JointType.ElbowRight, 0, 8);
            this.poseLibrary[0].Angles[3] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 0, 8);


            //Pose 2 - 把双手交叉 Both Hands Cross
            this.poseLibrary[1] = new Pose();
            this.poseLibrary[1].Title = "把手交叉（Hands Cross）";
            this.poseLibrary[1].Angles = new PoseAngle[4];
            this.poseLibrary[1].Angles[0] = new PoseAngle(JointType.ShoulderLeft, JointType.ElbowLeft, 245, 8);
            this.poseLibrary[1].Angles[1] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 0, 8);
            this.poseLibrary[1].Angles[2] = new PoseAngle(JointType.ShoulderRight, JointType.ElbowRight, 285, 8);
            this.poseLibrary[1].Angles[3] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 180, 8);


            //Pose 3 - 举起左手 Left Up and Right Down
            this.poseLibrary[2] = new Pose();
            this.poseLibrary[2].Title = "（举起左手）Left Up and Right Down";
            this.poseLibrary[2].Angles = new PoseAngle[4];
            this.poseLibrary[2].Angles[0] = new PoseAngle(JointType.ShoulderLeft, JointType.ElbowLeft, 95, 10);
            this.poseLibrary[2].Angles[1] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 95, 10);
            this.poseLibrary[2].Angles[2] = new PoseAngle(JointType.ShoulderRight, JointType.ElbowRight, 270, 30);
            this.poseLibrary[2].Angles[3] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 270, 30);


            //Pose 4 - 举起右手 Right Up and Left Down
            this.poseLibrary[3] = new Pose();
            this.poseLibrary[3].Title = "（举起右手）Right Up and Left Down";
            this.poseLibrary[3].Angles = new PoseAngle[4];
            this.poseLibrary[3].Angles[0] = new PoseAngle(JointType.ShoulderLeft, JointType.ElbowLeft, 270, 30);
            this.poseLibrary[3].Angles[1] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 270, 30);
            this.poseLibrary[3].Angles[2] = new PoseAngle(JointType.ShoulderRight, JointType.ElbowRight, 85, 10);
            this.poseLibrary[3].Angles[3] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 85, 10);
        }
       
        private enum State
        {
            TitleScreen,      // 初始片头
            Running,
           // GameOver,
            Success
        }
        State mcurrentstate = State.TitleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height, 1.0f, 1000.0f);
            view = Matrix.CreateLookAt(new Vector3(0.0f, 9.5f, -17.0f), new Vector3(0, 0, 0), new Vector3(0, 1, 0));//投影变换和摄像机的视角变换
            car = new car();
            car.view = view;
            car.projection = projection;
            road = new road();
            road.view = view;
            road.projection = projection;
            hazard = new Hazard();
            hazard.view = view;
            hazard.projection = projection;
            PopulatePoseLibrary();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()//加载模型和图片
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
             car.CarModel = Content.Load<Model>("Models//car");
            //mSpriteTexture = this.Content.Load<Texture2D>("SpeedWin");
             road.RoadModel = Content.Load<Model>("Models//road");
             mFont = Content.Load<SpriteFont>("Models//MyFont");
             mBackground = Content.Load<Texture2D>("Models//Background");
   
             hazard.HazardModel = Content.Load<Model>("Models//hazard");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();
            switch (mcurrentstate)
            {
                case State.TitleScreen://显示游戏画面但游戏没有开始，按下空格键后游戏正式开始
                    {
                        if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                        {
                            mcurrentstate = State.Running;
                        }
                        break;
                    }
                //case State.Success:
               case State.Success:
                    {
                        if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                        {
                            mcurrentstate = State.TitleScreen;
                        }
                        break;
                    }
                case State.Running:
                    {
                        if(car.carboundingsphere.Intersects(hazard.hazardsphere))//碰撞时，游戏失败退出
                        {
                            this.Exit();
                        }
                        else if(hazardnumber>=100)//通过大于100个障碍物时游戏胜利结束
                        {
                            mcurrentstate = State.Success;
                        }

                        else{
                            StartKinectST();
                        
                            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
                        roaddepth0 -= (float)(RoadSpeed * elapsed);//两条道路向前移动
                        roaddepth1 -= (float)(RoadSpeed * elapsed);
                        if (roaddepth0 < -75.0f)//当一条道路移出画面时，另一条在后面补上，制造出一条道路不断延伸的视觉效果
                        {
                            roaddepth0 = roaddepth1 + 100.0f;
                        }
                        if (roaddepth1 < -75.0f)
                        {
                            roaddepth1 = roaddepth0 + 100.0f;
                        }
                        if (!hazardvisible)//如果画面中午障碍物，构造新的障碍物对象
                        {
                            hazard = new Hazard();
                            hazard.HazardModel = Content.Load<Model>("Models//hazard");
                            hazard.view = view;
                            hazard.projection = projection;
                            hazard.addHazard(gameTime);
                          
                            hazard.hazardsphere.Center.Z = 60f;
                            hazard.hazardsphere.Center.X = hazard.haloc;
                            hazardvisible = true;
                            hazardnumber++;
                        }

                        xpositionc += Hv * elapsed;//更新障碍物移动距离
                        hazard.hazardsphere.Center.Z = (float)(60-xpositionc);//更新障碍物的位置
                        if (xpositionc > 70)//z障碍物移出画面时，更新障碍物可见标志
                        {
                            xpositionc = 0;
                            hazardvisible = false;

                        }
                       // checkinput();//检查左右键的输入一遍控制汽车移动
                        car.update(gameTime);
                        base.Update(gameTime);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue); 

           // spriteBatch.Begin();

            /*switch (mcurrentstate) {
                case State.TitleScreen:
                        {
                            spriteBatch.Draw(mBackground, new Rectangle(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);


                            DrawTextCentered("Drive Fast And Avoid the Oncoming Obstacles", 200);
                            DrawTextCentered("Press 'Space' to begin", 260);
                            

                            break;
                        }
               case State.Running:{*/
                
                        road.RoadDraw(gameTime, 0, 0, roaddepth0);
                        road.RoadDraw(gameTime, 0, 0, roaddepth1);
                        car.CarDraw(gameTime);
            
                        hazard.hazardmove(gameTime,xpositionc);
                       // break;
            //  }
              //  default:{

              /*   if (mcurrentstate == State.GameOver)
                {
                    DrawTextDisplayArea();

                    DrawTextCentered("Game Over.", 200);
                    DrawTextCentered("Press 'Space' to try again.", 260);
             

                }
            
                break;
                }*/
                   

   
       // }
            //spriteBatch.End();

            base.Draw(gameTime);
        }
       /* public void checkinput()//调整汽车的左右位置
        {
            KeyboardState newstate = Keyboard.GetState();
            if(newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                car.movingleft = true;
                car.movingright = false;
              //  car.carlocation = 2.5f;
            }
            if (newstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                car.movingleft  = false;
                car.movingright = true;
            //  car.carlocation =- 2.5f;
            }
        }
  /*      private void DrawTextDisplayArea()//并没有用上
        {
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (450 / 2));
            spriteBatch.Draw(mBackground, new Rectangle(aPositionX, 75, 450, 400), Color.White);
        }

        private void DrawTextCentered(string theDisplayText, int thePositionY)
        {
            Vector2 aSize = mFont.MeasureString(theDisplayText);
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (aSize.X / 2));

            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX, thePositionY), Color.Beige, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX + 1, thePositionY + 1), Color.Brown, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
        
        */


    }
}
