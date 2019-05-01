using System;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SeekiosApp.iOS.Helper
{
    public class ImageHelper
    {
        /// <summary>
        /// Froms the URL.
        /// </summary>
        /// <returns>The URL.</returns>
        /// <param name="uri">URI.</param>
        public static UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
            {
                return UIImage.LoadFromData(data);
            }
        }

        /// <summary>
        /// Bytes to image.
        /// </summary>
        /// <returns>The to image.</returns>
        /// <param name="stringArray">String array.</param>
        /// <param name="width">Width.</param>
        /// <param name="radius">Radius.</param>
        public static UIImage ByteToImage(string stringArray, int width, float radius)
        {
            if (string.IsNullOrEmpty(stringArray)) return null;

            var imageBytes = Convert.FromBase64String(stringArray);
            var imageData = NSData.FromArray(imageBytes);
            var uiImage = ResizeImage(UIImage.LoadFromData(imageData), width, width);
            return CircleImage(uiImage, width, radius);
        }

        public static UIImage ByteToImage(string stringArray)
        {
            if (string.IsNullOrEmpty(stringArray)) return null;

            var imageBytes = Convert.FromBase64String(stringArray);
            var imageData = NSData.FromArray(imageBytes);
            return UIImage.LoadFromData(imageData);
        }

        /// <summary>
        /// Images to byte.
        /// </summary>
        /// <returns>The to byte.</returns>
        /// <param name="image">Image.</param>
        public static byte[] ImageToByte(UIImage image)
        {
            using (NSData imageData = image.AsPNG())
            {
                byte[] imageByteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, imageByteArray, 0, Convert.ToInt32(imageData.Length));
                return imageByteArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The to base64 string.</returns>
        /// <param name="image">Image.</param>
        public static string ImageToBase64String(UIImage image)
        {
            var imageData = image.AsJPEG(0.5f);
            return imageData.GetBase64EncodedData(NSDataBase64EncodingOptions.None).ToString();
        }

        /// <summary>
        /// Strings to image.
        /// </summary>
        /// <returns>The to image.</returns>
        /// <param name="imageStr">Image string.</param>
        /// <param name="width">Width.</param>
        /// <param name="radius">Radius.</param>
        public static UIImage StringToImage(string imageStr, int width, float radius)
        {
            var imageCell = ResizeImage(UIImage.FromBundle(imageStr), width, width);
            return CircleImage(imageCell, width, radius);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SeekiosApp.iOS.Helper.ImageHelper"/> is retina.
        /// </summary>
        /// <value><c>true</c> if is retina; otherwise, <c>false</c>.</value>
        public static bool IsRetina
        {
            get { return true; }
        }

        /// <summary>
        /// Circles the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="imageCell">Image cell.</param>
        /// <param name="width">Width.</param>
        /// <param name="radius">Radius.</param>
        public static UIImage CircleImage(UIImage imageCell, int width, float radius)
        {
            UIGraphics.BeginImageContext(new SizeF(width, width));
            UIImage modifiedImage = null;

            using (var context = UIGraphics.GetCurrentContext())
            {
                radius = IsRetina ? radius * 2 : radius;

                context.BeginPath();
                context.MoveTo(width, width / 2);
                context.AddArcToPoint(width, width, width / 2, width, radius);
                context.AddArcToPoint(0, width, 0, width / 2, radius);
                context.AddArcToPoint(0, 0, width / 2, 0, radius);
                context.AddArcToPoint(width, 0, width, width / 2, radius);
                context.ClosePath();
                context.Clip();

                imageCell.Draw(new PointF(0, 0));
                modifiedImage = UIGraphics.GetImageFromCurrentImageContext();

                UIGraphics.EndImageContext();
            }
            return modifiedImage;
        }

        /// <summary>
        /// Maxs the resize image.
        /// </summary>
        /// <returns>The resize image.</returns>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="maxWidth">Max width.</param>
        /// <param name="maxHeight">Max height.</param>
        // resize the image to be contained within a maximum width and height, keeping aspect ratio
        public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        // resize the image (without trying to maintain aspect ratio)
        public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            if (sourceImage == null)
            {
                sourceImage = UIImage.FromBundle("DefaultUser");
            }

            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        /// <summary>
        /// Crops the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="crop_x">Crop x.</param>
        /// <param name="crop_y">Crop y.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        // crop the image, without resizing
        public static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIImage modifiedImage = null;
            UIGraphics.BeginImageContext(new SizeF(width, height));
            using (var context = UIGraphics.GetCurrentContext())
            {
                var clippedRect = new RectangleF(0, 0, width, height);
                context.ClipToRect(clippedRect);
                var drawRect = new CGRect(-crop_x, -crop_y, imgSize.Width, imgSize.Height);
                sourceImage.Draw(drawRect);
                modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }
            return modifiedImage;
        }

        public static UIImage GetColoredImage(string imageName, UIColor color)
        {
            UIImage image = UIImage.FromBundle(imageName);
            UIImage coloredImage = null;

            UIGraphics.BeginImageContext(image.Size);
            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                context.TranslateCTM(0, image.Size.Height);
                context.ScaleCTM(1.0f, -1.0f);

                var rect = new RectangleF(0, 0, (float)image.Size.Width, (float)image.Size.Height);

                // draw image, (to get transparancy mask)
                context.SetBlendMode(CGBlendMode.Normal);
                context.DrawImage(rect, image.CGImage);

                // draw the color using the sourcein blend mode so its only draw on the non-transparent pixels
                context.SetBlendMode(CGBlendMode.SourceIn);
                context.SetFillColor(color.CGColor);
                context.FillRect(rect);

                coloredImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            return coloredImage;
        }
    }
}

