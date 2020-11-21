using Processing;

namespace Processing_Test
{
    class Processing_UpgradeTesting : PCanvas
    {
        public Processing_UpgradeTesting() =>
            CreateCanvas(1000, 1000, 60);

        public override void Draw(float delta)
        {
            Art.Background(Paint.CornflowerBlue);
            Art.Circle(Width / 2, TotalFrameCount, 20);
        }
    }
}
