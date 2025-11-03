using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Form1 : Form
    {
        private List<LineSegment> lines = new List<LineSegment>();

        public Form1()
        {
            InitializeComponent();
        }

        private void DrawFractal_Click(object sender, EventArgs e)
        {
            lines.Clear();

            var center = new PointF(canvasPanel.Width / 4, canvasPanel.Height / 3);
            var sideLength = Math.Min(canvasPanel.Width, canvasPanel.Height) * 0.4f;
            var maxDepth = 8;
            var color = Color.Yellow;
            float initialRotation = 0;

            DrawTriangle(center, sideLength, initialRotation, 2, color, 0.5f, 0, maxDepth);
            canvasPanel.Invalidate();
        }

        private void DrawTriangle(PointF center, float sideLength, float degreesRotate, int thickness, Color color,
                                float shrinkSideBy, int iteration, int maxDepth)
        {
            float triangleHeight = sideLength * (float)Math.Sqrt(3) / 2;

            PointF top = new PointF(center.X - triangleHeight / 2, center.Y);
            PointF bottomLeft = new PointF(center.X + triangleHeight / 2, center.Y - sideLength / 2);
            PointF bottomRight = new PointF(center.X + triangleHeight / 2, center.Y + sideLength / 2);

            if (degreesRotate != 0)
            {
                top = Rotate(top, center, degreesRotate);
                bottomLeft = Rotate(bottomLeft, center, degreesRotate);
                bottomRight = Rotate(bottomRight, center, degreesRotate);
            }

            var lines = new[] {
                (top, bottomLeft),
                (top, bottomRight),
                (bottomLeft, bottomRight)
            };

            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;
                PlotLine(line.Item1, line.Item2, thickness, color);

                if (iteration < maxDepth && (iteration < 1 || lineNumber < 3))
                {
                    float gradient = (line.Item2.X - line.Item1.X) / (line.Item2.Y - line.Item1.Y);

                    float newSideLength = sideLength * shrinkSideBy;

                    PointF centerOfLine = new PointF(
                        (line.Item1.X + line.Item2.X) / 2,
                        (line.Item1.Y + line.Item2.Y) / 2
                    );

                    PointF newCenter = PointF.Empty;
                    float newRotation = degreesRotate;

                    if (lineNumber == 1)
                        newRotation += 60;
                    else if (lineNumber == 2)
                        newRotation -= 60;
                    else
                        newRotation += 180;

                    if (gradient < 0.0001 && gradient > -0.0001)
                    {
                        if (centerOfLine.X - center.X > 0)
                            newCenter = new PointF(
                                centerOfLine.X + triangleHeight * (shrinkSideBy / 2),
                                centerOfLine.Y
                            );
                        else
                            newCenter = new PointF(
                                centerOfLine.X - triangleHeight * (shrinkSideBy / 2),
                                centerOfLine.Y
                            );
                    }
                    else
                    {
                        float differenceFromCenter = -1 / gradient;

                        float distanceFromCenter = triangleHeight * (shrinkSideBy / 2);

                        float xLength = (float)Math.Sqrt(
                            (distanceFromCenter * distanceFromCenter) /
                            (1 + differenceFromCenter * differenceFromCenter)
                        );

                        if (centerOfLine.Y < center.Y && xLength > 0)
                            xLength *= -1;

                        float yLength = xLength * differenceFromCenter;

                        newCenter = new PointF(
                            centerOfLine.X + yLength,
                            centerOfLine.Y + xLength
                        );
                    }

                    DrawTriangle(newCenter, newSideLength, newRotation, thickness,
                                color, shrinkSideBy, iteration + 1, maxDepth);
                }
            }
        }

        private static PointF Rotate(PointF point, PointF center, float degrees)
        {
            double angle = degrees * Math.PI / 180.0;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            float x = point.X - center.X;
            float y = point.Y - center.Y;

            float newX = (float)(x * cos - y * sin);
            float newY = (float)(x * sin + y * cos);

            return new PointF(newX + center.X, newY + center.Y);
        }

        private void PlotLine(PointF from, PointF to, int thickness, Color color)
        {
            lines.Add(new LineSegment { From = from, To = to, Thickness = thickness, Color = color });
        }

        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var line in lines)
            {
                using (var pen = new Pen(line.Color, line.Thickness))
                {
                    g.DrawLine(pen, line.From, line.To);
                }
            }
        }

        private class LineSegment
        {
            public PointF From { get; set; }
            public PointF To { get; set; }
            public int Thickness { get; set; }
            public Color Color { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}