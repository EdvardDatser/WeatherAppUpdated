using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace WeatherApp
{
    public partial class GifView : SKCanvasView
    {
        private SKCodec gifCodec;
        private bool isAnimating = false;
        private int currentFrameIndex = 0;
        private SKBitmap[] frames;

        public static readonly BindableProperty AspectProperty =
            BindableProperty.Create(
                nameof(Aspect),
                typeof(Aspect),
                typeof(GifView),
                Aspect.Fill);

        public Aspect Aspect
        {
            get { return (Aspect)GetValue(AspectProperty); }
            set { SetValue(AspectProperty, value); }
        }

        public GifView()
        {
            LoadGif();
            StartAnimation();
        }

        private void LoadGif()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "WeatherApp.Assets.Rain.gif";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    gifCodec = SKCodec.Create(stream);
                    frames = new SKBitmap[gifCodec.FrameCount];

                    for (int i = 0; i < gifCodec.FrameCount; i++)
                    {
                        SKImageInfo info = new SKImageInfo(gifCodec.Info.Width, gifCodec.Info.Height);
                        SKBitmap bitmap = new SKBitmap(info);
                        gifCodec.GetPixels(info, bitmap.GetPixels(), new SKCodecOptions(i));
                        frames[i] = bitmap;
                    }
                }
                else
                {
                    Console.WriteLine("Stream is null. Resource not found.");
                }
            }
        }

        public async Task StartAnimation()
        {
            if (gifCodec != null && !isAnimating)
            {
                isAnimating = true;
                while (isAnimating)
                {
                    // Invalidate the canvas to trigger a redraw
                    InvalidateSurface();

                    // Move to the next frame
                    currentFrameIndex = (currentFrameIndex + 1) % gifCodec.FrameCount;

                    // Delay for the duration of the frame
                    await Task.Delay(gifCodec.FrameInfo[currentFrameIndex].Duration);
                }
            }
        }

        public void StopAnimation()
        {
            isAnimating = false;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if (frames != null && frames.Length > 0)
            {
                e.Surface.Canvas.Clear();
                e.Surface.Canvas.DrawBitmap(frames[currentFrameIndex], new SKPoint(0, 0));
            }
        }
    }
}