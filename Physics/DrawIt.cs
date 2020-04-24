using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DNet.Common;
using Box2DNet.Dynamics;
using Processing;

namespace Physics
{
    public class DrawIt : DebugDraw
    {
        public CanvasArt Art;

        public override void DrawCircle(Vector2 center, float radius, Color color)
        {
            center = center.WorldToScreen();
            radius = radius * Extensions.Scale;

            Art.Stroke(color.ToPColor());
            Art.NoFill();
            Art.Circle(center.X, center.Y, radius);
        }

        public override void DrawPolygon(Vector2[] vertices, int vertexCount, Color color)
        {
            vertices = vertices.WorldToScreen();
            Art.Stroke(color.ToPColor());
            Art.NoFill();
            Art.BeginShape();
            foreach (var v in vertices)
            {
                Art.Vertex(v.X, v.Y);
            }
            Art.Vertex(vertices[0].X, vertices[0].Y);
            Art.EndShape(EndShapeType.Open);
        }

        public override void DrawSegment(Vector2 p1, Vector2 p2, Color color)
        {
            p1 = p1.WorldToScreen();
            p2 = p2.WorldToScreen();

            Art.NoFill();
            Art.Stroke(color.ToPColor());
            Art.Line((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            center = center.WorldToScreen();
            radius = radius * Extensions.Scale;

            Art.NoStroke();
            Art.Fill(color.ToPColor());
            Art.Circle(center.X, center.Y, radius);
        }

        public override void DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color)
        {
            vertices = vertices.WorldToScreen();
            Art.NoStroke();
            Art.Fill(color.ToPColor());
            Art.BeginShape();
            for (var i = 0; i < vertexCount; i++)
            {
                var v = vertices[i];
                Art.Vertex(v.X, v.Y);
            }
            Art.Vertex(vertices[0].X, vertices[0].Y);
            Art.EndShape(EndShapeType.Close);
        }

        public override void DrawTransform(Transform xf)
        {
            var pos = xf.position.WorldToScreen();
            Art.NoStroke();
            Art.Fill(PColor.White);
            Art.Circle(pos.X, pos.Y, 2);
        }
    }
}