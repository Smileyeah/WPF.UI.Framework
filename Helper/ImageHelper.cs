using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WPF.UI.Helpers
{
    public static class ImageHelper
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr obj);

        /// <summary>
        /// 取缩略图数据
        /// </summary>
        private static byte[] GetThumbnail(BitmapImage image)
        {
            Int32Rect sourceRect = default(Int32Rect);
            if (image.PixelWidth > image.PixelHeight)
            {
                double temp = (double)((image.PixelWidth - image.PixelHeight) / 2);
                sourceRect.X = System.Convert.ToInt32(temp);
                sourceRect.Y = 0;
                sourceRect.Width = System.Convert.ToInt32(image.PixelHeight);
                sourceRect.Height = System.Convert.ToInt32(image.PixelHeight);
            }
            else
            {
                double temp = (double)((image.PixelHeight - image.PixelWidth) / 2);
                sourceRect.X = 0;
                sourceRect.Y = System.Convert.ToInt32(temp);
                sourceRect.Width = System.Convert.ToInt32(image.PixelWidth);
                sourceRect.Height = System.Convert.ToInt32(image.PixelWidth);
            }

            int bits = 0;
            bool flag = false;
            if (image.Format.BitsPerPixel == 8)
            {
                bits = image.Format.BitsPerPixel * sourceRect.Width;
                flag = false;
            }
            else
            {
                if (image.Format.BitsPerPixel != 32)
                {
                    return null;
                }

                flag = true;
                bits = (sourceRect.Width * image.Format.BitsPerPixel + 31 & -32) / 8;
            }

            byte[] buffer = new byte[sourceRect.Height * bits];
            byte[] bytes = null;

            try
            {
                image.CopyPixels(sourceRect, buffer, bits, 0);
                if (null == buffer || 0 == bits)
                {
                    return null;
                }

                BitmapSource bmps = null;
                if ( flag )
                {
                    bmps = BitmapSource.Create(sourceRect.Width, sourceRect.Height, 96.0, 96.0, PixelFormats.Bgra32, null, buffer, bits);
                }
                else
                {
                    bmps = BitmapSource.Create(sourceRect.Width, sourceRect.Height, 96.0, 96.0, PixelFormats.Bgr24, null, buffer, bits);
                }

                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bmps));
                using (MemoryStream ms = new MemoryStream())
                {
                    jpegBitmapEncoder.Save(ms);
                    bytes = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return bytes;
        }

        /// <summary>
        /// 取缩略图数据
        /// </summary>
        private static byte[] GetThumbBytes(byte[] buffer, int dstHeight = 0, int dstWidth = 0)
        {
            if (null == buffer || buffer.Length <= 0)
            {
                return null;
            }

            byte[] dst = null;

            MemoryStream    ms = null;
            BitmapImage     image = null;
            try
            {
                ms = new MemoryStream(buffer);
                image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;

                if (dstWidth > 0)
                {
                    image.DecodePixelWidth = dstWidth;
                }
                if (dstHeight > 0)
                {
                    image.DecodePixelHeight = dstHeight;
                }
                image.EndInit();

                dst = ImageHelper.GetThumbnail(image);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != ms)
            {
                ms.Dispose();
            }
            return dst;
        }

        /// <summary>
        /// 取缩略图数据
        /// </summary>
        public static byte[] GetThumbBytes(string localPath, int width = 0, int height = 0)
        {
            if (String.IsNullOrEmpty(localPath) || !System.IO.File.Exists(localPath))
            {
                return null;
            }

            byte[] bytes = null;

            FileStream fs = null;
            try
            {
                fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (null != fs && fs.Length > 0L)
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    bytes = ImageHelper.GetThumbBytes(buffer, width, height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != fs)
            {
                fs.Dispose();
            }
            return bytes;
        }

        /// <summary>
        /// 取缩略图
        /// </summary>
        public static BitmapImage GetThumbImage(byte[] buffer, int dstHeight = 0, int dstWidth = 0)
        {
            if (null == buffer || buffer.Length <= 0)
            {
                return null;
            }
            
            MemoryStream    ms = null;
            BitmapImage     dst = null;
            try
            {
                ms = new MemoryStream(buffer);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;

                if (dstWidth > 0)
                {
                    image.DecodePixelWidth = dstWidth;
                }
                if (dstHeight > 0)
                {
                    image.DecodePixelHeight = dstHeight;
                }
                image.EndInit();
                dst = image;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != ms)
            {
                ms.Dispose();
            }
            return dst;
        }

        /// <summary>
        /// 取缩略图
        /// </summary>
        public static Bitmap GetThumbBitmap(Bitmap src, int dstHeight, int dstWidth)
        {
            ImageFormat imageFormat = src.RawFormat;
            int iWidth = src.Width;
            int iHeight = src.Height;
            int tempWidth = 0;
            int tempHeight = 0;
            if (iHeight > dstHeight || iWidth > dstWidth)
            {
                if (iWidth * dstHeight > iHeight * dstWidth)
                {
                    tempWidth = dstWidth;
                    tempHeight = dstWidth * iHeight / iWidth;
                }
                else
                {
                    tempHeight = dstHeight;
                    tempWidth = iWidth * dstHeight / iHeight;
                }
            }
            else
            {
                tempWidth = iWidth;
                tempHeight = iHeight;
            }

            Bitmap      dst = null;
            Graphics    gphs = null;
            try
            {
                dst = new Bitmap(dstWidth, dstHeight);
                gphs = Graphics.FromImage(dst);
                gphs.Clear(System.Drawing.Color.Transparent);
                gphs.CompositingQuality = CompositingQuality.HighQuality;
                gphs.SmoothingMode = SmoothingMode.HighQuality;
                gphs.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gphs.DrawImage(src, new Rectangle((dstWidth - tempWidth) / 2, (dstHeight - tempHeight) / 2, tempWidth, tempHeight), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                if (null != dst)
                {
                    dst.Dispose();
                    dst = null;
                }
            }
            if (null != gphs)
            {
                gphs.Dispose();
            }
            
            return dst;
        }


        /// <summary>
        /// 更改Image大小
        /// </summary>
        public static Bitmap Resize(Bitmap bmp, int width, int height, int mode)
        {
            Bitmap dst = null;

            Graphics gphs = null;
            try
            {
                dst = new Bitmap(width, height);
                gphs = Graphics.FromImage(dst);
                gphs.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gphs.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                if (null != dst)
                {
                    dst.Dispose();
                    dst = null;
                }
            }
            if (null != gphs)
            {
                gphs.Dispose();
            }
            return dst;
        }


        /// <summary>
        /// 将Image旋转固定角度
        /// </summary>
        public static Bitmap Rotate(Image image, int angle)
        {
            angle %= 360;

            double expr = (double)angle * 3.1415926535897931 / 180.0;
            double cos = Math.Cos(expr);
            double sin = Math.Sin(expr);

            Bitmap      bmp = null;
            Graphics    gphs = null;
            try
            {
                int width = (int)Math.Max(Math.Abs((double)image.Width * cos - (double)image.Height * sin), Math.Abs((double)image.Width * cos + (double)image.Height * sin));
                int height = (int)Math.Max(Math.Abs((double)image.Width * sin - (double)image.Height * cos), Math.Abs((double)image.Width * sin + (double)image.Height * cos));

                System.Drawing.Point ptf = new System.Drawing.Point((width - image.Width) / 2, (height - image.Height) / 2);
                Rectangle rect = new Rectangle(ptf.X, ptf.Y, image.Width, image.Height);
                System.Drawing.Point pts = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                bmp = new Bitmap(width, height);
                gphs = Graphics.FromImage(bmp);
                gphs.InterpolationMode = InterpolationMode.Bilinear;
                gphs.SmoothingMode = SmoothingMode.HighQuality;
                gphs.TranslateTransform((float)pts.X, (float)pts.Y);
                gphs.RotateTransform((float)(360 - angle));
                gphs.TranslateTransform((float)(-(float)pts.X), (float)(-(float)pts.Y));
                gphs.DrawImage(image, rect);
                gphs.ResetTransform();
                gphs.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                if (null != bmp)
                {
                    bmp.Dispose();
                }
            }
            if (null != gphs)
            {
                gphs.Dispose();
            }
            return bmp;
        }


        /// <summary>
        /// 将字节数组直接转BitmapSource对象
        /// </summary>
        public static BitmapImage Convert(byte[] imageBytes)
        {
            if (null == imageBytes || imageBytes.Length <= 0)
            {
                return null;
            }

            BitmapImage     image = null;
            MemoryStream    ms = null;

            try
            {
                ms = new MemoryStream(imageBytes);

                BitmapImage temp = new BitmapImage();
                temp.BeginInit();
                temp.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                temp.CacheOption = BitmapCacheOption.OnLoad;
                temp.StreamSource = ms;
                temp.EndInit();
                temp.Freeze();
                image = temp;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != ms)
            {
                ms.Dispose();
            }
            return image;
        }

        /// <summary>
        /// 将字节数组直接转BitmapSource对象
        /// </summary>
        public static BitmapImage Convert(byte[] imageBytes, int dstWidth = 0, int dstHeight = 0)
        {
            if (null == imageBytes || imageBytes.Length <= 0)
            {
                return null;
            }

            BitmapImage     image = null;
            MemoryStream    ms = null;

            try
            {
                ms = new MemoryStream(imageBytes);

                BitmapImage temp = new BitmapImage();
                temp.BeginInit();
                temp.DecodePixelWidth = dstWidth;
                temp.DecodePixelHeight = dstHeight;
                temp.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                temp.CacheOption = BitmapCacheOption.OnLoad;
                temp.StreamSource = ms;
                temp.EndInit();
                temp.Freeze();
                image = temp;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != ms)
            {
                ms.Dispose();
            }
            return image;
        }

        /// <summary>
        /// 将字节数组转Bitmap，然后转BitmapSource对象
        /// </summary>
        public static BitmapSource Transform(byte[] imageBytes)
        {
            if (null == imageBytes || imageBytes.Length <= 0)
            {
                return null;
            }

            BitmapSource    bmps = null;
            MemoryStream    ms = null;
            Bitmap          bmp = null;

            try
            {
                ms = new MemoryStream(imageBytes);
                bmp = new Bitmap(ms);
                bmps = ImageHelper.Transform(bmp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != bmp)
            {
                bmp.Dispose();
            }
            if (null != ms)
            {
                ms.Dispose();
            }
            return bmps;
        }

        /// <summary>
        /// 将字节数组转Bitmap计算大小，然后转进行一定缩放了的BitmapSource对象
        /// </summary>
        public static BitmapSource Transform(byte[] imageBytes, int minWidth, int minHeight)
        {
            if (null == imageBytes || imageBytes.Length <= 0)
            {
                return null;
            }

            BitmapSource    bmps = null;
            MemoryStream    ms = null;
            Bitmap          bmpl = null;
            
            try
            {
                ms = new MemoryStream(imageBytes);
                bmpl = new Bitmap(ms);

                double tempScaleX = minWidth / bmpl.Width;
                double tempScaleY = minHeight / bmpl.Height;
                double tempScale = (tempScaleX > tempScaleY) ? tempScaleX : tempScaleY;

                int tempWidth = (int)(bmpl.Width * tempScale);
                int tempHeight = (int)(bmpl.Height * tempScale);
                bmps = ImageHelper.Convert(imageBytes, tempWidth, tempHeight);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (null != bmpl)
            {
                bmpl.Dispose();
            }
            if (null != ms)
            {
                ms.Dispose();
            }

            return bmps;
        }

        /// <summary>
        /// 将Bitmap转BitmapSource对象
        /// </summary>
        public static BitmapSource Transform(Bitmap bmp)
        {
            BitmapSource bmps = null;
            IntPtr iptr = IntPtr.Zero;
            try
            {
                iptr = bmp.GetHbitmap();
                bmps = Imaging.CreateBitmapSourceFromHBitmap(iptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (IntPtr.Zero != iptr)
            {
                ImageHelper.DeleteObject(iptr);
            }
            return bmps;
        }

    }

}
