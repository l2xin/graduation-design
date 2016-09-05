using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

namespace ImageHelper
{
    public enum ImageType
    {
        Null,
        Png,
        Jpg,
        Gif,
        Bmp
    }

    public class ImageTypeHelper
    {
        public ImageTypeHelper()
        {

        }

        public static ImageType GetImageType(byte[] header)
        {
            //图片格式
            ImageType type = ImageType.Null;
            if (header == null || header.Length < 8)
                return ImageType.Null;
            //读取图片文件头8个字节，并根据若干个字节来确定图片格式

            //确定图片格式
            if (header[0] == 0x89 &&
                header[1] == 0x50 && // P
                header[2] == 0x4E && // N
                header[3] == 0x47 && // G
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A)
            {
                //Png图片 8字节：89 50 4E 47 0D 0A 1A 0A
                type = ImageType.Png;
            }
            else if (header[0] == 0xFF &&
                    header[1] == 0xD8)
            {
                //Jpg图片 2字节：FF D8
                type = ImageType.Jpg;
            }
            else if (header[0] == 0x47 &&   // G
                    header[1] == 0x49 &&    // I
                    header[2] == 0x46 &&    // F
                    header[3] == 0x38 &&    // 8
                    (header[4] == 0x39 ||   // 9
                    header[4] == 0x37) &&   // 7
                    header[5] == 0x61)      // a
            {
                //Gif图片 6字节：47 49 46 38 39|37 61
                type = ImageType.Gif;
            }
            else if (header[0] == 0x42 &&   //B
                    header[1] == 0x4D)      //M
            {
                //Bmp图片 2字节：42 4D
                type = ImageType.Bmp;
            }

            return type;
        }
    }

    public class GifReader
    {
#if UNITY_IOS
        [DllImport("__Internal")]
#else
        [DllImport("Gif", EntryPoint = "OpenGif", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr OpenGif(IntPtr pData, int len, ref int width, ref int height);

#if UNITY_IOS
        [DllImport("__Internal")]
#else
        [DllImport("Gif", EntryPoint = "CloseGif", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void CloseGif(IntPtr pGif);

        public GifReader()
        {
        }

        public Texture2D texture
        {
            get { return m_texture; }
        }

        public bool Open(byte[] data)
        {
            int len = data.Length;
            if (len < 1)
                return false;

            int size = Marshal.SizeOf(data[0]) * len;

            System.IntPtr pData = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, pData, len);

            int width = 0;
            int height = 0;

            System.IntPtr gifData = OpenGif(pData, len, ref width, ref height);

            ProcessTexture(gifData, width, height);

            Marshal.FreeHGlobal(pData);

            CloseGif(gifData);
            return (width > 0 && height > 0);
        }

        private void ProcessTexture(System.IntPtr ptr, int width, int height)
        {
            if (System.IntPtr.Zero == ptr || width < 1 || height < 1)
                return;

            int len = width * height;

            Color[] pixelColorArray = new Color[len];
            byte[] bytesData = new byte[len * 4];
            Marshal.Copy(ptr, bytesData, 0, len * 4);

            int counter = 0;
            Color pixelColor = new Color();
            const float f255_1 = 1.0f / 255.0f;

            int index = (height - 1) * width;

            for (int y = height - 1; y >= 0; --y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // Convert the bytes into color elements
                    pixelColor[0] = bytesData[counter++] * f255_1;
                    pixelColor[1] = bytesData[counter++] * f255_1;
                    pixelColor[2] = bytesData[counter++] * f255_1;
                    pixelColor[3] = bytesData[counter++] * f255_1;

                    pixelColorArray[index + x] = pixelColor;
                }

                index -= width;
            }

            if (null == m_texture || (null != m_texture && (m_texture.width != width || m_texture.height != height)))
            {
                m_texture = new Texture2D(width, height, TextureFormat.RGBA32, false);//, TextureFormat.ARGB32, true);
            }

            //set the anisotropic level and the filtering mode
            m_texture.anisoLevel = 0;
            m_texture.filterMode = FilterMode.Bilinear;

            // 左下为0点设置
            m_texture.SetPixels(pixelColorArray, 0);

            //apply the current texture, recalculate mipmaps and make this texture unwrittable
            m_texture.Apply(true, false);
        }

        private Texture2D m_texture;

        public static void GetImage(UITexture mTexture, byte[] mContent, string path)
        {
            ImageType mImageType = ImageTypeHelper.GetImageType(mContent);
            if (mImageType == ImageType.Gif)
            {
                GifReader gifReader = new GifReader();
                if (gifReader.Open(mContent))
                {
                    mTexture.mainTexture = gifReader.texture;
                }
                else
                {
                    mTexture.mainTexture = Resources.Load("default_face") as Texture;
                }
            }
            else if (mImageType == ImageType.Jpg || mImageType == ImageType.Png)
            {
                Texture2D m_loadPic = new Texture2D(100, 100);
                m_loadPic.LoadImage(mContent);
                mTexture.mainTexture = m_loadPic;
            }
        }
    }
}