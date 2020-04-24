using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Processing;
using Box2DNet;
using Box2DNet.Dynamics;
using Box2DNet.Collision;
using System.Numerics;
using Box2DNet.Common;
using static Box2DNet.Dynamics.DebugDraw;
using static Box2DNet.Dynamics.Body;

namespace Physics
{
    public class MainWindow : ProcessingCanvas
    {
        World World;
        DrawIt draw;

        public MainWindow() { CreateCanvas(1000, 1000, 60); }

        public void Setup()
        {
            Form.FormPictureBox.MouseDown += Form_MouseDown;
            Form.FormPictureBox.MouseUp += Form_MouseUp;

            Extensions.ScreenSize = Width;
            Extensions.Scale = 15f;
            var s = Extensions.Scale;

            var aabb = new AABB() { LowerBound = new Vector2(-10, -10), UpperBound = new Vector2(10, 10) };
            World = new World(aabb, new Vector2(0, -10), false);
            draw = new DrawIt() { Art = Art };
            draw.Flags = (DrawFlags)255;
            World.SetDebugDraw(draw);
            Art.StrokeWeight(2);
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (DownPos == null)
            {
                return;
            }

            var UpPos = new Vector2(e.X, e.Y);

            var def = new BodyDef();
            def.Position = DownPos.ScreenToWorld();
            var body = World.CreateBody(def);

            var p = new CircleDef();
            p.Density = 1;
            p.Restitution = 0;
            p.Radius = 1;
            BodyType
            
            var f = body.CreateFixture(p);

            body.SetMassFromShapes();
        }

        Vector2 DownPos;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            DownPos = new Vector2(e.X, e.Y);
        }

        public void Draw(float delta)
        {
            Title(FrameRateCurrent);
            Art.Background(PColor.Grey);
            World.Step(1f/ 60f, 8, 1);
        }
    }
}