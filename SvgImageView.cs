using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.IO;
using System.Reflection;

namespace WeatherApp
{
    public class SvgImageView : ContentView
    {
        private SKSvg svg; // SKSvg object to hold the SVG data
        private readonly SKCanvasView canvasView; // SKCanvasView for drawing the SVG

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            nameof(Source), typeof(string), typeof(SvgImageView), null, propertyChanged: OnSourceChanged);

        public delegate void SvgLoadErrorEventHandler(string errorMessage);
        public event SvgLoadErrorEventHandler SvgLoadError; // Event for handling SVG loading errors

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public SvgImageView()
        {
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnPaintSurface; // Subscribe to the PaintSurface event
            Content = canvasView; // Set the ContentView to the SKCanvasView
        }

        // Event handler for changes in the Source property
        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SvgImageView svgImageView && newValue is string svgSource)
            {
                svgImageView.LoadSvg(svgSource); // Load the SVG when the Source property changes
            }
        }

        // Method to load an SVG file
        private void LoadSvg(string svgSource)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"{assembly.GetName().Name}.Resources.Images.{svgSource}";

                // Debug information to verify the constructed resource name
                Console.WriteLine($"Resource Name: {resourceName}");

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        svg = new SKSvg();
                        svg.Load(stream);
                        canvasView.InvalidateSurface();
                    }
                    else
                    {
                        SvgLoadError?.Invoke($"Failed to load SVG: {svgSource}. Resource stream is null.");
                    }
                }
            }
            catch (Exception ex)
            {
                SvgLoadError?.Invoke($"Failed to load SVG: {svgSource}. Exception: {ex.Message}");
            }
        }

        // Event handler for the PaintSurface event
        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas; // Get the canvas from the event arguments
            canvas.Clear(SKColors.Transparent); // Clear the canvas

            if (svg != null) // Check if an SVG is loaded
            {
                var canvasSize = e.Info.Size; // Get the size of the canvas
                var svgSize = svg.Picture.CullRect.Size; // Get the size of the SVG

                var scale = Math.Min(canvasSize.Width / svgSize.Width, canvasSize.Height / svgSize.Height); // Calculate the scale factor
                var matrix = SKMatrix.CreateScale(scale, scale); // Create a scale matrix

                canvas.DrawPicture(svg.Picture, ref matrix); // Draw the SVG on the canvas with the scale matrix
            }
        }
    }
}
